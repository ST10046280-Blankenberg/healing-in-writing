using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Inject a volunteer service that governs enrolment and scheduling.
    public class VolunteerController : Controller
    {
        // GET: /Volunteer/Index
        public IActionResult Index()
        {
            return View();
        }
        
        // GET: /Volunteer/LogHours
        public IActionResult LogHours()
        {
            return View();
        }
        
        // GET: /Volunteer/MyEvents
        public IActionResult MyEvents()
        {
            return View();
        }
        
        // GET: /Volunteer/MyStories
        public IActionResult MyStories()
        {
            return View();
        }
        
        // TODO: Expose volunteer endpoints that just orchestrate calls to the service.
    }
}
