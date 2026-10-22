using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace LifeLine.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _db;
    private readonly PasswordHasher<AppUser> _hasher = new();

    public AccountController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult Register() => View(new RegisterVm());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm vm)
    {
        if (vm.LastDonationDate > DateTime.Today)
            ModelState.AddModelError(nameof(vm.LastDonationDate), "Date cannot be in the future.");
        if (!BloodHelper.Groups.Contains(vm.BloodGroup))
            ModelState.AddModelError(nameof(vm.BloodGroup), "Select a valid blood group.");

        var email = vm.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email))
            ModelState.AddModelError(nameof(vm.Email), "This email is already registered.");

        if (!ModelState.IsValid) return View(vm);

        var user = new AppUser
        {
            FullName = vm.FullName.Trim(), Email = email, Phone = vm.Phone.Trim(),
            BloodGroup = vm.BloodGroup, City = vm.City.Trim(), Area = vm.Area?.Trim(),
            LastDonationDate = vm.LastDonationDate, IsDonor = vm.IsDonor
        };
        user.PasswordHash = _hasher.HashPassword(user, vm.Password);
        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        await SignInUser(user);
        TempData["Success"] = "Welcome to LifeLine! Your account has been created.";
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginVm { ReturnUrl = returnUrl });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var email = vm.Email.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
        var ok = user != null &&
                 _hasher.VerifyHashedPassword(user, user.PasswordHash, vm.Password) != PasswordVerificationResult.Failed;
        if (!ok)
        {
            ModelState.AddModelError("", "Invalid email or password.");
            return View(vm);
        }

        await SignInUser(user!);
        if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
            return LocalRedirect(vm.ReturnUrl);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Success"] = "You have been logged out.";
        return RedirectToAction("Index", "Home");
    }

    [Authorize, HttpGet]
    public async Task<IActionResult> Profile()
    {
        var u = await _db.Users.FindAsync(User.GetId());
        if (u == null) return RedirectToAction(nameof(Login));
        ViewBag.User = u;
        return View(new ProfileVm
        {
            FullName = u.FullName, Phone = u.Phone, BloodGroup = u.BloodGroup, City = u.City,
            Area = u.Area, LastDonationDate = u.LastDonationDate, IsDonor = u.IsDonor
        });
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileVm vm)
    {
        var u = await _db.Users.FindAsync(User.GetId());
        if (u == null) return RedirectToAction(nameof(Login));
        ViewBag.User = u;

        if (vm.LastDonationDate > DateTime.Today)
            ModelState.AddModelError(nameof(vm.LastDonationDate), "Date cannot be in the future.");
        if (!BloodHelper.Groups.Contains(vm.BloodGroup))
            ModelState.AddModelError(nameof(vm.BloodGroup), "Select a valid blood group.");
        if (!ModelState.IsValid) return View(vm);

        u.FullName = vm.FullName.Trim(); u.Phone = vm.Phone.Trim(); u.BloodGroup = vm.BloodGroup;
        u.City = vm.City.Trim(); u.Area = vm.Area?.Trim();
        u.LastDonationDate = vm.LastDonationDate; u.IsDonor = vm.IsDonor;
        await _db.SaveChangesAsync();

        // Update the claims so the navbar shows the new name immediately
        await SignInUser(u);
        TempData["Success"] = "Profile updated.";
        return RedirectToAction(nameof(Profile));
    }

    private async Task SignInUser(AppUser u)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, u.Id.ToString()),
            new(ClaimTypes.Name, u.FullName),
            new(ClaimTypes.Email, u.Email)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity),
            new AuthenticationProperties { IsPersistent = true });
    }
}
