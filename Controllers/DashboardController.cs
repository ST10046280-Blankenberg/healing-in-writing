using HealingInWriting.Domain.Events;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Domain.Users;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Mapping;
using HealingInWriting.Models.Dashboard;
using HealingInWriting.Models.Filters;
using HealingInWriting.Models.Volunteer;
using HealingInWriting.Services.Events;
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
        private readonly IRegistrationService _registrationService;
        private readonly IBlobStorageService _blobStorageService;

        public DashboardController(
            UserManager<ApplicationUser> userManager,
            IVolunteerService volunteerService,
            IStoryService storyService,
            IEventService eventService,
            IRegistrationService registrationService,
            IBlobStorageService blobStorageService)
        {
            _userManager = userManager;
            _volunteerService = volunteerService;
            _storyService = storyService;
            _eventService = eventService;
            _registrationService = registrationService;
            _blobStorageService = blobStorageService;
        }

        // GET: /Dashboard/Index
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || user == null)
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
            if (user == null)
            {
                return Unauthorized();
            }
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
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            if (!ModelState.IsValid)
            {
                vm.RecentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(user.Id, 5);
                var summary = await _volunteerService.GetVolunteerHourSummaryAsync(user.Id);
                vm.TotalHours = summary.TotalHours;
                vm.ValidatedHours = summary.ValidatedHours;
                vm.PendingHours = summary.PendingHours;
                vm.NeedsInfoHours = summary.NeedsInfoHours;
                return View(vm);
            }

            string? attachmentUrl = null;
            if (vm.LogForm.Attachment != null && vm.LogForm.Attachment.Length > 0)
            {
                try
                {
                    // Upload to private container (personal data - requires SAS tokens)
                    attachmentUrl = await _blobStorageService.UploadFileAsync(
                        vm.LogForm.Attachment,
                        "volunteer-hours",
                        isPublic: false);
                }
                catch (ArgumentException ex)
                {
                    // Validation errors from BlobStorageService
                    ModelState.AddModelError("", ex.Message);
                    vm.RecentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(user.Id, 5);
                    var errorSummary = await _volunteerService.GetVolunteerHourSummaryAsync(user.Id);
                    vm.TotalHours = errorSummary.TotalHours;
                    vm.ValidatedHours = errorSummary.ValidatedHours;
                    vm.PendingHours = errorSummary.PendingHours;
                    vm.NeedsInfoHours = errorSummary.NeedsInfoHours;
                    return View(vm);
                }
            }

            var (success, error) = await _volunteerService.LogHoursAsync(user.Id, vm.LogForm, attachmentUrl);

            if (!success)
            {
                ModelState.AddModelError("", error ?? "An error occurred.");
                vm.RecentEntries = await _volunteerService.GetRecentVolunteerHoursForUserAsync(user.Id, 5);
                return View(vm);
            }

            TempData["Success"] = "Your hours have been submitted for validation.";
            return RedirectToAction(nameof(LogHours), "Dashboard", new { area = "" });
        }

        // GET: /Dashboard/MyEvents
        public async Task<IActionResult> MyEvents(
            string? SearchText,
            EventType? SelectedEventType,
            DateTime? StartDate,
            DateTime? EndDate)
        {
            var profileIdClaim = User.FindFirst("ProfileId")?.Value;
            if (string.IsNullOrEmpty(profileIdClaim) || !int.TryParse(profileIdClaim, out var profileId))
            {
                return Unauthorized();
            }

            var registrationsEnumerable = await _registrationService.GetFilteredUserRegistrationsAsync(
                profileId,
                SearchText,
                SelectedEventType,
                StartDate,
                EndDate);

            // Fix: Convert IEnumerable to IReadOnlyCollection
            var registrations = registrationsEnumerable is IReadOnlyCollection<Registration> readOnly
                ? readOnly
                : registrationsEnumerable.ToList();

            var filter = new EventsFilterViewModel
            {
                EventTypeOptions = Enum.GetValues(typeof(EventType)).Cast<EventType>().ToList(),
                SelectedEventType = SelectedEventType,
                StartDate = StartDate,
                EndDate = EndDate,
                SearchText = SearchText
            };

            var model = new MyEventsViewModel
            {
                Registrations = registrations,
                Filter = filter
            };

            return View(model);
        }

        // GET: /Dashboard/MyStories
        public async Task<IActionResult> MyStories(
            string? SearchText,
            string? SelectedDate,
            string? SelectedSort,
            string? SelectedCategory)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            StoryCategory? selectedCategory = null;
            if (!string.IsNullOrWhiteSpace(SelectedCategory) && Enum.TryParse<StoryCategory>(SelectedCategory, out var parsedCategory))
            {
                selectedCategory = parsedCategory;
            }

            var stories = await _storyService.GetFilteredUserStoriesAsync(
                userId,
                SearchText,
                SelectedDate,
                SelectedSort,
                selectedCategory);

            var filter = ViewModelMappers.ToStoriesFilterViewModel(
                SelectedDate,
                SelectedSort,
                selectedCategory,
                SearchText);

            var model = new MyStoriesViewModel
            {
                Stories = stories,
                Filter = filter
            };

            return View(model);
        }
    }
}
