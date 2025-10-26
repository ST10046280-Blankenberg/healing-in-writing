using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Connect to a resources service that curates materials for the platform.
    public class ResourcesController : Controller
    {
        // TODO: Keep resource endpoints slim by routing through the service.
        public IActionResult Index()
        {
            return View();
        }
    }
}
