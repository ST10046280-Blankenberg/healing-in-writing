using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Users;
using System.ComponentModel.DataAnnotations;

namespace HealingInWriting.Domain.Events;

/// <summary>
/// Represents a scheduled event or workshop, including metadata, timing, location, and ownership.
/// </summary>
public class Event
{
    #region Metadata

    /// <summary>
    /// Unique identifier for the event.
    /// </summary>
    [Key]
    public int EventId { get; set; }

    /// <summary>
    /// Foreign key referencing the user who owns or created the event.
    /// </summary>
    [Required]
    public int UserId { get; set; }

    /// <summary>
    /// Foreign key referencing the address where the event is held.
    /// </summary>
    [Required]
    public int AddressId { get; set; }

    /// <summary>
    /// Current lifecycle status of the event (e.g., Draft, Published, Completed).
    /// </summary>
    [Required]
    public EventStatus EventStatus { get; set; }

    /// <summary>
    /// Title of the event.
    /// </summary>
    [Required]
    [StringLength(200)]
    public required string Title { get; set; }

    /// <summary>
    /// Detailed description of the event.
    /// </summary>
    [StringLength(2000)]
    public required string Description { get; set; }

    /// <summary>
    /// Type of event (e.g., Workshop, CommunityEvent).
    /// </summary>
    [Required]
    public EventType EventType { get; set; }

    /// <summary>
    /// Date and time when the event starts.
    /// </summary>
    [Required]
    public DateTime StartDateTime { get; set; }

    /// <summary>
    /// Date and time when the event ends.
    /// </summary>
    [Required]
    public DateTime EndDateTime { get; set; }

    /// <summary>
    /// Maximum number of attendees allowed for the event.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Capacity { get; set; }

    #endregion

    #region Navigation Properties

    /// <summary>
    /// Navigation property for the event's address.
    /// </summary>
    [Required]
    public required Address Address { get; set; }

    /// <summary>
    /// Navigation property for the user profile of the event owner.
    /// </summary>
    [Required]
    public required UserProfile User { get; set; }
    #endregion
}

/// <summary>
/// Enumerates the types of events supported by the platform.
/// </summary>
public enum EventType
{
    /// <summary>
    /// An educational or instructional workshop.
    /// </summary>
    Workshop,

    /// <summary>
    /// A general community event.
    /// </summary>
    CommunityEvent
    /* 
        To display Enum As String in ASP.NET MVC Razor View:
        @foreach (var type in Enum.GetValues(typeof(EventType)))
        {
        <option value="@type">@type</option>
        }
    */
}

/// <summary>
/// Defines the lifecycle status of an event.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// Event is being drafted and not yet published.
    /// </summary>
    Draft,

    /// <summary>
    /// Event is published and open for registration.
    /// </summary>
    Published,

    /// <summary>
    /// Event registration is closed but event hasn't occurred yet.
    /// </summary>
    RegistrationClosed,

    /// <summary>
    /// Event is currently in progress.
    /// </summary>
    InProgress,

    /// <summary>
    /// Event has been completed.
    /// </summary>
    Completed,

    /// <summary>
    /// Event has been cancelled.
    /// </summary>
    Cancelled
}