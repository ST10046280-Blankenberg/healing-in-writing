using HealingInWriting.Data;
using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Services;
using Microsoft.EntityFrameworkCore;

namespace HealingInWriting.Services.Stories;

/// <summary>
/// Story service for managing story persistence and retrieval.
/// </summary>
public class StoryService : IStoryService
{
    private readonly ApplicationDbContext _context;

    public StoryService(ApplicationDbContext context)
    {
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
}
