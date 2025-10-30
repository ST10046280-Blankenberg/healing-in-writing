namespace HealingInWriting.Models.Events;

/// <summary>
/// View model for admin viewing all registrations for an event.
/// </summary>
public class EventRegistrationsViewModel
{
    public int EventId { get; set; }
    public string EventTitle { get; set; } = string.Empty;
    public DateTime EventStartDateTime { get; set; }
    public int Capacity { get; set; }
    public int RegisteredCount { get; set; }
    public List<RegistrationItemViewModel> Registrations { get; set; } = new();
}

/// <summary>
/// Individual registration item for display in admin list.
/// </summary>
public class RegistrationItemViewModel
{
    public int RegistrationId { get; set; }
    public string AttendeeName { get; set; } = string.Empty;
    public string AttendeeEmail { get; set; } = string.Empty;
    public string? AttendeePhone { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool IsGuest { get; set; }
    public bool IsAdminOverride { get; set; }
}
