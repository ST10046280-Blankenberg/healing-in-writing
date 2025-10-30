using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Areas.Admin.Controllers

{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class HoursController : Controller
    {
        // GET: Admin/Reports/Index
        public IActionResult Index()
        {
            return View();
        }
    }
}

