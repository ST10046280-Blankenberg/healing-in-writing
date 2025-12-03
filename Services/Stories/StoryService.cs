using System.Globalization;
using HealingInWriting.Data;
using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Admin;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Services.Stories;

/// <summary>
/// Story service for managing story persistence and retrieval.
/// </summary>
public class StoryService : IStoryService
{
    private readonly IStoryRepository _storyRepository;
    private readonly ApplicationDbContext _context;

    public StoryService(IStoryRepository storyRepository, ApplicationDbContext context)
    {
        _storyRepository = storyRepository;
        _context = context;
    }

    public async Task<IReadOnlyCollection<Story>> GetPublishedAsync()
    {
        var stories = await _context.Stories
            .Include(s => s.Author)
                .ThenInclude(a => a.User)
            .Include(s => s.Tags)
            .Where(s => s.Status == StoryStatus.Published)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return stories;
    }

    public async Task<Story> SubmitStoryAsync(string userId, string title, string content, string tags, bool isAnonymous, string? coverImageUrl = null)
    {
        // Find or create user profile
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            // Auto-create a basic profile for the user
            userProfile = new UserProfile
            {
                UserId = userId,
                Bio = "",
                City = ""
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
        }

        // Sanitise content
        var sanitizer = new Ganss.Xss.HtmlSanitizer();
        var sanitizedContent = sanitizer.Sanitize(content);

        // Generate summary from content (first 500 chars)
        var plainText = System.Text.RegularExpressions.Regex.Replace(sanitizedContent, "<.*?>", string.Empty);
        var summary = plainText.Length > 500 ? plainText.Substring(0, 497) + "..." : plainText;

        // Create story
        var story = new Story
        {
            Title = title.Trim(),
            Content = sanitizedContent,
            Summary = summary,
            Category = StoryCategory.SurvivorStory,
            IsAnonymous = isAnonymous,
            UserId = userProfile.ProfileId,
            CreatedAt = DateTime.UtcNow,
            Status = StoryStatus.Submitted,
            Tags = new List<Tag>(),
            CoverImageUrl = coverImageUrl
        };

        _context.Stories.Add(story);
        await _context.SaveChangesAsync();

        return story;
    }

    public async Task<Story> SaveDraftAsync(string userId, string title, string content, string tags, bool isAnonymous, string? coverImageUrl = null)
    {
        // Find or create user profile
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            // Auto-create a basic profile for the user
            userProfile = new UserProfile
            {
                UserId = userId,
                Bio = "",
                City = ""
            };
            _context.UserProfiles.Add(userProfile);
            await _context.SaveChangesAsync();
        }

        // Sanitise content
        var safeContent = content ?? "";
        var sanitizer = new Ganss.Xss.HtmlSanitizer();
        var sanitizedContent = sanitizer.Sanitize(safeContent);

        // Generate summary from content (first 500 chars)
        var plainText = System.Text.RegularExpressions.Regex.Replace(sanitizedContent, "<.*?>", string.Empty);
        var summary = plainText.Length > 500 ? plainText.Substring(0, 497) + "..." : plainText;

        // Check if user already has an existing draft (most recent one)
        var existingDraft = await _context.Stories
            .Where(s => s.UserId == userProfile.ProfileId && s.Status == StoryStatus.Draft)
            .OrderByDescending(s => s.UpdatedAt ?? s.CreatedAt)
            .FirstOrDefaultAsync();

