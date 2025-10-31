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
        private readonly IOurImpactService _ourImpactService;

        public SiteSettingsController(
            IBankDetailsService bankDetailsService,
            IPrivacyPolicyService privacyPolicyService,
            IOurImpactService ourImpactService)
        {
            _bankDetailsService = bankDetailsService;
            _privacyPolicyService = privacyPolicyService;
            _ourImpactService = ourImpactService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bankDetails = await _bankDetailsService.GetAsync();
            var privacyPolicy = await _privacyPolicyService.GetAsync();
            var ourImpact = await _ourImpactService.GetAsync();
            var model = new SiteSettingsViewModel
            {
                BankDetails = bankDetails.ToViewModel(),
                PrivacyPolicy = privacyPolicy.ToViewModel(),
                OurImpact = ourImpact.ToViewModel()
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
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetailsVm,
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    OurImpact = ourImpact.ToViewModel()
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
                // Re-fetch other data to maintain view model integrity
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetailsVm,
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    OurImpact = ourImpact.ToViewModel()
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
                // Re-fetch other data for the view model
                var bankDetails = await _bankDetailsService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicyVm,
                    OurImpact = ourImpact.ToViewModel()
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
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicyVm,
                    OurImpact = ourImpact.ToViewModel()
                };
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOurImpact([Bind(Prefix = "OurImpact")] OurImpactViewModel ourImpactVm)
        {
            if (!ModelState.IsValid)
            {
                // Re-fetch other data for the view model
                var bankDetails = await _bankDetailsService.GetAsync();
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    OurImpact = ourImpactVm
                };
                return View("Index", model);
            }

            try
            {
                var entity = ourImpactVm.ToEntity();
                await _ourImpactService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                TempData["OurImpactSuccess"] = "Our Impact updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                var bankDetails = await _bankDetailsService.GetAsync();
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    OurImpact = ourImpactVm
                };
                return View("Index", model);
            }
        }
    }
}