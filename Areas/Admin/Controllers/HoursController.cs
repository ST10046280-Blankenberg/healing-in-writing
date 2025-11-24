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
        private readonly IBlobStorageService _blobStorageService;

        public HoursController(IVolunteerService volunteerService, IBlobStorageService blobStorageService)
        {
            _volunteerService = volunteerService;
            _blobStorageService = blobStorageService;
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

        /// <summary>
        /// Generates a temporary SAS token URL for viewing a volunteer hour attachment.
        /// </summary>
        [HttpGet]
        public IActionResult GetAttachmentUrl(string blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl))
            {
                return BadRequest("Blob URL is required");
            }

            // Check if it's a blob storage URL
            if (!blobUrl.Contains("blob.core.windows.net"))
            {
                // Legacy local file or invalid URL
                return Redirect(blobUrl);
            }

            try
            {
                // Generate SAS token URL (1 hour expiry)
                var sasUrl = _blobStorageService.GenerateSasUrl(blobUrl, expiryHours: 1);
                return Redirect(sasUrl);
            }
            catch (Exception)
            {
                return NotFound("Unable to generate access URL for attachment");
            }
        }
    }
}

