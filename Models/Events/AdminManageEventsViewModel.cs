using System;
using HealingInWriting.Domain.Events;

namespace HealingInWriting.Models.Events;

public class AdminManageEventsViewModel
{
    public List<AdminEventSummaryViewModel> Events { get; set; } = new();
}

public class AdminEventSummaryViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public EventType EventType { get; set; }
    public EventStatus Status { get; set; }
    public DateTime StartDateTime { get; set; }
    public DateTime EndDateTime { get; set; }
    public string LocationSummary { get; set; } = string.Empty;
    public bool IsRsvpOpen { get; set; }
}
