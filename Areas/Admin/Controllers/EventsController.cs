using HealingInWriting.Data;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        
        private readonly IEventService _eventService;
        private readonly ApplicationDbContext _context;

        public EventsController(IEventService eventService, ApplicationDbContext context)
        {
            _eventService = eventService;
            _context = context;
        }
        // GET: Admin/Events/Manage
        public IActionResult Manage()
        {
            return View();
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
                Id = id ?? 0
            };

            if (id.HasValue)
            {
                var existingEvent = await _eventService.GetEventByIdAsync(id.Value);
                if (existingEvent != null)
                {
                    model.Title = existingEvent.Title;
                    model.Description = existingEvent.Description;
                    model.EventType = existingEvent.EventType;
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

                var eventId = await _eventService.CreateEventAsync(model, userId);

                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating event: {ex.Message}");
                return View(model);
            }
        }
    }
}
