using HealingInWriting.Domain.Shared;

namespace HealingInWriting.Models.Stories;

// TODO: Represent detailed story content tailored for the details view.
public class StoryDetailViewModel
{
    public int StoryId { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string Content { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? ReturnUrl { get; set; }
}
