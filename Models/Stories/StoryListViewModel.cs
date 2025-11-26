namespace HealingInWriting.Models.Stories;

public class StoryListViewModel
{
    public List<StorySummaryViewModel> Stories { get; set; } = new();

    // Filtering Metadata
    public List<string> AvailableTags { get; set; } = new();

    public List<string> SelectedTags { get; set; } = new();
}

public class StorySummaryViewModel
{
    public int StoryId { get; set; }
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
}
