using HealingInWriting.Domain.Events;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using HealingInWriting.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

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

        public async Task<IActionResult> Index(
            string? SearchText,
            EventType? SelectedEventType,
            DateTime? StartDate,
            DateTime? EndDate)
        {
            // Prepare filter options
            var filter = new EventsFilterViewModel
            {
                EventTypeOptions = Enum.GetValues(typeof(EventType)).Cast<EventType>().ToList(),
                SelectedEventType = SelectedEventType,
                StartDate = StartDate,
                EndDate = EndDate,
                SearchText = SearchText
            };

            // Get all events
            var events = await _eventService.GetAllEventsAsync();

            // Only published events
            var filtered = events.Where(e => e.EventStatus == EventStatus.Published);

            // Apply search text
            if (!string.IsNullOrWhiteSpace(SearchText))
                filtered = filtered.Where(e => e.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase)
                                       || e.Description.Contains(SearchText, StringComparison.OrdinalIgnoreCase));

            // Filter by event type
            if (SelectedEventType.HasValue)
                filtered = filtered.Where(e => e.EventType == SelectedEventType.Value);

            // Filter by date range
            if (StartDate.HasValue)
                filtered = filtered.Where(e => e.StartDateTime >= StartDate.Value);
            if (EndDate.HasValue)
                filtered = filtered.Where(e => e.EndDateTime <= EndDate.Value);

            // Map to view model
            var eventsList = new EventsIndexViewModel
            {
                Events = filtered
                    .OrderBy(e => e.StartDateTime)
                    .Select(e => new EventCardViewModel
                    {
                        Id = e.EventId,
                        Title = e.Title,
                        Description = e.Description,
                        EventType = e.EventType,
                        StartDateTime = e.StartDateTime,
                        LocationSummary = string.Join(", ", new[] { e.Address?.City, e.Address?.Province }.Where(part => !string.IsNullOrWhiteSpace(part)))
                    })
                    .ToList()
            };

            var model = new EventsListWithFiltersViewModel
            {
                EventsList = eventsList,
                Filter = filter
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            var capacityInfo = await _registrationService.GetCapacityInfoAsync(id);

            var isAuthenticated = User.Identity?.IsAuthenticated ?? false;
            var isUserRegistered = false;
            int? userRegistrationId = null;

            if (isAuthenticated && int.TryParse(User.FindFirst("ProfileId")?.Value, out var userId))
            {
                isUserRegistered = await _registrationService.IsUserRegisteredAsync(id, userId);
                if (isUserRegistered)
                {
                    var userRegistrations = await _registrationService.GetUserRegistrationsAsync(userId);
                    userRegistrationId = userRegistrations
                        .FirstOrDefault(r => r.EventId == id)?.RegistrationId;
                }
            }

            var viewModel = new EventDetailViewModel
            {
                Id = @event.EventId,
                EventType = @event.EventType.ToString(),
                EventStatus = @event.EventStatus,
                EventTags = @event.EventTags,
                Title = @event.Title,
                StartDateTime = @event.StartDateTime,
                EndDateTime = @event.EndDateTime,
                Description = @event.Description,
                Address = @event.Address,
                Capacity = capacityInfo.Capacity,
                RegisteredCount = capacityInfo.RegisteredCount,
                AvailableSpots = capacityInfo.AvailableSpots,
                CanRegister = capacityInfo.CanRegister,
                RegistrationStatusMessage = capacityInfo.StatusMessage,
                IsUserRegistered = isUserRegistered,
                UserRegistrationId = userRegistrationId,
                IsAuthenticated = isAuthenticated
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(int eventId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
            {
                return RedirectToAction("RegisterGuest", "Events", new { area = "", eventId });
            }

            if (!int.TryParse(User.FindFirst("ProfileId")?.Value, out var userId))
            {
                TempData["ErrorMessage"] = "Unable to process registration. Please try logging in again.";
                return RedirectToAction("Details", "Events", new { area = "", id = eventId });
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

            return RedirectToAction("Details", "Events", new { area = "", id = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterGuest(GuestRegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please provide valid registration details.";
                return RedirectToAction("Details", "Events", new { area = "", id = model.EventId });
            }

            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();

            var result = await _registrationService.RegisterGuestAsync(
                model.EventId,
                model.GuestName,
                model.GuestEmail,
                ipAddress,
                model.GuestPhone);

            if (result.Success)
            {
                TempData["SuccessMessage"] = result.Message;
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction("Details", "Events", new { area = "", id = model.EventId });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRegistration(int registrationId, int eventId)
        {
            if (!int.TryParse(User.FindFirst("ProfileId")?.Value, out var userId))
            {
                TempData["ErrorMessage"] = "Unable to process cancellation. Please try logging in again.";
                return RedirectToAction("Details", "Events", new { area = "", id = eventId });
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

            return RedirectToAction("Details", "Events", new { area = "", id = eventId });
        }
    }
}
