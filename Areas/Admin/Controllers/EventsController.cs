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

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
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
    }
}
