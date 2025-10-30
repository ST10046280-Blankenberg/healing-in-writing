using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Models.Events;

/// <summary>
/// View model for guest registration form.
/// Used when unauthenticated users register for events.
/// </summary>
public class GuestRegistrationViewModel
{
    public int EventId { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
    [Display(Name = "Full Name")]
    public string GuestName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters")]
    [Display(Name = "Email Address")]
    public string GuestEmail { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number")]
    [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
    [Display(Name = "Phone Number (Optional)")]
    public string? GuestPhone { get; set; }
}
