using HealingInWriting.Domain.Events;

namespace HealingInWriting.Models.Events;

public class EventsIndexViewModel
{
    public List<EventCardViewModel> Events { get; set; } = new();
}

public class EventCardViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public EventType EventType { get; set; }
    public DateTime StartDateTime { get; set; }
    public string LocationSummary { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = "https://via.placeholder.com/304x262";
}
