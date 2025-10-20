using HealingInWriting.Interfaces.Services;

namespace HealingInWriting.Services.Events;

// TODO: Implement event orchestration while delegating data access to repositories.
public class EventService : IEventService
{
    // TODO: Inject event repositories and apply business rules before responding.
    public double CalculateDistanceKm(double lat1, double lon1, double lat2, double lon2)
    {
        throw new NotImplementedException();
    }
}
