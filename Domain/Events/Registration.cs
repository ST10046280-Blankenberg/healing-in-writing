using System.ComponentModel.DataAnnotations;
using HealingInWriting.Domain.Users;

namespace HealingInWriting.Domain.Events;

/// <summary>
/// Represents an attendee registration for an event.
/// Supports both authenticated users and guest registrations.
/// </summary>
public class Registration
{
    [Key]
    public int RegistrationId { get; set; }

    [Required]
    public int EventId { get; set; }

    /// <summary>
    /// User ID for authenticated registrations. Null for guest registrations.
    /// </summary>
    public int? UserId { get; set; }

    [Required]
    public DateTime RegistrationDate { get; set; }

    /// <summary>
    /// Guest name for unauthenticated registrations. Null for authenticated users.
    /// </summary>
    [StringLength(100)]
    public string? GuestName { get; set; }

    /// <summary>
    /// Guest email for unauthenticated registrations. Required for guest registrations.
    /// </summary>
    [StringLength(256)]
    [EmailAddress]
    public string? GuestEmail { get; set; }

    /// <summary>
    /// Guest phone number for unauthenticated registrations. Optional.
    /// </summary>
    [StringLength(20)]
    public string? GuestPhone { get; set; }

    /// <summary>
    /// Indicates whether registration was created by admin (bypassing capacity checks).
    /// </summary>
    public bool IsAdminOverride { get; set; }

    #region Navigation Properties

    /// <summary>
    /// Navigation property to the event.
    /// </summary>
    [Required]
    public Event Event { get; set; } = null!;

    /// <summary>
    /// Navigation property to the user. Null for guest registrations.
    /// </summary>
    public UserProfile? User { get; set; }

    #endregion
}
