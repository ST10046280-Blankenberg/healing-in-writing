using System;
using HealingInWriting.Domain.Events;
using HealingInWriting.Models.Filters;
using HealingInWriting.Models.Shared;

namespace HealingInWriting.Models.Events;

public class AdminManageEventsViewModel
{
    public List<AdminEventSummaryViewModel> Events { get; set; } = new();
    
    // Filter model
    public AdminEventsFilterViewModel Filters { get; set; } = new();
    
    // Dropdown options
    public List<AdminDropdownOption> StatusOptions { get; set; } = new();
    public List<AdminDropdownOption> DateOptions { get; set; } = new();
    public List<AdminDropdownOption> SortOptions { get; set; } = new();
    
    // Pagination properties
    public int CurrentPage { get; set; } = 1;
    public int TotalPages { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalEvents { get; set; }
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
