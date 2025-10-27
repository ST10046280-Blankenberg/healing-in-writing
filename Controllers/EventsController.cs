using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Depend on an events service for scheduling and registration logic.
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Details(int id)
        {
            // TODO: Load event details from service
            return View();
        }
        
        // TODO: Implement thin actions that coordinate event workflows via the service.
    }
}
