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
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            VolunteerHourSummaryViewModel summary = new();
            if (User.IsInRole("Volunteer"))
            {
                summary = await _volunteerService.GetVolunteerHourSummaryAsync(user.Id);
            }

            ViewBag.VolunteerTotalHours = summary.TotalHours;
            return View();
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
