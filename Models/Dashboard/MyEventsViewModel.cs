using HealingInWriting.Domain.Events;
using HealingInWriting.Models.Filters;

namespace HealingInWriting.Models.Dashboard
{
    public class MyEventsViewModel
    {
        public IReadOnlyCollection<Registration> Registrations { get; set; }
        public EventsFilterViewModel Filter { get; set; }
    }
}
