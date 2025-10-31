using HealingInWriting.Domain.Shared;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Stories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;


namespace HealingInWriting.Controllers
{
    public class StoriesController : Controller
    {
        private readonly IStoryService _storyService;

        public StoriesController(IStoryService storyService)
        {
            _storyService = storyService;
        }

        public async Task<IActionResult> Index()
        {
            var stories = await _storyService.GetPublishedAsync();

            var viewModel = new StoryListViewModel
            {
                Stories = stories.Select(story => new StorySummaryViewModel
                {
                    StoryId = story.StoryId,
                    Title = story.Title,
                    Summary = story.Summary,
                    CreatedAt = story.CreatedAt,
                    AuthorName = story.Author?.UserId ?? string.Empty,
                    Tags = story.Tags.Select(tag => tag.Name).ToList()
                }).ToList()
            };

            return View(viewModel);
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
        public async Task<IActionResult> Submit(string title, string content, string tags, bool anonymous, bool consent)
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
                await _storyService.SubmitStoryAsync(userId, title, content, tags, anonymous);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View();
            }
            TempData["SuccessMessage"] = "Your story has been submitted for review!";
            return RedirectToAction(nameof(Index), new { area = "" });
        }
    }
}
