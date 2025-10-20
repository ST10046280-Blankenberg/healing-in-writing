using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Events;

// TODO: Capture event scheduling details and ownership metadata.
public class Event
{
    [Key]
    public int EventId { get; set; }        // PK: event_id

    [Required]
    public int UserId { get; set; }         // FK: user_id

    [Required]
    public int AddressId { get; set; }      // FK: address_id

    [Required]
    public int StatusId { get; set; }       // FK: status_id

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [StringLength(2000)]
    public string Description { get; set; }

    [Required]
    public DateTime StartDateTime { get; set; }

    [Required]
    public DateTime EndDateTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    // TODO: Navigation properties for related entities
    // public User User { get; set; }
    // public Address Address { get; set; }
    // public Status Status { get; set; }
}
