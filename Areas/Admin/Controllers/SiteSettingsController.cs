using System.Threading.Tasks;
using HealingInWriting.Domain.Common;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SiteSettingsController : Controller
    {
        private readonly IBankDetailsService _service;

        public SiteSettingsController(IBankDetailsService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entity = await _service.GetAsync();
            return View(entity.ToViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BankDetailsViewModel vm)
        {
            if (!ModelState.IsValid)
                return View("Index", vm);

            await _service.UpdateAsync(vm.ToEntity(), User.Identity.Name);
            TempData["Success"] = "Bank details updated.";
            return RedirectToAction("Index");
        }
    }
}