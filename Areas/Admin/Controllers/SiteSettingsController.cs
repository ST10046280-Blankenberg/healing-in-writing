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
        public async Task<IActionResult> Update([Bind(Prefix = "BankDetails")] BankDetailsViewModel bankDetailsVm)
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch both to maintain view model integrity
                var bankDetails = await _bankDetailsService.GetAsync();
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetailsVm,
                    PrivacyPolicy = privacyPolicy.ToViewModel()
                };
                return View("Index", model);
            }

            try
            {
                var entity = bankDetailsVm.ToEntity();
                await _bankDetailsService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                
                TempData["Success"] = "Bank details updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                // Re-fetch privacy policy to maintain view model integrity
                var bankDetails = await _bankDetailsService.GetAsync();
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetailsVm,
                    PrivacyPolicy = privacyPolicy.ToViewModel()
                };
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrivacyPolicy([Bind(Prefix = "PrivacyPolicy")] PrivacyPolicyViewModel privacyPolicyVm)
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch bank details for the view model
                var bankDetails = await _bankDetailsService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicyVm
                };
                return View("Index", model);
            }

            try
            {
                var entity = privacyPolicyVm.ToEntity();
                await _privacyPolicyService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                TempData["PrivacySuccess"] = "Privacy policy updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                var bankDetails = await _bankDetailsService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicyVm
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