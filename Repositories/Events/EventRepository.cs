using HealingInWriting.Domain.Events;
using HealingInWriting.Interfaces.Repository;

namespace HealingInWriting.Repositories.Events;

// TODO: Implement event repository using EF Core or other data source abstractions.
public class EventRepository : IEventRepository
{
    // TODO: Inject DbContext and translate event aggregates to persistence models.
    public Task AddAsync(Event @event)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int eventId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Event>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Event?> GetByIdAsync(int eventId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Event @event)
    {
        throw new NotImplementedException();
    }
}
