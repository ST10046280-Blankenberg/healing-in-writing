using HealingInWriting.Domain.Shared;

namespace HealingInWriting.Models.Events;

// TODO: Structure event listings for the events index page.
public class EventListViewModel
{
    public List<EventSummaryViewModel> Events { get; set; } = new();

    //Filtering Metadata
    public List<DateTime> AvailableDates { get; set; } = new();
}

public class EventSummaryViewModel
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public string Decription { get; set; } = string.Empty;
}