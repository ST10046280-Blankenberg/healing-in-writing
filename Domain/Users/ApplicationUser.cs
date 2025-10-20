using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Users;

/// <summary>
/// Extends IdentityUser with application-specific user properties.
/// Integrates with ASP.NET Core Identity for authentication and authorisation.
/// </summary>
public class ApplicationUser : IdentityUser
{
    [Required]
    [StringLength(80)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(80)]
    public string LastName { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    // public UserProfile UserProfile { get; set; }
    // public ICollection<Story> Stories { get; set; }
    // public ICollection<Event> Events { get; set; }
    // public ICollection<Donation> Donations { get; set; }
    // public Volunteer Volunteer { get; set; }
}
