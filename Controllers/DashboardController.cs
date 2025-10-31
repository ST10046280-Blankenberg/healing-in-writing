using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Dashboard;
using HealingInWriting.Models.Volunteer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HealingInWriting.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVolunteerService _volunteerService;
        private readonly IStoryService _storyService;
        private readonly IEventService _eventService;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IVolunteerService volunteerService,
            IStoryService storyService,
            IEventService eventService)
        {
            _userManager = userManager;
            _volunteerService = volunteerService;
            _storyService = storyService;
            _eventService = eventService;
        }

        // GET: /Dashboard/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            VolunteerHourSummaryViewModel summary = new();
            if (User.IsInRole("Volunteer"))
            {
                summary = await _volunteerService.GetVolunteerHourSummaryAsync(user.Id);
            }

            var viewModel = new DashboardViewModel
            {
                UserName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? User.Identity?.Name ?? "User",
                MyStoriesCount = await _storyService.GetUserStoryCountAsync(userId),
                MyEventsCount = await _eventService.GetUserUpcomingEventsCountAsync(userId),
                MyHoursCount = User.IsInRole("Volunteer") ? (int)summary.TotalHours : null
            };

            return View(viewModel);
        }

        // GET: /Dashboard/LogHours
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> LogHours()
        {
            var user = await _userManager.GetUserAsync(User);
            var recentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(user.Id, 5);

            var summary = await _volunteerService.GetVolunteerHourSummaryAsync(user.Id);

            var vm = new LogHoursPageViewModel
            {
                LogForm = new LogHoursViewModel(),
                RecentEntries = recentEntries,
                TotalHours = summary.TotalHours,
                ValidatedHours = summary.ValidatedHours,
                PendingHours = summary.PendingHours,
                NeedsInfoHours = summary.NeedsInfoHours
            };
            return View(vm);
        }

        // POST: /Dashboard/LogHours
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> LogHours(LogHoursPageViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                vm.RecentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(user.Id, 5);
                var summary = await _volunteerService.GetVolunteerHourSummaryAsync(user.Id);
                vm.TotalHours = summary.TotalHours;
                vm.ValidatedHours = summary.ValidatedHours;
                vm.PendingHours = summary.PendingHours;
                vm.NeedsInfoHours = summary.NeedsInfoHours;
                return View(vm);
            }

            var userObj = await _userManager.GetUserAsync(User);

            string? attachmentUrl = null;
            if (vm.LogForm.Attachment != null && vm.LogForm.Attachment.Length > 0)
            {
                // Save file logic here
                attachmentUrl = "/uploads/" + vm.LogForm.Attachment.FileName;
            }

            var (success, error) = await _volunteerService.LogHoursAsync(userObj.Id, vm.LogForm, attachmentUrl);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "An error occurred.");
                vm.RecentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(userObj.Id, 5);
                return View(vm);
            }

            TempData["Success"] = "Your hours have been submitted for validation.";
            return RedirectToAction(nameof(LogHours), "Dashboard", new { area = "" });
        }

        // GET: /Dashboard/MyEvents
        public async Task<IActionResult> MyEvents()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var registrations = await _eventService.GetUserRegistrationsAsync(userId);
            return View(registrations);
        }

        // GET: /Dashboard/MyStories
        public async Task<IActionResult> MyStories()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var stories = await _storyService.GetUserStoriesAsync(userId);
            return View(stories);
        }
    }
}
