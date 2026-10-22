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

        if (!string.IsNullOrEmpty(group))
        {
            var donorGroups = BloodHelper.DonorsFor(group);
            q = q.Where(u => donorGroups.Contains(u.BloodGroup));
        }
        if (!string.IsNullOrWhiteSpace(city)) q = q.Where(u => u.City.Contains(city.Trim()));

        ViewBag.Group = group; ViewBag.City = city;
        return View(await q.OrderBy(u => u.City).ThenBy(u => u.FullName).ToListAsync());
    }
}
