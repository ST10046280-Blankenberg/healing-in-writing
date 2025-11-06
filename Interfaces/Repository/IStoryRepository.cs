using HealingInWriting.Domain.Stories;

namespace HealingInWriting.Interfaces.Repository
{
    /// <summary>
    /// Defines story persistence operations abstracting the data source.
    /// </summary>
    public interface IStoryRepository
    {
        /// <summary>
        /// Retrieves all stories in the catalogue.
        /// </summary>
        Task<IEnumerable<Story>> GetAllAsync();

        /// <summary>
        /// Retrieves a single story by its unique identifier.
        /// </summary>
        Task<Story?> GetByIdAsync(int storyId);

        /// <summary>
        /// Adds a new story to the catalogue.
        /// </summary>
        Task AddAsync(Story story);

        /// <summary>
        /// Updates an existing story in the catalogue.
        /// </summary>
        Task UpdateAsync(Story story);

        /// <summary>
        /// Deletes a story from the catalogue by its unique identifier.
        /// </summary>
        Task DeleteAsync(int storyId);
        Task<IEnumerable<Story>> GetFilteredUserStoriesAsync(int profileId, string? searchText, string? selectedDate, string? selectedSort, StoryCategory? selectedCategory);
    }
}
