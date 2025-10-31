using System.Diagnostics;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models;
using HealingInWriting.Models.Common;
using HealingInWriting.Models.Home;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IEventService _eventService;
        private readonly IPrivacyPolicyService _privacyPolicyService;

        public HomeController(
            ILogger<HomeController> logger, 
            IEventService eventService,
            IPrivacyPolicyService privacyPolicyService)
        {
            _logger = logger;
            _eventService = eventService;
            _privacyPolicyService = privacyPolicyService;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _eventService.GetAllEventsAsync();

            // Get published events, ordered by date
            var publishedEvents = events
                .Where(e => e.EventStatus == Domain.Events.EventStatus.Published)
                .OrderBy(e => e.StartDateTime)
                .ToList();

            // Map to view model
            var viewModel = new HomeIndexViewModel();

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

            return View(viewModel);
        }

        // TODO: Keep about page content static or delegate to service if dynamic content is needed.
        public IActionResult About()
        {
            return View();
        }

        // TODO: Keep privacy content generation inside the service layer.
        public async Task<IActionResult> Privacy()
        {
            var privacyPolicy = await _privacyPolicyService.GetAsync();
            return View(privacyPolicy.ToViewModel());
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
    }
}
