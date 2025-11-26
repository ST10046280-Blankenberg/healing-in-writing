using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Mapping;
using HealingInWriting.Models.Filters;
using HealingInWriting.Models.Stories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;


namespace HealingInWriting.Controllers
{
    public class StoriesController : Controller
    {
        private readonly IStoryService _storyService;
        private readonly IBlobStorageService _blobStorageService;

        public StoriesController(IStoryService storyService, IBlobStorageService blobStorageService)
        {
            _storyService = storyService;
            _blobStorageService = blobStorageService;
        }

        public async Task<IActionResult> Index(
            string? SearchText,
            string? SelectedDate,
            string? SelectedSort,
            string? SelectedCategory)
        {
            // Parse selectedCategory from query string to nullable StoryCategory
            StoryCategory? selectedCategory = null;
            if (!string.IsNullOrWhiteSpace(SelectedCategory) && Enum.TryParse<StoryCategory>(SelectedCategory, out var parsedCategory))
            {
                selectedCategory = parsedCategory;
            }

            var filter = ViewModelMappers.ToStoriesFilterViewModel(
                categoryOptions: Enum.GetValues(typeof(StoryCategory)).Cast<StoryCategory>(),
                selectedDate: SelectedDate,
                selectedSort: SelectedSort,
                selectedCategory: selectedCategory,
                searchText: SearchText
            );

            // Get all published stories
            var stories = await _storyService.GetPublishedAsync();

            // Apply filtering
            if (!string.IsNullOrWhiteSpace(SearchText))
                stories = stories.Where(s => s.Title.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ||
                                             s.Summary.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

            if (selectedCategory.HasValue)
                stories = stories.Where(s => s.Category == selectedCategory.Value).ToList();

            // You can add more filtering for date and sort as needed

            // Sorting example
            if (SelectedSort == "Oldest")
                stories = stories.OrderBy(s => s.CreatedAt).ToList();
            else // Default to Newest
                stories = stories.OrderByDescending(s => s.CreatedAt).ToList();

            var storyList = new StoryListViewModel
            {
                Stories = stories.Select(s => s.ToStorySummaryViewModel()).ToList()
            };

            var model = new StoryListWithFilterViewModel
            {
                StoryList = storyList,
                Filter = filter
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id, string? returnUrl)
        {
            var story = await _storyService.GetStoryByIdAsync(id);
            if (story is null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin");
            var isOwner = !string.IsNullOrEmpty(currentUserId)
                && string.Equals(story.Author?.UserId, currentUserId, StringComparison.Ordinal);

            var isPublic = story.Status == StoryStatus.Published;
            if (!isPublic && !isAdmin && !isOwner)
            {
                return NotFound();
            }

            var viewModel = new StoryDetailViewModel
            {
                StoryId = story.StoryId,
                Title = story.Title,
                AuthorName = ResolveAuthorName(story, isAdmin, isOwner),
                CreatedAt = story.CreatedAt,
                Content = story.Content,
                CoverImageUrl = story.CoverImageUrl,
                Tags = story.Tags?.ToList() ?? new List<Domain.Shared.Tag>(),
                ReturnUrl = SanitizeReturnUrl(returnUrl)
            };

            return View(viewModel);

            static string ResolveAuthorName(Story story, bool isAdmin, bool isOwner)
            {
                if (story.IsAnonymous && !isAdmin && !isOwner)
                {
                    return "Anonymous";
                }

                var firstName = story.Author?.User?.FirstName;
                var lastName = story.Author?.User?.LastName;
                var fullName = string.Join(" ", new[] { firstName, lastName }
                    .Where(name => !string.IsNullOrWhiteSpace(name)));

                if (!string.IsNullOrWhiteSpace(fullName))
                {
                    return fullName;
                }

                if (!string.IsNullOrWhiteSpace(story.Author?.User?.Email))
                {
                    return story.Author.User.Email;
                }

                return story.Author?.UserId ?? "Unknown author";
            }

            string? SanitizeReturnUrl(string? candidate)
            {
                return !string.IsNullOrWhiteSpace(candidate) && Url.IsLocalUrl(candidate)
                    ? candidate
                    : null;
            }
        }

        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("standard")]
        public async Task<IActionResult> Submit(string title, string content, string tags, bool anonymous, bool consent, IFormFile? coverImage)
        {
            if (!consent)
            {
                ModelState.AddModelError("consent", "You must consent to sharing your story for review.");
                return View();
            }

            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
            {
                ModelState.AddModelError("", "Title and content are required.");
                return View();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            try
            {
                // Handle cover image upload
                string? coverImageUrl = null;
                if (coverImage != null && coverImage.Length > 0)
                {
                    coverImageUrl = await _blobStorageService.UploadImageAsync(
                        coverImage,
                        "stories",
                        isPublic: true);
                }

                await _storyService.SubmitStoryAsync(userId, title, content, tags, anonymous, coverImageUrl);
            }
            catch (ArgumentException ex)
            {
                // Validation errors from BlobStorageService
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            TempData["SuccessMessage"] = "Your story has been submitted for review!";
            return RedirectToAction(nameof(Index), new { area = "" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("standard")]
        public async Task<IActionResult> SaveDraft(string? title, string? content, string? tags, bool anonymous, IFormFile? coverImage)
        {
            if (string.IsNullOrWhiteSpace(title) && string.IsNullOrWhiteSpace(content))
            {
                return Json(new { success = false, message = "Please add a title or content to save a draft" });
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false, message = "User not authenticated" });
            }

            try
            {
                // Handle cover image upload
                string? coverImageUrl = null;
                if (coverImage != null && coverImage.Length > 0)
                {
                    coverImageUrl = await _blobStorageService.UploadImageAsync(
                        coverImage,
                        "stories",
                        isPublic: true);
                }

                await _storyService.SaveDraftAsync(
                    userId,
                    title ?? "",
                    content ?? "",
                    tags ?? "",
                    anonymous,
                    coverImageUrl);
                return Json(new { success = true, message = "Draft saved successfully!" });
            }
            catch (ArgumentException ex)
            {
                // Validation errors from BlobStorageService
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error saving draft: {ex.Message}" });
            }
        }
    }
}
