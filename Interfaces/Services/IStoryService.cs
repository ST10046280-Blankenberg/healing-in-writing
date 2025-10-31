using HealingInWriting.Domain.Stories;

namespace HealingInWriting.Interfaces.Services;

public interface IStoryService
{
    /// <summary>
    /// Retrieves the set of stories ready to be displayed publicly.
    /// </summary>
    Task<IReadOnlyCollection<Story>> GetPublishedAsync();

    /// <summary>
    /// Creates a new story submission from the authenticated user.
    /// </summary>
    Task<Story> SubmitStoryAsync(string userId, string title, string content, string tags, bool isAnonymous);

    /// <summary>
    /// Gets the count of stories for a specific user.
    /// </summary>
    Task<int> GetUserStoryCountAsync(string userId);

    /// <summary>
    /// Gets all stories for a specific user, grouped by status.
    /// </summary>
    Task<IReadOnlyCollection<Story>> GetUserStoriesAsync(string userId);
}
