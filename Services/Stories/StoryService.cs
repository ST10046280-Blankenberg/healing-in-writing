using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Services;

namespace HealingInWriting.Services.Stories;

/// <summary>
/// Temporary in-memory story service to keep controllers thin until persistence is wired up.
/// </summary>
public class StoryService : IStoryService
{
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

    public Task<IReadOnlyCollection<Story>> GetPublishedAsync()
    {
        return Task.FromResult(PublishedStories);
    }
}
