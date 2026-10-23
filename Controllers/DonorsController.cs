namespace LifeLine.Controllers;

public class DonorsController : Controller
{
    private readonly AppDbContext _db;
    public DonorsController(AppDbContext db) => _db = db;

    // Search donors who are currently eligible (90-day rule) and compatible with the needed group.
    public async Task<IActionResult> Index(string? group, string? city)
    {
        var cutoff = DateTime.Today.AddDays(-BloodHelper.CooldownDays);
        var q = _db.Users.Where(u => u.IsDonor && (u.LastDonationDate == null || u.LastDonationDate <= cutoff));

        List<string>? compatibleDonors = null;
        if (!string.IsNullOrEmpty(group))
        {
            compatibleDonors = BloodHelper.DonorsFor(group);
            q = q.Where(u => compatibleDonors.Contains(u.BloodGroup));
        }
        if (!string.IsNullOrWhiteSpace(city)) 
            q = q.Where(u => u.City.ToLower().Contains(city.Trim().ToLower()));

        ViewBag.Group = group; 
        ViewBag.City = city;
        ViewBag.CompatibleDonors = compatibleDonors;
        return View(await q.OrderBy(u => u.City).ThenBy(u => u.FullName).ToListAsync());
    }
}
