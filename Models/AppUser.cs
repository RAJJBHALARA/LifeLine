using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LifeLine.Models;

public class AppUser
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string FullName { get; set; } = "";

    [Required, MaxLength(200)]
    public string Email { get; set; } = "";

    [Required]
    public string PasswordHash { get; set; } = "";

    [Required, MaxLength(15)]
    public string Phone { get; set; } = "";

    [Required, MaxLength(5)]
    public string BloodGroup { get; set; } = "";

    [Required, MaxLength(60)]
    public string City { get; set; } = "";

    [MaxLength(100)]
    public string? Area { get; set; }

    public DateTime? LastDonationDate { get; set; }
    public bool IsDonor { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<BloodRequest> Requests { get; set; } = new();
    public List<DonorResponse> Responses { get; set; } = new();

    /// <summary>90-day cooldown rule: eligible if never donated or last donation was 90+ days ago.</summary>
    [NotMapped]
    public bool IsEligible =>
        IsDonor && (LastDonationDate == null ||
                    (DateTime.Today - LastDonationDate.Value.Date).TotalDays >= BloodHelper.CooldownDays);

    [NotMapped]
    public DateTime? NextEligibleDate => LastDonationDate?.Date.AddDays(BloodHelper.CooldownDays);
}
