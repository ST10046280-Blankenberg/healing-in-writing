using System;
using System.Threading.Tasks;
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
        private readonly IBankDetailsService _bankDetailsService;

        public SiteSettingsController(IBankDetailsService bankDetailsService)
        {
            _bankDetailsService = bankDetailsService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var entity = await _bankDetailsService.GetAsync();
            var viewModel = entity.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(BankDetailsViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", vm);
            }

            try
            {
                var entity = vm.ToEntity();
                await _bankDetailsService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                
                TempData["Success"] = "Bank details updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                return View("Index", vm);
            }
        }
    }
}