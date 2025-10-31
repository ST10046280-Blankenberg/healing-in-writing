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
        public IActionResult LogHours()
        {
            return View(new LogHoursViewModel());
        }

        // POST: /Dashboard/LogHours
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Volunteer")]
        public async Task<IActionResult> LogHours(LogHoursViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);

            string? attachmentUrl = null;
            if (model.Attachment != null && model.Attachment.Length > 0)
            {
                // Save file logic here
                attachmentUrl = "/uploads/" + model.Attachment.FileName;
            }

            var (success, error) = await _volunteerService.LogHoursAsync(user.Id, model, attachmentUrl);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "An error occurred.");
                return View(model);
            }

            TempData["Success"] = "Your hours have been submitted for validation.";
            return RedirectToAction("LogHours");
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
