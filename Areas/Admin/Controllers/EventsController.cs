using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class EventsController : Controller
    {
        // GET: Admin/Events/Manage
        public IActionResult Manage()
        {
            return View();
        }

        // GET: Admin/Events/Details
        public IActionResult Details()
        {
            return View();
        }
    }
}
