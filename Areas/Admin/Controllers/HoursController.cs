using HealingInWriting.Models.Volunteer;
using HealingInWriting.Domain.Volunteers;
using HealingInWriting.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Areas.Admin.Controllers

{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HoursController : Controller
    {
        private readonly IVolunteerService _volunteerService;

        public HoursController(IVolunteerService volunteerService)
        {
            _volunteerService = volunteerService;
        }

        public async Task<IActionResult> Index(
            DateOnly? startDate,
            DateOnly? endDate,
            string? status,
            string? orderBy,
            string? search)
        {
            var filter = new VolunteerHourFilterViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Status = status,
                OrderBy = orderBy,
                Search = search
            };

            filter.Results = await _volunteerService.GetFilteredVolunteerHourApprovalsAsync(
                startDate, endDate, status, orderBy, search);

            // Pass enum values for the status dropdown
            ViewBag.StatusOptions = Enum.GetNames(typeof(VolunteerHourStatus));
            return View(filter);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(Guid id, string status)
        {
            // You may want to validate the status string here
            var result = await _volunteerService.UpdateHourStatusAsync(id, status, User.Identity?.Name);
            if (!result.Success)
            {
                TempData["Error"] = result.Error;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

