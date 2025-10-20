using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealingInWriting.Domain.Volunteers;

/// <summary>
/// Represents the assignment of a volunteer to a specific event.
/// Links volunteers to events with role and note tracking.
/// </summary>
public class VolunteerAssignment
{
    [Key]
    public int AssignmentId { get; set; }

    [Required]
    public int VolunteerId { get; set; }

    [Required]
    public int EventId { get; set; }

    public string Roles { get; set; }

    public string Notes { get; set; }

    // Navigation properties
    [ForeignKey(nameof(VolunteerId))]
    public Volunteer Volunteer { get; set; }

    [ForeignKey(nameof(EventId))]
    public Events.Event Event { get; set; }
}
