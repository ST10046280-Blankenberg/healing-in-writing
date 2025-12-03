using System;
using System.Linq;
using System.Threading.Tasks;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StoriesController : Controller
    {
        private readonly IStoryService _storyService;

        public StoriesController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        /// <summary>
        /// Displays the admin story management page with filtering, sorting, and pagination.
        /// Delegates all business logic to the story service layer.
        /// </summary>
        // GET: Admin/Stories/Manage
        public async Task<IActionResult> Manage(string? searchTerm, string? status, string? dateRange, string? tag, string? sortOrder, int page = 1)
        {
            const int pageSize = 10;
            var currentPage = Math.Max(page, 1);
            var normalizedSortOrder = string.IsNullOrWhiteSpace(sortOrder) ? "newest" : sortOrder.ToLowerInvariant();

            // Parse status filter
            StoryStatus? statusFilter = null;
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<StoryStatus>(status, true, out var parsed))
            {
                statusFilter = parsed;
            }

            // Get filtered stories from service
            var (stories, totalCount) = await _storyService.GetFilteredStoriesForAdminAsync(
                searchTerm,
                statusFilter,
                dateRange,
                tag,
                normalizedSortOrder,
                currentPage,
                pageSize);

            // Get all stories for dropdowns and status counts
            var allStories = await _storyService.GetAllStoriesForAdminAsync();
            var statusCounts = _storyService.CalculateStatusCounts(allStories);

            // Map to view models using service methods
            var storyViewModels = stories.Select(story => new AdminStoryListItemViewModel
            {
                StoryId = story.StoryId,
                Title = story.Title,
                AuthorName = _storyService.ResolveAuthorName(story),
                CreatedAt = story.CreatedAt,
                Status = story.Status,
                StatusText = story.Status.ToString(),
                StatusBadgeClass = _storyService.GetStatusBadgeClass(story.Status)
            }).ToList();

            var totalPages = Math.Max((int)Math.Ceiling(totalCount / (double)pageSize), 1);

            var viewModel = new AdminManageStoriesViewModel
            {
                Stories = storyViewModels,
                Filters = new AdminManageStoriesFilters
                {
                    SearchTerm = searchTerm,
                    Status = status,
                    DateRange = dateRange,
                    Tag = tag,
                    SortOrder = normalizedSortOrder,
                    Page = currentPage
                },
                StatusOptions = _storyService.BuildStatusOptions(status),
                DateOptions = _storyService.BuildDateOptions(dateRange),
                TagOptions = _storyService.BuildTagOptions(tag, allStories),
                SortOptions = _storyService.BuildSortOptions(normalizedSortOrder),
                PendingCount = statusCounts.GetValueOrDefault(StoryStatus.Submitted),
                PublishedCount = statusCounts.GetValueOrDefault(StoryStatus.Published),
                DraftCount = statusCounts.GetValueOrDefault(StoryStatus.Draft),
                RejectedCount = statusCounts.GetValueOrDefault(StoryStatus.Rejected),
                TotalStories = totalCount,
                CurrentPage = currentPage,
                TotalPages = totalPages,
                PageSize = pageSize
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? returnUrl)
        {
            await _storyService.UpdateStoryStatusAsync(id, StoryStatus.Published, User.Identity?.Name ?? "Admin");
            TempData["StoryStatusMessage"] = "Story approved and published.";
            var redirect = RedirectToSafeUrl(returnUrl);
            if (redirect != null)
            {
                return redirect;
            }

            return RedirectToAction(nameof(Manage));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? returnUrl)
        {
            await _storyService.UpdateStoryStatusAsync(id, StoryStatus.Rejected, User.Identity?.Name ?? "Admin");
            TempData["StoryStatusMessage"] = "Story rejected.";
            var redirect = RedirectToSafeUrl(returnUrl);
            if (redirect != null)
            {
                return redirect;
            }

            return RedirectToAction(nameof(Manage));
        }

        /// <summary>
        /// Safely redirects to a local URL if valid, otherwise returns null.
        /// Prevents open redirect vulnerabilities by validating the URL.
        /// </summary>
        private RedirectResult? RedirectToSafeUrl(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return null;
        }
    }
}
