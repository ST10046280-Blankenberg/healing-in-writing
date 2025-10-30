using HealingInWriting.Domain.Events;

namespace HealingInWriting.Interfaces.Services;

/// <summary>
/// Defines event registration operations with business rule enforcement.
/// </summary>
public interface IRegistrationService
{
    /// <summary>
    /// Registers an authenticated user for an event.
    /// Validates capacity, event status, and prevents duplicate registrations.
    /// </summary>
    /// <param name="eventId">Event to register for</param>
    /// <param name="userId">User registering</param>
    /// <param name="isAdminOverride">Admin bypass for capacity and status checks</param>
    /// <returns>Result with success status and message</returns>
    Task<RegistrationResult> RegisterUserAsync(int eventId, int userId, bool isAdminOverride = false);

    /// <summary>
    /// Registers a guest for an event using email.
    /// Validates capacity, event status, and prevents duplicate guest registrations.
    /// </summary>
    /// <param name="eventId">Event to register for</param>
    /// <param name="guestName">Guest's name</param>
    /// <param name="guestEmail">Guest's email (required)</param>
    /// <param name="guestPhone">Guest's phone (optional)</param>
    /// <param name="isAdminOverride">Admin bypass for capacity and status checks</param>
    /// <returns>Result with success status and message</returns>
    Task<RegistrationResult> RegisterGuestAsync(int eventId, string guestName, string guestEmail,
        string? guestPhone = null, bool isAdminOverride = false);

    /// <summary>
    /// Cancels a registration.
    /// Validates 48-hour cancellation policy unless admin override.
    /// </summary>
    /// <param name="registrationId">Registration to cancel</param>
    /// <param name="requestingUserId">User requesting cancellation (for ownership check)</param>
    /// <param name="isAdminOverride">Admin bypass for 48-hour rule</param>
    /// <returns>Result with success status and message</returns>
    Task<RegistrationResult> CancelRegistrationAsync(int registrationId, int? requestingUserId = null,
        bool isAdminOverride = false);

    /// <summary>
    /// Gets all registrations for an event.
    /// </summary>
    Task<IEnumerable<Registration>> GetEventRegistrationsAsync(int eventId);

    /// <summary>
    /// Gets all registrations for a user.
    /// </summary>
    Task<IEnumerable<Registration>> GetUserRegistrationsAsync(int userId);

    /// <summary>
    /// Checks if a user is registered for an event.
    /// </summary>
    Task<bool> IsUserRegisteredAsync(int eventId, int userId);

    /// <summary>
    /// Checks if a guest email is registered for an event.
    /// </summary>
    Task<bool> IsGuestRegisteredAsync(int eventId, string guestEmail);

    /// <summary>
    /// Gets registration count and capacity info for an event.
    /// </summary>
    Task<RegistrationCapacityInfo> GetCapacityInfoAsync(int eventId);
}

/// <summary>
/// Result of a registration operation.
/// </summary>
public class RegistrationResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? RegistrationId { get; set; }
}

/// <summary>
/// Registration capacity information for an event.
/// </summary>
public class RegistrationCapacityInfo
{
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public int AvailableSpots => Capacity - RegisteredCount;
    public bool IsFull => RegisteredCount >= Capacity;
    public bool CanRegister { get; set; }
    public string StatusMessage { get; set; } = string.Empty;
}
