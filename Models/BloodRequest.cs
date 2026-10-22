using System.ComponentModel.DataAnnotations;

namespace LifeLine.Models;

public enum Urgency { Normal = 0, Urgent = 1, Critical = 2 }

public enum RequestStatus { Open = 0, Fulfilled = 1, Closed = 2 }

public class BloodRequest
{
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string PatientName { get; set; } = "";

    [Required, MaxLength(5)]
    public string BloodGroup { get; set; } = "";

    [Range(1, 10)]
    public int Units { get; set; } = 1;

    [Required, MaxLength(60)]
    public string City { get; set; } = "";

    [Required, MaxLength(150)]
    public string Hospital { get; set; } = "";

    [Required, MaxLength(15)]
    public string ContactPhone { get; set; } = "";

    public Urgency Urgency { get; set; } = Urgency.Normal;

    [MaxLength(500)]
    public string? Notes { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public RequestStatus Status { get; set; } = RequestStatus.Open;

    public int RequesterId { get; set; }
    public AppUser Requester { get; set; } = null!;
    public List<DonorResponse> Responses { get; set; } = new();
}
