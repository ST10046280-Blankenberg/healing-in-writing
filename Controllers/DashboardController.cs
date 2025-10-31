using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealingInWriting.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IStoryService _storyService;
        private readonly IEventService _eventService;

        public DashboardController(IStoryService storyService, IEventService eventService)
        {
            _storyService = storyService;
            _eventService = eventService;
        }

        // GET: /Dashboard/Index
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var viewModel = new DashboardViewModel
            {
                UserName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? User.Identity?.Name ?? "User",
                MyStoriesCount = await _storyService.GetUserStoryCountAsync(userId),
                MyEventsCount = await _eventService.GetUserUpcomingEventsCountAsync(userId),
                MyHoursCount = User.IsInRole("Volunteer") ? 0 : null // TODO: Implement volunteer hours tracking
            };

            return View(viewModel);
        }

        // GET: /Dashboard/LogHours
        // Only volunteers can log hours
        [Authorize(Roles = "Volunteer")]
        public IActionResult LogHours()
        {
            return View();
        }

        // GET: /Dashboard/MyEvents
        public IActionResult MyEvents()
        {
            return View();
        }

        // GET: /Dashboard/MyStories
        public IActionResult MyStories()
        {
            return View();
        }
    }
}
