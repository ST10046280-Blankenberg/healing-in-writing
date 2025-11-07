using HealingInWriting.Data;
using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Repository;
using HealingInWriting.Interfaces.Services;
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
            .Include(s => s.Tags)
            .Where(s => s.Status == StoryStatus.Published)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return stories;
    }

    public async Task<Story> SubmitStoryAsync(string userId, string title, string content, string tags, bool isAnonymous)
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

        // Sanitize content
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
            Tags = new List<Tag>()
        };

        _context.Stories.Add(story);
        await _context.SaveChangesAsync();

        return story;
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
