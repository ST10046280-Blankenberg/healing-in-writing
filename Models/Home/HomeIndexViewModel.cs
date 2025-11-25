using HealingInWriting.Domain.Events;
using HealingInWriting.Models.Common;

namespace HealingInWriting.Models.Home;

public class HomeIndexViewModel
{
    public HomeEventViewModel? FeaturedEvent { get; set; }
    public List<HomeEventViewModel> UpcomingEvents { get; set; } = new();
    public IReadOnlyCollection<HomeStoryViewModel> Stories { get; set; } = Array.Empty<HomeStoryViewModel>();
    public OurImpactViewModel? OurImpact { get; set; }
}

public class HomeEventViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EventType EventType { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string LocationSummary { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = "~/images/upcoming-events.webp";
}

public class HomeStoryViewModel
{
    public int StoryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Snippet { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
