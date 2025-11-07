using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        private readonly IReportService _reportService;

        public ReportsController(IReportService reportService)
        {
            _reportService = reportService;
        }

        // GET: Admin/Reports/Index
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] ReportsFilterViewModel filters)
        {
            var model = await _reportService.GetDashboardDataAsync(filters);
            return View(model);
        }
    }
}
