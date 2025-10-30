using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealingInWriting.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly IRegistrationService _registrationService;

        public EventsController(IEventService eventService, IRegistrationService registrationService)
        {
            _eventService = eventService;
            _registrationService = registrationService;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int eventId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("RegisterGuest", new { eventId });
            }

            if (!int.TryParse(User.FindFirst("ProfileId")?.Value, out var userId))
            {
                TempData["ErrorMessage"] = "Unable to process registration. Please try logging in again.";
                return RedirectToAction("Details", new { id = eventId });
            }

            var result = await _registrationService.RegisterUserAsync(eventId, userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Details", new { id = eventId });
        }

        [HttpGet]
        public IActionResult RegisterGuest(int eventId)
        {
            var model = new GuestRegistrationViewModel { EventId = eventId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterGuest(GuestRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _registrationService.RegisterGuestAsync(
                model.EventId,
                model.GuestName,
                model.GuestEmail,
                model.GuestPhone);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
                return RedirectToAction("Details", new { id = model.EventId });
            }

            ModelState.AddModelError("", result.Message);
            return View(model);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRegistration(int registrationId, int eventId)
        {
            if (!int.TryParse(User.FindFirst("ProfileId")?.Value, out var userId))
            {
                TempData["ErrorMessage"] = "Unable to process cancellation. Please try logging in again.";
                return RedirectToAction("Details", new { id = eventId });
            }

            var result = await _registrationService.CancelRegistrationAsync(registrationId, userId);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Details", new { id = eventId });
        }
    }
}
