using HealingInWriting.Domain.Stories;

namespace HealingInWriting.Interfaces.Services;

public interface IStoryService
{
    /// <summary>
    /// Retrieves the set of stories ready to be displayed publicly.
    /// </summary>
    Task<IReadOnlyCollection<Story>> GetPublishedAsync();
}
