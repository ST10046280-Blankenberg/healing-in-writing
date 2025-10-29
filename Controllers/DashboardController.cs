using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        // GET: /Dashboard/Index
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Dashboard/LogHours
        // Only volunteers can log hours
        [Authorize(Roles = "Volunteer")]
        public IActionResult LogHours()
        {
            return View();
        }

        // GET: /Dashboard/MyEvents
        public IActionResult MyEvents()
        {
            return View();
        }

        // GET: /Dashboard/MyStories
        public IActionResult MyStories()
        {
            return View();
        }
    }
}
