using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Volunteer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    [Authorize(Roles = "Volunteer")]
    public class DashboardController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IVolunteerService _volunteerService;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IVolunteerService volunteerService)
        {
            _userManager = userManager;
            _volunteerService = volunteerService;
        }

        // GET: /Dashboard/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Dashboard/LogHours
        // Only volunteers can log hours
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> LogHours()
        {
            var user = await _userManager.GetUserAsync(User);
            var recentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(user.Id, 5);

            var vm = new LogHoursPageViewModel
            {
                LogForm = new LogHoursViewModel(),
                RecentEntries = recentEntries
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
                // Re-populate recent entries if validation fails
                var user = await _userManager.GetUserAsync(User);
                vm.RecentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(user.Id, 5);
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