        if (existingDraft != null)
        {
            // Update existing draft - only update title if a title is provided
            if (!string.IsNullOrWhiteSpace(title))
            {
                existingDraft.Title = title.Trim();
            }
            existingDraft.Content = sanitizedContent;
            existingDraft.Summary = summary;
            existingDraft.IsAnonymous = isAnonymous;
            existingDraft.UpdatedAt = DateTime.UtcNow;

            // Only update cover image if a new one is provided
            if (!string.IsNullOrEmpty(coverImageUrl))
            {
                existingDraft.CoverImageUrl = coverImageUrl;
            }

            await _context.SaveChangesAsync();
            return existingDraft;
        }
        else
        {
            // Create new draft story
            var safeTitle = string.IsNullOrWhiteSpace(title) ? "Untitled Draft" : title.Trim();
            var story = new Story
            {
                Title = safeTitle,
                Content = sanitizedContent,
                Summary = summary,
                Category = StoryCategory.SurvivorStory,
                IsAnonymous = isAnonymous,
                UserId = userProfile.ProfileId,
                CreatedAt = DateTime.UtcNow,
                Status = StoryStatus.Draft,
                Tags = new List<Tag>(),
                CoverImageUrl = coverImageUrl
            };

            _context.Stories.Add(story);
            await _context.SaveChangesAsync();

            return story;
        }
    }

    public async Task<int> GetUserStoryCountAsync(string userId)
    {
        // Find user profile first
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            return 0;
        }

        return await _context.Stories
            .Where(s => s.UserId == userProfile.ProfileId)
            .CountAsync();
    }

    public async Task<IReadOnlyCollection<Story>> GetUserStoriesAsync(string userId)
    {
        // Find user profile first
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            return new List<Story>();
        }

        var stories = await _context.Stories
            .Include(s => s.Tags)
            .Include(s => s.Author)
            .Where(s => s.UserId == userProfile.ProfileId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return stories;
    }

    public async Task<Story?> GetStoryByIdAsync(int storyId)
    {
        var story = await _context.Stories
            .Include(s => s.Tags)
            .Include(s => s.Author)
                .ThenInclude(author => author.User)
            .FirstOrDefaultAsync(s => s.StoryId == storyId);

        return story;
    }

    public async Task<IReadOnlyCollection<Story>> GetAllStoriesForAdminAsync()
    {
        var stories = await _context.Stories
            .AsNoTracking()
            .Include(s => s.Author)
                .ThenInclude(author => author.User)
            .Include(s => s.Tags)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return stories;
    }

    public async Task<bool> UpdateStoryStatusAsync(int storyId, StoryStatus newStatus, string updatedBy)
    {
        var story = await _context.Stories.FirstOrDefaultAsync(s => s.StoryId == storyId);
        if (story == null)
        {
            return false;
        }

        if (story.Status == newStatus)
        {
            return true;
        }

        story.Status = newStatus;
        story.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IReadOnlyCollection<Story>> GetFilteredUserStoriesAsync(
        string userId,
        string? searchText,
        string? selectedDate,
        string? selectedSort,
        StoryCategory? selectedCategory)
    {
        // Find user profile first
        var userProfile = await _context.UserProfiles
            .FirstOrDefaultAsync(up => up.UserId == userId);

        if (userProfile == null)
        {
            return new List<Story>();
        }

        // Delegate filtering to repository
        var stories = await _storyRepository.GetFilteredUserStoriesAsync(
            userProfile.ProfileId,
            searchText,
            selectedDate,
            selectedSort,
            selectedCategory);

        return stories is IReadOnlyCollection<Story> readOnly ? readOnly : stories.ToList();
    }

    public async Task<(IEnumerable<Story> Stories, int TotalCount)> GetFilteredStoriesForAdminAsync(
        string? searchTerm,
        StoryStatus? status,
        string? dateRange,
        string? tag,
        string sortOrder,
        int page,
        int pageSize)
    {
        var query = _context.Stories
            .Include(s => s.Author)
                .ThenInclude(a => a.User)
            .Include(s => s.Tags)
            .AsQueryable();

        // Apply search filter
        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            query = query.Where(s =>
                s.Title.ToLower().Contains(term) ||
                (s.Summary != null && s.Summary.ToLower().Contains(term)) ||
                s.Content.ToLower().Contains(term));
        }

        // Apply status filter
        if (status.HasValue)
        {
            query = query.Where(s => s.Status == status.Value);
        }

        // Apply tag filter
        if (!string.IsNullOrWhiteSpace(tag))
        {
            query = query.Where(s => s.Tags.Any(t =>
                t.Name.Equals(tag, StringComparison.OrdinalIgnoreCase)));
        }

        // Apply date range filter
        var dateThreshold = ParseDateRange(dateRange);
        if (dateThreshold.HasValue)
        {
            query = query.Where(s => s.CreatedAt >= dateThreshold.Value);
        }

        // Apply sorting
        query = sortOrder?.ToLower() switch
        {
            "oldest" => query.OrderBy(s => s.CreatedAt),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var stories = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (stories, totalCount);
    }

    /// <summary>
    /// Resolves the display name for a story author, handling anonymous stories and missing data gracefully.
    /// Returns "Anonymous" for anonymous stories, the author's full name if available,
    /// email as fallback, or user ID as last resort.
    /// </summary>
    public string ResolveAuthorName(Story story)
    {
        if (story.IsAnonymous)
        {
            return "Anonymous";
        }

        var firstName = story.Author?.User?.FirstName;
        var lastName = story.Author?.User?.LastName;

        var nameParts = new[] { firstName, lastName }
            .Where(part => !string.IsNullOrWhiteSpace(part))
            .ToArray();

        if (nameParts.Any())
        {
            return string.Join(" ", nameParts);
        }

        if (!string.IsNullOrWhiteSpace(story.Author?.User?.Email))
        {
            return story.Author!.User!.Email!;
        }

        return story.Author?.UserId ?? "Unknown";
    }

    /// <summary>
    /// Maps a story status to its corresponding CSS badge class for display purposes.
    /// This ensures consistent visual representation of story statuses across the application.
    /// </summary>
    public string GetStatusBadgeClass(StoryStatus status)
    {
        return status switch
        {
            StoryStatus.Submitted => "manage-stories__status-badge--submitted",
            StoryStatus.Published => "manage-stories__status-badge--published",
            StoryStatus.Rejected => "manage-stories__status-badge--rejected",
            StoryStatus.Draft => "manage-stories__status-badge--pending",
            StoryStatus.Archived => "manage-stories__status-badge--approved",
            _ => "manage-stories__status-badge"
        };
    }

    /// <summary>
    /// Calculates the count of stories grouped by status from a collection of stories.
    /// Useful for displaying statistics and metrics in admin dashboards.
    /// </summary>
    public Dictionary<StoryStatus, int> CalculateStatusCounts(IEnumerable<Story> stories)
    {
        return stories
            .GroupBy(story => story.Status)
            .ToDictionary(group => group.Key, group => group.Count());
    }

    /// <summary>
    /// Builds a list of status filter options for admin dropdowns with the selected value highlighted.
    /// Creates options for all available story statuses with proper formatting.
    /// </summary>
    public IReadOnlyCollection<AdminSelectOption> BuildStatusOptions(string? selectedStatus)
    {
        var options = new List<AdminSelectOption>
        {
            new("", "Any Status", string.IsNullOrWhiteSpace(selectedStatus))
        };

        foreach (var status in Enum.GetValues(typeof(StoryStatus)).Cast<StoryStatus>())
        {
            options.Add(new AdminSelectOption(
                status.ToString(),
                CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status.ToString().Replace("_", " ").ToLowerInvariant()),
                string.Equals(selectedStatus, status.ToString(), StringComparison.OrdinalIgnoreCase)));
        }

        return options;
    }

    /// <summary>
    /// Builds a list of date range filter options for admin dropdowns with the selected value highlighted.
    /// Provides common date range filters such as last 7, 30, 90 days, and this year.
    /// </summary>
    public IReadOnlyCollection<AdminSelectOption> BuildDateOptions(string? selectedDate)
    {
        var options = new List<AdminSelectOption>
        {
            new("any", "Any Date", string.IsNullOrWhiteSpace(selectedDate) || selectedDate.Equals("any", StringComparison.OrdinalIgnoreCase)),
            new("last7", "Last 7 days", string.Equals(selectedDate, "last7", StringComparison.OrdinalIgnoreCase)),
            new("last30", "Last 30 days", string.Equals(selectedDate, "last30", StringComparison.OrdinalIgnoreCase)),
            new("last90", "Last 90 days", string.Equals(selectedDate, "last90", StringComparison.OrdinalIgnoreCase)),
            new("this-year", "This year", string.Equals(selectedDate, "this-year", StringComparison.OrdinalIgnoreCase))
        };

        return options;
    }

    /// <summary>
    /// Builds a list of tag filter options for admin dropdowns based on available tags in stories.
    /// Extracts unique tags from the provided stories and includes the selected tag even if not in the dataset.
    /// </summary>
    public IReadOnlyCollection<AdminSelectOption> BuildTagOptions(string? selectedTag, IEnumerable<Story> stories)
    {
        var tags = stories
            .SelectMany(story => story.Tags)
            .Select(tag => tag.Name)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

        var options = new List<AdminSelectOption>
        {
            new("", "Any Tag", string.IsNullOrWhiteSpace(selectedTag))
        };

        foreach (var tag in tags)
        {
            options.Add(new AdminSelectOption(tag, tag, string.Equals(tag, selectedTag, StringComparison.OrdinalIgnoreCase)));
        }

        // Preserve manually entered tags even if not present in dataset
        if (!string.IsNullOrWhiteSpace(selectedTag)
            && options.All(option => !string.Equals(option.Value, selectedTag, StringComparison.OrdinalIgnoreCase)))
        {
            options.Add(new AdminSelectOption(selectedTag, selectedTag, true));
        }

        return options;
    }

    /// <summary>
    /// Builds a list of sort order options for admin dropdowns with the selected value highlighted.
    /// Provides options for sorting by newest or oldest stories.
    /// </summary>
    public IReadOnlyCollection<AdminSelectOption> BuildSortOptions(string? selectedSort)
    {
        var normalizedSort = string.IsNullOrWhiteSpace(selectedSort)
            ? "newest"
            : selectedSort.ToLowerInvariant();

        return new List<AdminSelectOption>
        {
            new("newest", "Newest", normalizedSort == "newest"),
            new("oldest", "Oldest", normalizedSort == "oldest")
        };
    }

    private static DateTime? ParseDateRange(string? dateRange)
    {
        if (string.IsNullOrWhiteSpace(dateRange) ||
            dateRange.Equals("any", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        var now = DateTime.UtcNow;
        return dateRange.ToLowerInvariant() switch
        {
            "last7" => now.AddDays(-7),
            "last30" => now.AddDays(-30),
            "last90" => now.AddDays(-90),
            "this-year" => new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            _ => null
        };
    }
}
