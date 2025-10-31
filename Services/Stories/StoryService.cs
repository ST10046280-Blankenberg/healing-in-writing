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
    private static readonly IReadOnlyCollection<Story> PublishedStories = new List<Story>
    {
        new()
        {
            StoryId = 1,
            UserId = 101,
            Title = "Finding Strength in Silence",
            Summary = "A journey through trauma and rediscovery.",
            Content = "This is placeholder text representing the story body.",
            Status = StoryStatus.Published,
            CreatedAt = DateTime.Now.AddDays(-10),
            Tags = new List<Tag>
            {
                new() { TagId = 1, Name = "Healing" },
                new() { TagId = 2, Name = "Survivor" }
            },
            Author = new UserProfile
            {
                ProfileId = 1,
                UserId = "mock-user-1",
                Bio = "Mock author for testing."
            }
        },
        new()
        {
            StoryId = 2,
            UserId = 102,
            Title = "The Letter I Never Sent",
            Summary = "Writing as a form of closure and strength.",
            Content = "Content placeholder for mock story.",
            Status = StoryStatus.Published,
            CreatedAt = DateTime.Now.AddDays(-7),
            Tags = new List<Tag>
            {
                new() { TagId = 3, Name = "Letter" },
                new() { TagId = 4, Name = "Reflection" }
            },
            Author = new UserProfile
            {
                ProfileId = 2,
                UserId = "mock-user-2",
                Bio = "Mock author for testing."
            }
        },
        new()
        {
            StoryId = 3,
            UserId = 103,
            Title = "Hope After Darkness",
            Summary = "Learning to rebuild life after loss.",
            Content = "Content placeholder for mock story.",
            Status = StoryStatus.Published,
            CreatedAt = DateTime.Now.AddDays(-4),
            Tags = new List<Tag>
            {
                new() { TagId = 5, Name = "Hope" },
                new() { TagId = 6, Name = "Recovery" }
            },
            Author = new UserProfile
            {
                ProfileId = 3,
                UserId = "mock-user-3",
                Bio = "Mock author for testing."
            }
        },
        new()
        {
            StoryId = 4,
            UserId = 104,
            Title = "Poems for the Past",
            Summary = "A short collection of reflective verses.",
            Content = "Mock poetic story content.",
            Status = StoryStatus.Published,
            CreatedAt = DateTime.Now.AddDays(-2),
            Tags = new List<Tag>
            {
                new() { TagId = 7, Name = "Poetry" },
                new() { TagId = 8, Name = "Expression" }
            },
            Author = new UserProfile
            {
                ProfileId = 4,
                UserId = "mock-user-4",
                Bio = "Mock author for testing."
            }
        },
        new()
        {
            StoryId = 5,
            UserId = 105,
            Title = "Breaking the Silence",
            Summary = "How sharing helped me heal.",
            Content = "Mock story text for layout testing.",
            Status = StoryStatus.Published,
            CreatedAt = DateTime.Now.AddDays(-1),
            Tags = new List<Tag>
            {
                new() { TagId = 9, Name = "Awareness" },
                new() { TagId = 10, Name = "Courage" }
            },
            Author = new UserProfile
            {
                ProfileId = 5,
                UserId = "mock-user-5",
                Bio = "Mock author for testing."
            }
        }
    };

    public async Task<IReadOnlyCollection<Story>> GetPublishedAsync()
    {
        var stories = await _context.Stories
            .Include(s => s.Author)
            .Include(s => s.Tags)
            .Where(s => s.Status == StoryStatus.Published)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        // If no stories in database, return mock data for display
        if (stories.Count == 0)
        {
            return PublishedStories;
        }

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
}
