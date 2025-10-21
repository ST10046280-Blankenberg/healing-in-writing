using HealingInWriting.Interfaces.Services;
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
            return View(stories);
        }
    }
}
