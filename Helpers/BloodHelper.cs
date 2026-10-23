using System.Security.Claims;

namespace LifeLine.Helpers;

public static class BloodHelper
{
    /// <summary>Minimum days between two donations (the "cooldown rule").</summary>
    public const int CooldownDays = 90;

    public static readonly string[] Groups = { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" };

    // Recipient group -> donor groups that can give to them.
    private static readonly Dictionary<string, string[]> Compatible = new()
    {
        ["A+"]  = new[] { "A+", "A-", "O+", "O-" },
        ["A-"]  = new[] { "A-", "O-" },
        ["B+"]  = new[] { "B+", "B-", "O+", "O-" },
        ["B-"]  = new[] { "B-", "O-" },
        ["AB+"] = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "O+", "O-" },
        ["AB-"] = new[] { "A-", "B-", "AB-", "O-" },
        ["O+"]  = new[] { "O+", "O-" },
        ["O-"]  = new[] { "O-" },
    };

    /// <summary>Returns list of donor blood groups that can give blood to this recipient group.</summary>
    public static List<string> DonorsFor(string recipientGroup) =>
        Compatible.TryGetValue(recipientGroup, out var list) ? list.ToList() : new List<string> { recipientGroup };

    /// <summary>Returns list of recipient blood groups that can receive blood from this donor group.</summary>
    public static List<string> RecipientsFor(string donorGroup) =>
        Compatible.Where(kvp => kvp.Value.Contains(donorGroup))
                  .Select(kvp => kvp.Key)
                  .ToList();

    /// <summary>Checks whether donorGroup is biologically compatible with recipientGroup.</summary>
    public static bool CanDonateTo(string donorGroup, string recipientGroup) =>
        DonorsFor(recipientGroup).Contains(donorGroup);

    public static int GetId(this ClaimsPrincipal p) =>
        int.Parse(p.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
