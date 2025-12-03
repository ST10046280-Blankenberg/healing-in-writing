using System.Collections.Generic;
using HealingInWriting.Domain.Events;
using HealingInWriting.Models.Events;
using HealingInWriting.Models.Shared;

namespace HealingInWriting.Interfaces.Services;

// TODO: Define event management operations consumed by controllers.
public interface IEventService
{
    // TODO: Add scheduling, detail retrieval, and RSVP coordination methods.
    public double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2);

    Task<int> CreateEventAsync(CreateEventViewModel model, string userId);

    Task UpdateEventAsync(CreateEventViewModel model, string userId);

    Task<Event?> GetEventByIdAsync(int eventId);

    Task<IReadOnlyCollection<Event>> GetAllEventsAsync();

    Task<bool> DeleteEventAsync(int eventId);

    Task<bool> UpdateEventStatusAsync(int eventId, EventStatus newStatus);

    Task<int> GetUserUpcomingEventsCountAsync(string userId);

    /// <summary>
    /// Gets all event registrations for a specific user.
    /// </summary>
    Task<IReadOnlyCollection<Registration>> GetUserRegistrationsAsync(string userId);
    
    Task<EventsIndexViewModel> GetFilteredEventsAsync(string? searchText, EventType? selectedEventType, DateTime? startDate, DateTime? endDate);

    /// <summary>
    /// Retrieves filtered and paginated events for admin management
    /// </summary>
    Task<(IEnumerable<Event> Events, int TotalCount)> GetFilteredEventsForAdminAsync(
        string? searchTerm,
        EventStatus? status,
        string? dateRange,
        string sortOrder,
        int page,
        int pageSize);

    /// <summary>
    /// Seeds the database with sample event data and registrations for testing purposes.
    /// Should only be used in development or staging environments.
    /// </summary>
    /// <param name="userId">The user ID to associate as the event creator</param>
    /// <param name="eventCount">Number of events to seed (default: 10)</param>
    /// <returns>Success or error message</returns>
    Task<string?> SeedEventsAsync(string userId, int eventCount = 10);

    /// <summary>
    /// Builds a list of status filter options for admin dropdowns with the selected value highlighted.
    /// </summary>
    List<AdminDropdownOption> BuildStatusOptions(string? selectedStatus);

    /// <summary>
    /// Builds a list of date range filter options for admin dropdowns with the selected value highlighted.
    /// </summary>
    List<AdminDropdownOption> BuildDateOptions(string? selectedRange);

    /// <summary>
    /// Builds a list of sort order options for admin dropdowns with the selected value highlighted.
    /// </summary>
    List<AdminDropdownOption> BuildSortOptions(string? selectedSort);
}
