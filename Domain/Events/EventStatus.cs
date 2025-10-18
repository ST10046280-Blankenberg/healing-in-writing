namespace HealingInWriting.Domain.Events;

/// <summary>
/// Defines the lifecycle status of an event.
/// </summary>
public enum EventStatus
{
    /// <summary>
    /// Event is being drafted and not yet published
    /// </summary>
    Draft,

    /// <summary>
    /// Event is published and open for registration
    /// </summary>
    Published,

    /// <summary>
    /// Event registration is closed but event hasn't occurred yet
    /// </summary>
    RegistrationClosed,

    /// <summary>
    /// Event is currently in progress
    /// </summary>
    InProgress,

    /// <summary>
    /// Event has been completed
    /// </summary>
    Completed,

    /// <summary>
    /// Event has been cancelled
    /// </summary>
    Cancelled
}
