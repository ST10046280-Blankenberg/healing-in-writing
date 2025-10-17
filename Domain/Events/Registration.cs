using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Events;

// TODO: Represent an attendee registration value object protecting invariants.
public class Registration
{
    [Key]
    public int RegistrationId { get; set; }         // PK: registration_id

    [Required]
    public int EventId { get; set; }                // FK: event_id

    [Required]
    public int UserId { get; set; }                 // FK: user_id

    [Required]
    public DateTime RegistrationDate { get; set; }  // registration_date

    // TODO: Navigation properties for related entities
    // public Event Event { get; set; }
    // public User User { get; set; }
}
