namespace LifeLine.Controllers;

public class HomeController : Controller
{
    private readonly AppDbContext _db;
    public HomeController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var cutoff = DateTime.Today.AddDays(-BloodHelper.CooldownDays);
        ViewBag.Donors = await _db.Users.CountAsync(u => u.IsDonor);
        ViewBag.Eligible = await _db.Users.CountAsync(u => u.IsDonor && (u.LastDonationDate == null || u.LastDonationDate <= cutoff));
        ViewBag.Open = await _db.Requests.CountAsync(r => r.Status == RequestStatus.Open);
        ViewBag.Fulfilled = await _db.Requests.CountAsync(r => r.Status == RequestStatus.Fulfilled);

        var latest = await _db.Requests.Include(r => r.Responses)
            .Where(r => r.Status == RequestStatus.Open)
            .OrderByDescending(r => r.Urgency).ThenByDescending(r => r.CreatedAt)
            .Take(3).ToListAsync();
        return View(latest);
    }

    public IActionResult Error() => View();
}
