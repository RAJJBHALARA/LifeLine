using System.ComponentModel.DataAnnotations;

namespace LifeLine.ViewModels;

public class RegisterVm
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s.''-]+$", ErrorMessage = "Name can only contain letters, spaces, apostrophes, and hyphens.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [StringLength(200)]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character (@$!%*?&#).")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    public string ConfirmPassword { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number.")]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Blood group is required.")]
    [Display(Name = "Blood group")]
    public string BloodGroup { get; set; } = "";

    [Required(ErrorMessage = "City is required.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "City must be between 2 and 60 characters.")]
    [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "City can only contain letters, spaces, and hyphens.")]
    public string City { get; set; } = "";

    [StringLength(100, ErrorMessage = "Area must be at most 100 characters.")]
    public string? Area { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Last donation date (leave empty if never)")]
    public DateTime? LastDonationDate { get; set; }

    [Display(Name = "I want to be listed as a donor")]
    public bool IsDonor { get; set; } = true;
}

public class LoginVm
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Password is required.")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}

public class ProfileVm
{
    [Required(ErrorMessage = "Full name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s.''-]+$", ErrorMessage = "Name can only contain letters, spaces, apostrophes, and hyphens.")]
    [Display(Name = "Full name")]
    public string FullName { get; set; } = "";

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number.")]
    [Display(Name = "Phone number")]
    public string Phone { get; set; } = "";

    [Required(ErrorMessage = "Blood group is required.")]
    [Display(Name = "Blood group")]
    public string BloodGroup { get; set; } = "";

    [Required(ErrorMessage = "City is required.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "City must be between 2 and 60 characters.")]
    [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "City can only contain letters, spaces, and hyphens.")]
    public string City { get; set; } = "";

    [StringLength(100, ErrorMessage = "Area must be at most 100 characters.")]
    public string? Area { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Last donation date")]
    public DateTime? LastDonationDate { get; set; }

    [Display(Name = "Listed as a donor")]
    public bool IsDonor { get; set; }
}

public class CreateRequestVm
{
    [Required(ErrorMessage = "Patient name is required.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s.''-]+$", ErrorMessage = "Name can only contain letters, spaces, apostrophes, and hyphens.")]
    [Display(Name = "Patient name")]
    public string PatientName { get; set; } = "";

    [Required(ErrorMessage = "Blood group is required.")]
    [Display(Name = "Blood group needed")]
    public string BloodGroup { get; set; } = "";

    [Range(1, 10, ErrorMessage = "Units must be between 1 and 10.")]
    public int Units { get; set; } = 1;

    [Required(ErrorMessage = "City is required.")]
    [StringLength(60, MinimumLength = 2, ErrorMessage = "City must be between 2 and 60 characters.")]
    [RegularExpression(@"^[a-zA-Z\s\-]+$", ErrorMessage = "City can only contain letters, spaces, and hyphens.")]
    public string City { get; set; } = "";

    [Required(ErrorMessage = "Hospital is required.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Hospital must be between 2 and 150 characters.")]
    public string Hospital { get; set; } = "";

    [Required(ErrorMessage = "Contact phone is required.")]
    [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Enter a valid 10-digit Indian mobile number.")]
    [Display(Name = "Contact phone")]
    public string ContactPhone { get; set; } = "";

    public Urgency Urgency { get; set; } = Urgency.Normal;

    [StringLength(500, ErrorMessage = "Notes must be at most 500 characters.")]
    public string? Notes { get; set; }
}
