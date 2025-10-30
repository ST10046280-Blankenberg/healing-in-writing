using HealingInWriting.Domain.Events;
using HealingInWriting.Domain.Shared;

namespace HealingInWriting.Models.Events;

/// <summary>
/// Provides the event detail structure consumed by the view.
/// </summary>
public class EventDetailViewModel
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public EventStatus EventStatus { get; set; }
    public List<Tag> EventTags { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public Address Address { get; set; } = new Address();
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public int AvailableSpots { get; set; }
    public bool CanRegister { get; set; }
    public string RegistrationStatusMessage { get; set; } = string.Empty;
    public bool IsUserRegistered { get; set; }
    public int? UserRegistrationId { get; set; }
    public bool IsAuthenticated { get; set; }
}
