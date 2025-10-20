using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Users;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Events;

// TODO: Capture event scheduling details and ownership metadata.
public class Event
{
    #region Metadata
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
    public required string Title { get; set; }

    [StringLength(2000)]
    public required string Description { get; set; }

    [Required]
    public EventType EventType { get; set; }

    [Required]
    public DateTime StartDateTime { get; set; }

    [Required]
    public DateTime EndDateTime { get; set; }

    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }
    #endregion

    #region Navigation Properties
    [Required]
    public required Address Address { get; set; }
    [Required]
    public required UserProfile User { get; set; }

    // TODO: Navigation properties for related entities
    // public Status Status { get; set; }
    // Possible change Status to a an Enum if only a few states exist or if only used by this domain.
    #endregion
}
public enum EventType
{
    Workshop,
    CommunityEvent
}

/* 
 To display Enum As String in ASP.NET MVC Razor View:
@foreach (var type in Enum.GetValues(typeof(EventType)))
{
    <option value="@type">@type</option>
}
 */