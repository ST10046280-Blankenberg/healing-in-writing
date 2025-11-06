using HealingInWriting.Models.Filters;

namespace HealingInWriting.Models.Events
{
    public class EventsListWithFiltersViewModel
    {
        public EventsIndexViewModel EventsList { get; set; }
        public EventsFilterViewModel Filter { get; set; }
    }
}