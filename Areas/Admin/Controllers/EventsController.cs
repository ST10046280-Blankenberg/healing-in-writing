using HealingInWriting.Domain.Events;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {

        private readonly IEventService _eventService;
        private readonly IRegistrationService _registrationService;

        public EventsController(IEventService eventService, IRegistrationService registrationService)
        {
            _eventService = eventService;
            _registrationService = registrationService;
        }
        // GET: Admin/Events/Manage
        public async Task<IActionResult> Manage()
        {
            var events = await _eventService.GetAllEventsAsync();

            var model = new AdminManageEventsViewModel
            {
                Events = events.Select(@event =>
                {
                    var locationParts = new[]
                        {
                            @event.Address?.StreetAddress,
                            @event.Address?.City,
                            @event.Address?.Province
                        }
                        .Where(part => !string.IsNullOrWhiteSpace(part));

                    var isRsvpOpen = @event.EventStatus == EventStatus.Published
                                     && @event.StartDateTime > DateTime.UtcNow;

                    return new AdminEventSummaryViewModel
                    {
                        Id = @event.EventId,
                        Title = @event.Title,
                        EventType = @event.EventType,
                        Status = @event.EventStatus,
                        StartDateTime = @event.StartDateTime,
                        EndDateTime = @event.EndDateTime,
                        LocationSummary = string.Join(", ", locationParts),
                        IsRsvpOpen = isRsvpOpen
                    };
                }).ToList()
            };

            return View(model);
        }

        // [HttpGet]
        // // GET: Admin/Events/Details
        // public IActionResult Details()
        // {
        //     return View();
        // }
        
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            var model = new CreateEventViewModel
            {
                Id = id ?? 0,
                EventDate = DateTime.Today,
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(10, 0, 0)
            };

            if (id.HasValue)
            {
                var existingEvent = await _eventService.GetEventByIdAsync(id.Value);
                if (existingEvent != null)
                {
                    model.Title = existingEvent.Title;
                    model.Description = existingEvent.Description;
                    model.EventType = existingEvent.EventType;
                    model.EventStatus = existingEvent.EventStatus;
                    model.EventDate = existingEvent.StartDateTime.Date;
                    model.StartTime = existingEvent.StartDateTime.TimeOfDay;
                    model.EndTime = existingEvent.EndDateTime.TimeOfDay;
                    model.Capacity = existingEvent.Capacity;

                    if (existingEvent.Address != null)
                    {
                        model.StreetAddress = existingEvent.Address.StreetAddress;
                        model.Suburb = existingEvent.Address.Suburb;
                        model.City = existingEvent.Address.City;
                        model.Province = existingEvent.Address.Province;
                        model.PostalCode = existingEvent.Address.PostalCode;
                        model.Latitude = existingEvent.Address.Latitude;
                        model.Longitude = existingEvent.Address.Longitude;
                    }

                    // Load existing tags as comma-separated string
                    if (existingEvent.EventTags != null && existingEvent.EventTags.Any())
                    {
                        model.Tags = string.Join(",", existingEvent.EventTags.Select(t => t.Name));
                    }
                }
            }

            return View(model);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "User not authenticated");
                    return View(model);
                }

                if (model.Id > 0)
                {
                    // Update existing event
                    await _eventService.UpdateEventAsync(model, userId);
                    TempData["SuccessMessage"] = "Event updated successfully!";
                }
                else
                {
                    // Create new event
                    var eventId = await _eventService.CreateEventAsync(model, userId);
                    TempData["SuccessMessage"] = "Event created successfully!";
                }

                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                var action = model.Id > 0 ? "updating" : "creating";
                ModelState.AddModelError("", $"Error {action} event: {ex.Message}");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _eventService.DeleteEventAsync(id);

                if (success)
                {
                    TempData["SuccessMessage"] = "Event deleted successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = "Event not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting event: {ex.Message}";
            }

            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, EventStatus status)
        {
            try
            {
                var success = await _eventService.UpdateEventStatusAsync(id, status);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Event status updated to {status}.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Event not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating event status: {ex.Message}";
            }

            return RedirectToAction(nameof(Manage));
        }

        [HttpGet]
        public async Task<IActionResult> Registrations(int id)
        {
            var @event = await _eventService.GetEventByIdAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            var registrations = await _registrationService.GetEventRegistrationsAsync(id);

            var viewModel = new EventRegistrationsViewModel
            {
                EventId = @event.EventId,
                EventTitle = @event.Title,
                EventStartDateTime = @event.StartDateTime,
                Capacity = @event.Capacity,
                RegisteredCount = registrations.Count(),
                Registrations = registrations.Select(r => new RegistrationItemViewModel
                {
                    RegistrationId = r.RegistrationId,
                    AttendeeName = r.UserId.HasValue
                        ? $"{r.User?.User?.FirstName} {r.User?.User?.LastName}".Trim()
                        : r.GuestName ?? "Unknown",
                    AttendeeEmail = r.UserId.HasValue
                        ? r.User?.User?.Email ?? "N/A"
                        : r.GuestEmail ?? "N/A",
                    AttendeePhone = r.UserId.HasValue
                        ? r.User?.User?.PhoneNumber
                        : r.GuestPhone,
                    RegistrationDate = r.RegistrationDate,
                    IsGuest = !r.UserId.HasValue,
                    IsAdminOverride = r.IsAdminOverride
                }).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddRegistration(int eventId, string guestName, string guestEmail, string? guestPhone)
        {
            var result = await _registrationService.RegisterGuestAsync(
                eventId,
                guestName,
                guestEmail,
                ipAddress: null,
                guestPhone,
                isAdminOverride: true);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Registration added successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Registrations), new { id = eventId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveRegistration(int registrationId, int eventId)
        {
            var result = await _registrationService.CancelRegistrationAsync(
                registrationId,
                requestingUserId: null,
                isAdminOverride: true);

            if (result.Success)
            {
                TempData["SuccessMessage"] = "Registration removed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message;
            }

            return RedirectToAction(nameof(Registrations), new { id = eventId });
        }
    }
}
