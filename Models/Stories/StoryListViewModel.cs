using HealingInWriting.Domain.Shared;

namespace HealingInWriting.Models.Stories;

// TODO: Prepare story list data shaped for the stories index view.
public class StoryListViewModel
{
    public List<StorySummaryViewModel> Stories { get; set; } = new();

    // Filtering Metadata
    public List<Tag> AvailableTags { get; set; } = new();

    public List<int> SelectedTagIds { get; set; } = new();


}

public class StorySummaryViewModel
{
    public int StoryId { get; set; }
    public List<Tag> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public string Title { get; set; }
    public string AuthorName { get; set; }
    public string Summary { get; set; }
}
