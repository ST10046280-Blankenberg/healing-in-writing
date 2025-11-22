using HealingInWriting.Domain.Events;

namespace HealingInWriting.Models.Filters
{
    /// <summary>
    /// Filter parameters for user-facing Events page
    /// </summary>
    public class EventsFilterViewModel : BaseFilterViewModel
    {
        public List<EventType> EventTypeOptions { get; set; } = new();
        public EventType? SelectedEventType { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}