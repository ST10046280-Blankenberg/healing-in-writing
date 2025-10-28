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
                AvailableTags = await _context.Tags.ToListAsync()
            };

            // TODO: If id has value, load existing event for editing
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Details(CreateEventViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableTags = await _context.Tags.ToListAsync();
                return View(model);
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var eventId = await _eventService.CreateEventAsync(model, userId);

                TempData["SuccessMessage"] = "Event created successfully!";
                return RedirectToAction(nameof(Manage));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating event: {ex.Message}");
                model.AvailableTags = await _context.Tags.ToListAsync();
                return View(model);
            }
        }
    }
}
