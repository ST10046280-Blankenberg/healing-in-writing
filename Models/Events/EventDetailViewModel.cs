using HealingInWriting.Domain.Shared;

namespace HealingInWriting.Models.Events;

/// <summary>
/// Provides the event detail structure consumed by the view.
/// </summary>
public class EventDetailViewModel
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public List<Tag> EventTags { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
}
