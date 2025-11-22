using System;
using System.Collections.Generic;
using System.Globalization;
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

            // Get status counts (presentation logic)
            var allStories = await _storyService.GetAllStoriesForAdminAsync();
            var statusCounts = CalculateStatusCounts(allStories);

            // Map to view models
            var storyViewModels = stories.Select(story => new AdminStoryListItemViewModel
            {
                StoryId = story.StoryId,
                Title = story.Title,
                AuthorName = ResolveAuthorName(story),
                CreatedAt = story.CreatedAt,
                Status = story.Status,
                StatusText = story.Status.ToString(),
                StatusBadgeClass = GetStatusBadgeClass(story.Status)
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
                StatusOptions = BuildStatusOptions(status),
                DateOptions = BuildDateOptions(dateRange),
                TagOptions = BuildTagOptions(tag, allStories),
                SortOptions = BuildSortOptions(normalizedSortOrder),
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

        private static string ResolveAuthorName(Story story)
        {
            if (story.IsAnonymous)
            {
                return "Anonymous";
            }

            var firstName = story.Author?.User?.FirstName;
            var lastName = story.Author?.User?.LastName;

            var nameParts = new[] { firstName, lastName }
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();

            if (nameParts.Any())
            {
                return string.Join(" ", nameParts);
            }

            if (!string.IsNullOrWhiteSpace(story.Author?.User?.Email))
            {
                return story.Author!.User!.Email!;
            }

            return story.Author?.UserId ?? "Unknown";
        }

        private static string GetStatusBadgeClass(StoryStatus status)
        {
            return status switch
            {
                StoryStatus.Submitted => "manage-stories__status-badge--submitted",
                StoryStatus.Published => "manage-stories__status-badge--published",
                StoryStatus.Rejected => "manage-stories__status-badge--rejected",
                StoryStatus.Draft => "manage-stories__status-badge--pending",
                StoryStatus.Archived => "manage-stories__status-badge--approved",
                _ => "manage-stories__status-badge"
            };
        }

        private static Dictionary<StoryStatus, int> CalculateStatusCounts(IEnumerable<Story> stories)
        {
            return stories
                .GroupBy(story => story.Status)
                .ToDictionary(group => group.Key, group => group.Count());
        }


        private static IReadOnlyCollection<AdminSelectOption> BuildStatusOptions(string? selectedStatus)
        {
            var options = new List<AdminSelectOption>
            {
                new("", "Any Status", string.IsNullOrWhiteSpace(selectedStatus))
            };

            foreach (var status in Enum.GetValues(typeof(StoryStatus)).Cast<StoryStatus>())
            {
                options.Add(new AdminSelectOption(
                    status.ToString(),
                    CultureInfo.CurrentCulture.TextInfo.ToTitleCase(status.ToString().Replace("_", " ").ToLowerInvariant()),
                    string.Equals(selectedStatus, status.ToString(), StringComparison.OrdinalIgnoreCase)));
            }

            return options;
        }

        private static IReadOnlyCollection<AdminSelectOption> BuildDateOptions(string? selectedDate)
        {
            var options = new List<AdminSelectOption>
            {
                new("any", "Any Date", string.IsNullOrWhiteSpace(selectedDate) || selectedDate.Equals("any", StringComparison.OrdinalIgnoreCase)),
                new("last7", "Last 7 days", string.Equals(selectedDate, "last7", StringComparison.OrdinalIgnoreCase)),
                new("last30", "Last 30 days", string.Equals(selectedDate, "last30", StringComparison.OrdinalIgnoreCase)),
                new("last90", "Last 90 days", string.Equals(selectedDate, "last90", StringComparison.OrdinalIgnoreCase)),
                new("this-year", "This year", string.Equals(selectedDate, "this-year", StringComparison.OrdinalIgnoreCase))
            };

            return options;
        }

        private static IReadOnlyCollection<AdminSelectOption> BuildTagOptions(string? selectedTag, IEnumerable<Story> stories)
        {
            var tags = stories
                .SelectMany(story => story.Tags)
                .Select(tag => tag.Name)
                .Where(name => !string.IsNullOrWhiteSpace(name))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var options = new List<AdminSelectOption>
            {
                new("", "Any Tag", string.IsNullOrWhiteSpace(selectedTag))
            };

            foreach (var tag in tags)
            {
                options.Add(new AdminSelectOption(tag, tag, string.Equals(tag, selectedTag, StringComparison.OrdinalIgnoreCase)));
            }

            // Preserve manually entered tags even if not present in dataset
            if (!string.IsNullOrWhiteSpace(selectedTag)
                && options.All(option => !string.Equals(option.Value, selectedTag, StringComparison.OrdinalIgnoreCase)))
            {
                options.Add(new AdminSelectOption(selectedTag, selectedTag, true));
            }

            return options;
        }

        private static IReadOnlyCollection<AdminSelectOption> BuildSortOptions(string? selectedSort)
        {
            var normalizedSort = string.IsNullOrWhiteSpace(selectedSort)
                ? "newest"
                : selectedSort.ToLowerInvariant();

            return new List<AdminSelectOption>
            {
                new("newest", "Newest", normalizedSort == "newest"),
                new("oldest", "Oldest", normalizedSort == "oldest")
            };
        }


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
