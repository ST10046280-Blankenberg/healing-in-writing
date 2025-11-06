using System.Diagnostics;
using HealingInWriting.Domain.Stories;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models;
using HealingInWriting.Models.Common;
using HealingInWriting.Models.Home;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace HealingInWriting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly IStoryService _storyService;
        private readonly IPrivacyPolicyService _privacyPolicyService;
        private readonly IOurImpactService _ourImpactService;

        public HomeController(
            ILogger<HomeController> logger,
            IEventService eventService,
            IStoryService storyService,
            IPrivacyPolicyService privacyPolicyService,
            IOurImpactService ourImpactService)
        {
            _logger = logger;
            _eventService = eventService;
            _storyService = storyService;
            _privacyPolicyService = privacyPolicyService;
            _ourImpactService = ourImpactService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllEventsAsync();
            var stories = await _storyService.GetPublishedAsync();

            // Get published events, ordered by date
            var publishedEvents = events
                .Where(e => e.EventStatus == Domain.Events.EventStatus.Published)
                .OrderBy(e => e.StartDateTime)
                .ToList();

            // Map to view model
            var viewModel = new HomeIndexViewModel();

            viewModel.Stories = stories
                .OrderByDescending(story => story.CreatedAt)
                .Take(3)
                .Select(story => new HomeStoryViewModel
                {
                    StoryId = story.StoryId,
                    Title = story.Title,
                    AuthorName = ResolveAuthorName(story),
                    Snippet = CreateSnippet(story),
                    CreatedAt = story.CreatedAt
                })
                .ToList();

            // First event is featured
            if (publishedEvents.Any())
            {
                var featured = publishedEvents.First();
                viewModel.FeaturedEvent = new HomeEventViewModel
                {
                    Id = featured.EventId,
                    Title = featured.Title,
                    Description = featured.Description,
                    EventType = featured.EventType,
                    StartDateTime = featured.StartDateTime,
                    EndDateTime = featured.EndDateTime,
                    LocationSummary = string.Join(", ", new[]
                    {
                        featured.Address?.StreetAddress,
                        featured.Address?.City,
                        featured.Address?.Province
                    }.Where(part => !string.IsNullOrWhiteSpace(part)))
                };

                // Next 3 events for the grid
                viewModel.UpcomingEvents = publishedEvents
                    .Skip(1)
                    .Take(3)
                    .Select(e => new HomeEventViewModel
                    {
                        Id = e.EventId,
                        Title = e.Title,
                        Description = e.Description,
                        EventType = e.EventType,
                        StartDateTime = e.StartDateTime,
                        EndDateTime = e.EndDateTime,
                        LocationSummary = string.Join(", ", new[]
                        {
                            e.Address?.City,
                            e.Address?.Province
                        }.Where(part => !string.IsNullOrWhiteSpace(part)))
                    })
                    .ToList();
            }

            // Fetch OurImpact
            var ourImpact = await _ourImpactService.GetAsync();
            if (ourImpact is not null)
            {
                viewModel.OurImpact = ourImpact.ToViewModel();
            }

            return View(viewModel);

            static string ResolveAuthorName(Story story)
            {
                if (story.IsAnonymous)
                {
                    return "Anonymous";
                }

                var parts = new[]
                {
                    story.Author?.User?.FirstName,
                    story.Author?.User?.LastName
                }
                .Where(part => !string.IsNullOrWhiteSpace(part))
                .ToArray();

                if (parts.Any())
                {
                    return string.Join(" ", parts);
                }

                return story.Author?.User?.Email ?? story.Author?.UserId ?? "Unknown";
            }

            static string CreateSnippet(Story story)
            {
                var source = !string.IsNullOrWhiteSpace(story.Summary)
                    ? story.Summary
                    : StripHtml(story.Content);

                if (string.IsNullOrWhiteSpace(source))
                {
                    return string.Empty;
                }

                var trimmed = source.Trim();
                if (trimmed.Length <= 180)
                {
                    return trimmed;
                }

                return trimmed.Substring(0, 177).Trim() + "...";
            }

            static string StripHtml(string value)
            {
                return string.IsNullOrWhiteSpace(value)
                    ? string.Empty
                    : Regex.Replace(value, "<[^>]*>", string.Empty);
            }
        }

        // TODO: Keep about page content static or delegate to service if dynamic content is needed.
        public async Task<IActionResult> About()
        {
            var ourImpact = await _ourImpactService.GetAsync();
            return View(ourImpact != null ? ourImpact.ToViewModel() : new OurImpactViewModel());
        }

        // TODO: Keep privacy content generation inside the service layer.
        public async Task<IActionResult> Privacy()
        {
            var privacyPolicy = await _privacyPolicyService.GetAsync();
            return View(privacyPolicy != null ? privacyPolicy.ToViewModel() : new PrivacyPolicyViewModel());
        }

        // TODO: Let the service surface diagnostics while the controller returns the view.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Resources()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        } 
        
        public IActionResult TermsOfService()
        {
            return View();
        }
        
        public IActionResult FAQ()
        {
            return View();
        }
    }
}
