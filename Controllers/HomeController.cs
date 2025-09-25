using System.Diagnostics;
using HealingInWriting.Models;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Wire up a dedicated home service so this controller only orchestrates calls.
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // TODO: Delegate home page data retrieval to the home service once available.
        public IActionResult Index()
        {
            return View();
        }

        // TODO: Keep privacy content generation inside the service layer.
        public IActionResult Privacy()
        {
            return View();
        }

        // TODO: Let the service surface diagnostics while the controller returns the view.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
