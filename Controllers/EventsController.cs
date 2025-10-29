using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllEventsAsync();

            // Map to view model
            var viewModel = new EventsIndexViewModel
            {
                Events = events
                    .Where(e => e.EventStatus == Domain.Events.EventStatus.Published)
                    .OrderBy(e => e.StartDateTime)
                    .Select(e => new EventCardViewModel
                    {
                        Id = e.EventId,
                        Title = e.Title,
                        Description = e.Description,
                        EventType = e.EventType,
                        StartDateTime = e.StartDateTime,
                        LocationSummary = string.Join(", ", new[]
                        {
                            e.Address?.City,
                            e.Address?.Province
                        }.Where(part => !string.IsNullOrWhiteSpace(part)))
                    })
                    .ToList()
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            // TODO: Create detailed view model and view
            return View(@event);
        }
    }
}
