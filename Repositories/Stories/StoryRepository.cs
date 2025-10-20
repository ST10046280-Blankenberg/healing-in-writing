using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Repository;

namespace HealingInWriting.Repositories.Stories;

// TODO: Implement story persistence using the configured data access technology.
public class StoryRepository : IStoryRepository
{
    // TODO: Inject DbContext and map story aggregates to database entities.
    public Task AddAsync(Story story)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(int storyId)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Story>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Story?> GetByIdAsync(int storyId)
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(Story story)
    {
        throw new NotImplementedException();
    }
}
