using System.ComponentModel.DataAnnotations;

namespace LifeLine.Models;

public enum ResponseStatus { Pending = 0, Donated = 1 }

public class DonorResponse
{
    public int Id { get; set; }
    public int RequestId { get; set; }
    public BloodRequest Request { get; set; } = null!;
    public int DonorId { get; set; }
    public AppUser Donor { get; set; } = null!;

    [MaxLength(300)]
    public string? Message { get; set; }

    public DateTime RespondedAt { get; set; } = DateTime.UtcNow;
    public ResponseStatus Status { get; set; } = ResponseStatus.Pending;
}
