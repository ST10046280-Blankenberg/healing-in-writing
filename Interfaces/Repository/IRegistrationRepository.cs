using HealingInWriting.Domain.Events;

namespace HealingInWriting.Interfaces.Repository;

/// <summary>
/// Specifies registration persistence operations, separating data access concerns from domain logic.
/// </summary>
public interface IRegistrationRepository
{
    /// <summary>
    /// Retrieves all registrations for a specific event.
    /// </summary>
    Task<IEnumerable<Registration>> GetByEventIdAsync(int eventId);

    /// <summary>
    /// Retrieves all registrations for a specific user.
    /// </summary>
    Task<IEnumerable<Registration>> GetByUserIdAsync(int userId);

    /// <summary>
    /// Retrieves a specific registration by its unique identifier.
    /// </summary>
    Task<Registration?> GetByIdAsync(int registrationId);

    /// <summary>
    /// Checks if a user is already registered for an event.
    /// </summary>
    Task<Registration?> GetByEventAndUserAsync(int eventId, int userId);

    /// <summary>
    /// Checks if a guest email is already registered for an event.
    /// </summary>
    Task<Registration?> GetByEventAndGuestEmailAsync(int eventId, string guestEmail);

    /// <summary>
    /// Gets the count of registrations for an event.
    /// </summary>
    Task<int> GetRegistrationCountAsync(int eventId);

    /// <summary>
    /// Adds a new registration to the database.
    /// </summary>
    Task AddAsync(Registration registration);

    /// <summary>
    /// Deletes a registration from the database.
    /// </summary>
    Task DeleteAsync(int registrationId);

    /// <summary>
    /// Gets the count of registrations from a specific IP address for an event within a time window.
    /// Used for rate limiting guest registrations.
    /// </summary>
    Task<int> GetRegistrationCountByIpAsync(int eventId, string ipAddress, DateTime since);
    Task<IEnumerable<Registration>> GetFilteredUserRegistrationsAsync(int userId, string? searchText, EventType? selectedEventType, DateTime? startDate, DateTime? endDate);
}
