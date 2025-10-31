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
        private readonly IPrivacyPolicyService _privacyPolicyService;

        public SiteSettingsController(
            IBankDetailsService bankDetailsService,
            IPrivacyPolicyService privacyPolicyService)
        {
            _bankDetailsService = bankDetailsService;
            _privacyPolicyService = privacyPolicyService;
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

        // Privacy Policy Management (separate from BankDetails)
        [HttpGet]
        public async Task<IActionResult> PrivacyPolicy()
        {
            var entity = await _privacyPolicyService.GetAsync();
            var viewModel = entity.ToViewModel();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrivacyPolicy(PrivacyPolicyViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                return View("PrivacyPolicy", vm);
            }

            try
            {
                var entity = vm.ToEntity();
                await _privacyPolicyService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                
                TempData["PrivacySuccess"] = "Privacy policy updated successfully.";
                return RedirectToAction("PrivacyPolicy");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                return View("PrivacyPolicy", vm);
            }
        }
    }
}