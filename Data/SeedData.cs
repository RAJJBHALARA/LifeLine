using Microsoft.AspNetCore.Identity;

namespace LifeLine.Data;

public static class SeedData
{
    // All demo accounts use this password.
    public const string DemoPassword = "Password@123";

    public static void Run(AppDbContext db)
    {
        if (db.Users.Any()) return;

        var hasher = new PasswordHasher<AppUser>();

        AppUser Make(string name, string email, string phone, string group, string city, string area, int? daysSinceDonation)
        {
            var u = new AppUser
            {
                FullName = name, Email = email, Phone = phone, BloodGroup = group,
                City = city, Area = area,
                LastDonationDate = daysSinceDonation == null ? null : DateTime.Today.AddDays(-daysSinceDonation.Value),
                IsDonor = true
            };
            u.PasswordHash = hasher.HashPassword(u, DemoPassword);
            return u;
        }

        var users = new[]
        {
            Make("Aarav Patel",  "aarav@demo.com",  "9876500001", "O+",  "Rajkot",    "Kalawad Road", null),
            Make("Priya Shah",   "priya@demo.com",  "9876500002", "A+",  "Rajkot",    "University Road", 120),
            Make("Rohan Mehta",  "rohan@demo.com",  "9876500003", "B+",  "Ahmedabad", "Navrangpura", 30),
            Make("Neha Joshi",   "neha@demo.com",   "9876500004", "O-",  "Rajkot",    "Raiya Road", 200),
            Make("Karan Desai",  "karan@demo.com",  "9876500005", "AB+", "Surat",     "Adajan", null),
        };
        db.Users.AddRange(users);
        db.SaveChanges();

        db.Requests.Add(new BloodRequest
        {
            PatientName = "Ramesh Kumar", BloodGroup = "A+", Units = 2, City = "Rajkot",
            Hospital = "Civil Hospital", ContactPhone = "9876500009", Urgency = Urgency.Critical,
            Notes = "Surgery scheduled tomorrow morning.", RequesterId = users[2].Id
        });
        db.Requests.Add(new BloodRequest
        {
            PatientName = "Sita Verma", BloodGroup = "O+", Units = 1, City = "Ahmedabad",
            Hospital = "Sterling Hospital", ContactPhone = "9876500010", Urgency = Urgency.Urgent,
            RequesterId = users[0].Id
        });
        db.SaveChanges();
    }
}
