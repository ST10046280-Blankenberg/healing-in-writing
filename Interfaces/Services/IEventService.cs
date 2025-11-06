using System.Collections.Generic;
using HealingInWriting.Domain.Events;
using HealingInWriting.Models.Events;

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
}
