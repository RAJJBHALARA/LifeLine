namespace LifeLine.Controllers;

public class RequestsController : Controller
{
    private readonly AppDbContext _db;
    public RequestsController(AppDbContext db) => _db = db;

    // Browse open requests with filters.
    public async Task<IActionResult> Index(string? group, string? city, Urgency? urgency)
    {
        var q = _db.Requests.Include(r => r.Responses)
            .Where(r => r.Status == RequestStatus.Open);

        if (!string.IsNullOrEmpty(group)) q = q.Where(r => r.BloodGroup == group);
        if (!string.IsNullOrWhiteSpace(city)) q = q.Where(r => r.City.Contains(city.Trim()));
        if (urgency.HasValue) q = q.Where(r => r.Urgency == urgency.Value);

        ViewBag.Group = group; ViewBag.City = city; ViewBag.Urgency = urgency;
        var list = await q.OrderByDescending(r => r.Urgency).ThenByDescending(r => r.CreatedAt).ToListAsync();
        return View(list);
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> Create()
    {
        var me = await _db.Users.FindAsync(User.GetId());
        return View(new CreateRequestVm { City = me?.City ?? "" , ContactPhone = me?.Phone ?? "" });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateRequestVm vm)
    {
        if (!BloodHelper.Groups.Contains(vm.BloodGroup))
            ModelState.AddModelError(nameof(vm.BloodGroup), "Select a valid blood group.");
        if (!ModelState.IsValid) return View(vm);

        var req = new BloodRequest
        {
            PatientName = vm.PatientName.Trim(), BloodGroup = vm.BloodGroup, Units = vm.Units,
            City = vm.City.Trim(), Hospital = vm.Hospital.Trim(), ContactPhone = vm.ContactPhone.Trim(),
            Urgency = vm.Urgency, Notes = vm.Notes?.Trim(), RequesterId = User.GetId()
        };
        _db.Requests.Add(req);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Blood request posted. Matching donors can now respond.";
        return RedirectToAction(nameof(Details), new { id = req.Id });
    }

    public async Task<IActionResult> Details(int id)
    {
        var req = await _db.Requests
            .Include(r => r.Requester)
            .Include(r => r.Responses).ThenInclude(x => x.Donor)
            .FirstOrDefaultAsync(r => r.Id == id);
        if (req == null) return NotFound();

        var loggedIn = User.Identity?.IsAuthenticated == true;
        var uid = loggedIn ? User.GetId() : 0;

        ViewBag.IsOwner = loggedIn && req.RequesterId == uid;
        ViewBag.HasResponded = loggedIn && req.Responses.Any(r => r.DonorId == uid);

        string? reason = null;
        if (loggedIn)
        {
            var me = await _db.Users.FindAsync(uid);
            reason = me == null ? "Please sign in." : CheckCanRespond(req, me);
        }
        ViewBag.CannotRespondReason = reason;
        return View(req);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(int id, string? message)
    {
        var req = await _db.Requests.Include(r => r.Responses).FirstOrDefaultAsync(r => r.Id == id);
        if (req == null) return NotFound();
        var me = await _db.Users.FindAsync(User.GetId());
        if (me == null) return Forbid();

        var error = CheckCanRespond(req, me);
        if (error != null)
        {
            TempData["Error"] = error;
            return RedirectToAction(nameof(Details), new { id });
        }

        // Truncate message to 300 chars as an extra safety layer
        var safeMessage = message?.Trim();
        if (safeMessage?.Length > 300) safeMessage = safeMessage[..300];

        _db.Responses.Add(new DonorResponse
        {
            RequestId = id, DonorId = me.Id, Message = safeMessage, RespondedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        TempData["Success"] = "Thank you! The requester can now see your contact details.";
        return RedirectToAction(nameof(Details), new { id });
    }

    // Requester confirms a donor actually donated -> starts the donor's 90-day cooldown.
    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkDonated(int responseId)
    {
        var resp = await _db.Responses
            .Include(r => r.Donor)
            .Include(r => r.Request).ThenInclude(r => r.Responses)
            .FirstOrDefaultAsync(r => r.Id == responseId);
        if (resp == null) return NotFound();
        if (resp.Request.RequesterId != User.GetId()) return Forbid();

        if (resp.Status != ResponseStatus.Donated)
        {
            resp.Status = ResponseStatus.Donated;
            resp.Donor.LastDonationDate = DateTime.Today;

            var donated = resp.Request.Responses.Count(r => r.Status == ResponseStatus.Donated);
            if (donated >= resp.Request.Units) resp.Request.Status = RequestStatus.Fulfilled;

            await _db.SaveChangesAsync();
            TempData["Success"] = "Marked as donated. The donor is now on a 90-day cooldown.";
        }
        return RedirectToAction(nameof(Details), new { id = resp.RequestId });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Close(int id)
    {
        var req = await _db.Requests.FindAsync(id);
        if (req == null) return NotFound();
        if (req.RequesterId != User.GetId()) return Forbid();

        req.Status = RequestStatus.Closed;
        await _db.SaveChangesAsync();
        TempData["Success"] = "Request closed.";
        return RedirectToAction(nameof(My));
    }

    // My requests and my responses.
    [Authorize]
    public async Task<IActionResult> My()
    {
        var uid = User.GetId();
        ViewBag.MyRequests = await _db.Requests.Include(r => r.Responses)
            .Where(r => r.RequesterId == uid).OrderByDescending(r => r.CreatedAt).ToListAsync();
        var responses = await _db.Responses.Include(r => r.Request)
            .Where(r => r.DonorId == uid).OrderByDescending(r => r.RespondedAt).ToListAsync();
        return View(responses);
    }

    // Returns null when the user may respond, otherwise the reason they can't.
    private static string? CheckCanRespond(BloodRequest req, AppUser me)
    {
        if (req.Status != RequestStatus.Open) return "This request is no longer open.";
        if (req.RequesterId == me.Id) return "You cannot respond to your own request.";
        if (req.Responses.Any(r => r.DonorId == me.Id)) return "You have already responded.";
        if (!me.IsDonor) return "Enable 'Listed as a donor' in your profile to respond.";
        if (!BloodHelper.CanDonateTo(me.BloodGroup, req.BloodGroup))
            return $"Your blood group ({me.BloodGroup}) is not compatible with {req.BloodGroup}.";
        if (!me.IsEligible)
            return $"You are in the {BloodHelper.CooldownDays}-day cooldown. Eligible again on {me.NextEligibleDate:dd MMM yyyy}.";
        return null;
    }
}
