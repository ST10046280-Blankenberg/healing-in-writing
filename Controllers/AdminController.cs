using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Controllers
{
    // TODO: Inject an admin application service to handle privileged operations.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // TODO: Add actions that defer management tasks to the admin service layer.

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult ManageStories()
        {
            return View();
        }

        public IActionResult ManageEvents()
        {
            return View();
        }

        public IActionResult ManageBooks()
        {
            return View();
        }

        public IActionResult ManageHours()
        {
            return View();
        }

        public IActionResult Reports()
        {
            return View();
        }

        public IActionResult SiteSettings()
        {
            return View();
        }

        public IActionResult AddBookView()
        {
            return View();
        }
    }
}
