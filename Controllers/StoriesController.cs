using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Stories;
using Microsoft.AspNetCore.Mvc;


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
    }
}
