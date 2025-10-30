using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Depend on a gallery service for photo and album management.
    public class GalleryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Details(int id)
        {
            // TODO: Load gallery album details from service
            return View();
        }
        
        // TODO: Implement thin actions that coordinate gallery workflows via the service.
    }
}

