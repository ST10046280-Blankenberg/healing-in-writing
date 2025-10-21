using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Inject a stories service to encapsulate storytelling business rules.
    public class StoriesController : Controller
    {
        // GET: Stories/Index
        public IActionResult Index()
        {
            // TODO: Replace with actual story data from service/repository
            return View();
        }
        
        // TODO: Keep actions minimal, delegating to stories service methods.
    }
}
