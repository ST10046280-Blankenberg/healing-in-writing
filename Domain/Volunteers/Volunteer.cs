using HealingInWriting.Domain.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealingInWriting.Domain.Volunteers;

/// <summary>
/// Represents a volunteer and their engagement details.
/// </summary>
public class Volunteer
{
    [Key]
    public int VolunteerId { get; set; }

    /// <summary>
    /// Foreign key to ApplicationUser (Identity PK is string).
    /// </summary>
    [Required]
    public string UserId { get; set; }

    /// <summary>
    /// Navigation property to the user account.
    /// </summary>
    [ForeignKey(nameof(UserId))]
    public ApplicationUser User { get; set; }

    /// <summary>
    /// Date the volunteer enrolled.
    /// </summary>
    public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indicates if the volunteer is currently active.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Optional: Volunteer-specific notes or status.
    /// </summary>
    [StringLength(250)]
    public string? Notes { get; set; }

    // Future: Add navigation properties for related entities (e.g., VolunteerHours)
    // public ICollection<VolunteerHour> VolunteerHours { get; set; }

    /// <summary>
    /// Convenience property for role assignment.
    /// </summary>
    [NotMapped]
    public string Role => "Volunteer";
}
