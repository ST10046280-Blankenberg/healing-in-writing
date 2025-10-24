using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Stories;
using Microsoft.AspNetCore.Mvc;
using Ganss.Xss;


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

        public IActionResult Details(int id)
        {
            // TODO: Replace with actual data retrieval once service is implemented
            var viewModel = new StoryDetailViewModel
            {
                StoryId = id,
                Title = "Article Title",
                AuthorName = "Author",
                CreatedAt = new DateTime(2025, 3, 10),
                Content = "In the thick of lockdown, 2020, poet, critic, and memoirist Jamie Hood published her debut, how to be a good girl, an interrogation of modern femininity and the narratives of love, desire, and violence yoked to it. The Rumpus praised Hood's \"bold vulnerability,\" and Vogue named it a Best Book of 2020.\n\nIn Trauma Plot, Hood draws on disparate literary forms to tell the story that lurked in good girl's margins—of three decades marred by sexual violence and the wreckage left behind. With her trademark critical remove, Hood interrogates the archetype of the rape survivor, who must perform penitence long after living through the unthinkable, invoking some of art's most infamous women to have played the role: Ovid's Philomela, David Lynch's Laura Palmer, and Artemisia Gentileschi, who captured Judith's wrath. In so doing, she asks: What do we as a culture demand of survivors? And what do survivors, in turn, owe a world that has abandoned them?\n\nTrauma Plot is a scalding work of personal and literary criticism. It is a send-up of our culture's pious disdain for \"trauma porn,\" a dirge for the broken promises of #MeToo, and a paean to finding life after death.",
                Tags = new List<Domain.Shared.Tag>
                {
                    new Domain.Shared.Tag { Name = "Tag" },
                    new Domain.Shared.Tag { Name = "Tag" },
                    new Domain.Shared.Tag { Name = "Tag" }
                }
            };

            return View(viewModel);
        }

        public IActionResult Submit()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

            // Sanitize HTML content to prevent XSS attacks
            var sanitizer = new HtmlSanitizer();
            var sanitizedContent = sanitizer.Sanitize(content);

            // TODO: Create story entity and save via service
            // Example:
            // var story = new Story
            // {
            //     Title = title,
            //     Content = sanitizedContent,
            //     IsAnonymous = anonymous,
            //     // ... set other properties
            // };
            // await _storyService.CreateAsync(story);

            // Temporary success redirect
            TempData["SuccessMessage"] = "Your story has been submitted for review!";
            return RedirectToAction(nameof(Index));
        }
    }
}
