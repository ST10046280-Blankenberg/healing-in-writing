namespace HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;

// TODO: Define event management operations consumed by controllers.
public interface IEventService
{
    // TODO: Add scheduling, detail retrieval, and RSVP coordination methods.
    public double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2);

    Task<int> CreateEventAsync(CreateEventViewModel model, string userId);
}
