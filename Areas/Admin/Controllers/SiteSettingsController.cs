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
            var bankDetails = await _bankDetailsService.GetAsync();
            var privacyPolicy = await _privacyPolicyService.GetAsync();
            var model = new SiteSettingsViewModel
            {
                BankDetails = bankDetails.ToViewModel(),
                PrivacyPolicy = privacyPolicy.ToViewModel()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(SiteSettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch privacy policy to maintain view model integrity
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                model.PrivacyPolicy = privacyPolicy.ToViewModel();
                return View("Index", model);
            }

            try
            {
                var entity = model.BankDetails.ToEntity();
                await _bankDetailsService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                
                TempData["Success"] = "Bank details updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                // Re-fetch privacy policy to maintain view model integrity
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                model.PrivacyPolicy = privacyPolicy.ToViewModel();
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrivacyPolicy(PrivacyPolicyViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch bank details for the view model
                var bankDetails = await _bankDetailsService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = vm
                };
                return View("Index", model);
            }

            try
            {
                var entity = vm.ToEntity();
                await _privacyPolicyService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                TempData["PrivacySuccess"] = "Privacy policy updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving. Please try again.");
                var bankDetails = await _bankDetailsService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = vm
                };
                return View("Index", model);
            }
        }
    }

    public class SiteSettingsViewModel
    {
        public BankDetailsViewModel BankDetails { get; set; }
        public PrivacyPolicyViewModel PrivacyPolicy { get; set; }
    }
}