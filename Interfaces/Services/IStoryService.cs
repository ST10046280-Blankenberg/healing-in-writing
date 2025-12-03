using HealingInWriting.Domain.Stories;
using HealingInWriting.Models.Admin;

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
    Task<Story> SubmitStoryAsync(string userId, string title, string content, string tags, bool isAnonymous, string? coverImageUrl = null);

    /// <summary>
    /// Saves a story as a draft for the authenticated user.
    /// </summary>
    Task<Story> SaveDraftAsync(string userId, string title, string content, string tags, bool isAnonymous, string? coverImageUrl = null);

    /// <summary>
    /// Gets the count of stories for a specific user.
    /// </summary>
    Task<int> GetUserStoryCountAsync(string userId);

    /// <summary>
    /// Gets all stories for a specific user, grouped by status.
    /// </summary>
    Task<IReadOnlyCollection<Story>> GetUserStoriesAsync(string userId);

    /// <summary>
    /// Retrieves a single story by identifier, including related data needed for detail views.
    /// </summary>
    Task<Story?> GetStoryByIdAsync(int storyId);

    /// <summary>
    /// Retrieves all stories for administrative management views.
    /// </summary>
    Task<IReadOnlyCollection<Story>> GetAllStoriesForAdminAsync();

    /// <summary>
    /// Updates the status of a story for administrative workflows.
    /// </summary>
    Task<bool> UpdateStoryStatusAsync(int storyId, StoryStatus newStatus, string updatedBy);

    /// <summary>
    /// Gets filtered stories for a specific user based on search text, date, sort order, and category.
    /// </summary>
    Task<IReadOnlyCollection<Story>> GetFilteredUserStoriesAsync(
        string userId,
        string? searchText,
        string? selectedDate,
        string? selectedSort,
        StoryCategory? selectedCategory);

    /// <summary>
    /// Retrieves filtered and paginated stories for admin management with efficient database querying.
    /// </summary>
    Task<(IEnumerable<Story> Stories, int TotalCount)> GetFilteredStoriesForAdminAsync(
        string? searchTerm,
        StoryStatus? status,
        string? dateRange,
        string? tag,
        string sortOrder,
        int page,
        int pageSize);

    /// <summary>
    /// Resolves the display name for a story author, handling anonymous stories and missing data gracefully.
    /// Returns "Anonymous" for anonymous stories, the author's full name if available, email as fallback, or user ID.
    /// </summary>
    string ResolveAuthorName(Story story);

    /// <summary>
    /// Maps a story status to its corresponding CSS badge class for display purposes.
    /// </summary>
    string GetStatusBadgeClass(StoryStatus status);

    /// <summary>
    /// Calculates the count of stories grouped by status from a collection of stories.
    /// </summary>
    Dictionary<StoryStatus, int> CalculateStatusCounts(IEnumerable<Story> stories);

    /// <summary>
    /// Builds a list of status filter options for admin dropdowns with the selected value highlighted.
    /// </summary>
    IReadOnlyCollection<AdminSelectOption> BuildStatusOptions(string? selectedStatus);

    /// <summary>
    /// Builds a list of date range filter options for admin dropdowns with the selected value highlighted.
    /// </summary>
    IReadOnlyCollection<AdminSelectOption> BuildDateOptions(string? selectedDate);

    /// <summary>
    /// Builds a list of tag filter options for admin dropdowns based on available tags in stories.
    /// Includes the selected tag even if it doesn't exist in the current dataset.
    /// </summary>
    IReadOnlyCollection<AdminSelectOption> BuildTagOptions(string? selectedTag, IEnumerable<Story> stories);

    /// <summary>
    /// Builds a list of sort order options for admin dropdowns with the selected value highlighted.
    /// </summary>
    IReadOnlyCollection<AdminSelectOption> BuildSortOptions(string? selectedSort);
}
