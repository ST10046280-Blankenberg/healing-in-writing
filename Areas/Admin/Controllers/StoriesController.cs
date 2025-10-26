using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class StoriesController : Controller
    {
        // GET: Admin/Stories/Manage
        public IActionResult Manage()
        {
            return View();
        }
    }
}
