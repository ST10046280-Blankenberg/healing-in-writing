using System;
using System.Linq;
using System.Threading.Tasks;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Common;
using HealingInWriting.Models.Gallery;
using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SiteSettingsController : Controller
    {
        private static readonly HtmlSanitizer PolicySanitizer = BuildPolicySanitizer();

        private readonly IBankDetailsService _bankDetailsService;
        private readonly IPrivacyPolicyService _privacyPolicyService;
        private readonly ITermsOfServiceService _termsOfServiceService;
        private readonly IOurImpactService _ourImpactService;
        private readonly IGalleryService _galleryService;

        public SiteSettingsController(
            IBankDetailsService bankDetailsService,
            IPrivacyPolicyService privacyPolicyService,
            ITermsOfServiceService termsOfServiceService,
            IOurImpactService ourImpactService,
            IGalleryService galleryService)
        {
            _bankDetailsService = bankDetailsService;
            _privacyPolicyService = privacyPolicyService;
            _termsOfServiceService = termsOfServiceService;
            _ourImpactService = ourImpactService;
            _galleryService = galleryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bankDetails = await _bankDetailsService.GetAsync();
            var privacyPolicy = await _privacyPolicyService.GetAsync();
            var termsOfService = await _termsOfServiceService.GetAsync();
            var ourImpact = await _ourImpactService.GetAsync();
            var galleryItems = await _galleryService.GetAllAsync();

            // Get distinct existing collection IDs
            var existingCollections = galleryItems
                .Where(g => !string.IsNullOrWhiteSpace(g.CollectionId))
                .Select(g => g.CollectionId)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            ViewBag.ExistingCollections = existingCollections;

            var model = new SiteSettingsViewModel
            {
                BankDetails = bankDetails.ToViewModel(),
                PrivacyPolicy = privacyPolicy.ToViewModel(),
                TermsOfService = termsOfService.ToViewModel(),
                OurImpact = ourImpact.ToViewModel(),
                GalleryItems = galleryItems.Select(g => g.ToViewModel()).ToList()
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
                var termsOfService = await _termsOfServiceService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetailsVm,
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    TermsOfService = termsOfService.ToViewModel(),
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
                var termsOfService = await _termsOfServiceService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetailsVm,
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    TermsOfService = termsOfService.ToViewModel(),
                    OurImpact = ourImpact.ToViewModel()
                };
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePrivacyPolicy([Bind(Prefix = "PrivacyPolicy")] PrivacyPolicyViewModel privacyPolicyVm)
        {
            privacyPolicyVm.ContentFormat = Domain.Common.PolicyContentFormat.Template;

            if (privacyPolicyVm.TemplateData == null)
            {
                privacyPolicyVm.TemplateData = PolicyTemplateDefaults.CreatePrivacyDefaults();
            }

            PolicyTemplateSerializer.PopulateListsFromText(privacyPolicyVm.TemplateData);
            SanitizeTemplateData(privacyPolicyVm.TemplateData);
            privacyPolicyVm.Content = PolicyTemplateSerializer.Serialize(privacyPolicyVm.TemplateData);
            ModelState.Remove("PrivacyPolicy.Content");
            ModelState.Remove("Content");

            if (!ModelState.IsValid)
            {
                var bankDetails = await _bankDetailsService.GetAsync();
                var termsOfService = await _termsOfServiceService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicyVm,
                    TermsOfService = termsOfService.ToViewModel(),
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
                var termsOfService = await _termsOfServiceService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicyVm,
                    TermsOfService = termsOfService.ToViewModel(),
                    OurImpact = ourImpact.ToViewModel()
                };
                return View("Index", model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateTermsOfService([Bind(Prefix = "TermsOfService")] TermsOfServiceViewModel termsVm)
        {
            termsVm.ContentFormat = Domain.Common.PolicyContentFormat.Template;

            if (termsVm.TemplateData == null)
            {
                termsVm.TemplateData = PolicyTemplateDefaults.CreateTermsDefaults();
            }

            PolicyTemplateSerializer.PopulateListsFromText(termsVm.TemplateData);
            SanitizeTemplateData(termsVm.TemplateData);
            termsVm.Content = PolicyTemplateSerializer.Serialize(termsVm.TemplateData);
            ModelState.Remove("TermsOfService.Content");
            ModelState.Remove("Content");

            if (!ModelState.IsValid)
            {
                var bankDetails = await _bankDetailsService.GetAsync();
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    TermsOfService = termsVm,
                    OurImpact = ourImpact.ToViewModel()
                };
                return View("Index", model);
            }

            try
            {
                var entity = termsVm.ToEntity();
                await _termsOfServiceService.UpdateAsync(entity, User.Identity?.Name ?? "System");
                TempData["TermsSuccess"] = "Terms of service updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while saving: {ex.Message}");
                var bankDetails = await _bankDetailsService.GetAsync();
                var privacyPolicy = await _privacyPolicyService.GetAsync();
                var ourImpact = await _ourImpactService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    TermsOfService = termsVm,
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
                var termsOfService = await _termsOfServiceService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    TermsOfService = termsOfService.ToViewModel(),
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
                var termsOfService = await _termsOfServiceService.GetAsync();
                var model = new SiteSettingsViewModel
                {
                    BankDetails = bankDetails.ToViewModel(),
                    PrivacyPolicy = privacyPolicy.ToViewModel(),
                    TermsOfService = termsOfService.ToViewModel(),
                    OurImpact = ourImpactVm
                };
                return View("Index", model);
            }
        }

        /// <summary>
        /// Handles gallery image upload requests with validation and delegates processing to the gallery service.
        /// Validates inputs, calls the service layer, and provides user feedback via TempData.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGalleryItem(List<IFormFile> images, string altText, bool isAlbum, int? albumPhotoCount, string collectionId)
        {
            // Input validation
            if (images == null || images.Count == 0)
            {
                TempData["GalleryError"] = "Please select at least one image to upload.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(altText))
            {
                TempData["GalleryError"] = "Please provide alt text/description for the images.";
                return RedirectToAction("Index");
            }

            // Validate collection ID for albums
            if (isAlbum && string.IsNullOrWhiteSpace(collectionId))
            {
                TempData["GalleryError"] = "Album photos require a collection ID. Please select an existing collection or create a new one.";
                return RedirectToAction("Index");
            }

            // Delegate upload logic to service
            var (successCount, failCount, lastError) = await _galleryService.AddMultipleGalleryItemsAsync(
                images,
                altText,
                isAlbum,
                albumPhotoCount,
                collectionId,
                User.Identity?.Name ?? "System");

            // Provide user feedback
            if (successCount > 0)
            {
                TempData["GallerySuccess"] = $"{successCount} photo(s) added successfully.";
                if (failCount > 0)
                {
                    TempData["GalleryError"] = $"Failed to upload {failCount} photo(s). Last error: {lastError}";
                }
            }
            else
            {
                TempData["GalleryError"] = $"Failed to upload photos. Error: {lastError}";
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Handles gallery item deletion by delegating to the service layer.
        /// The service handles both blob storage and legacy file system cleanup.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryItem(int id)
        {
            try
            {
                // Delegate deletion logic to service (handles image and database cleanup)
                await _galleryService.DeleteGalleryItemWithImageAsync(id);
                TempData["GallerySuccess"] = "Photo deleted successfully.";
            }
            catch (InvalidOperationException ex)
            {
                // Item not found
                TempData["GalleryError"] = ex.Message;
            }
            catch (Exception ex)
            {
                TempData["GalleryError"] = $"Error deleting photo: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
        private static HtmlSanitizer BuildPolicySanitizer()
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedTags.Clear();
            sanitizer.AllowedTags.Add("p");
            sanitizer.AllowedTags.Add("strong");
            sanitizer.AllowedTags.Add("em");
            sanitizer.AllowedTags.Add("ul");
            sanitizer.AllowedTags.Add("li");
            sanitizer.AllowedTags.Add("a");
            sanitizer.AllowedTags.Add("br");
            sanitizer.AllowedAttributes.Clear();
            sanitizer.AllowedAttributes.Add("href");
            sanitizer.AllowedAttributes.Add("rel");
            sanitizer.AllowedAttributes.Add("target");
            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");
            sanitizer.AllowedSchemes.Add("mailto");
            return sanitizer;
        }

        private static void SanitizeTemplateData(PolicyTemplateData data)
        {
            data.IntroText = PolicySanitizer.Sanitize(data.IntroText ?? string.Empty);
            data.FooterText = PolicySanitizer.Sanitize(data.FooterText ?? string.Empty);

            if (data.Sections == null)
            {
                data.Sections = new List<PolicyTemplateSection>();
            }

            foreach (var section in data.Sections)
            {
                section.Body = PolicySanitizer.Sanitize(section.Body ?? string.Empty);
                section.HighlightBody = PolicySanitizer.Sanitize(section.HighlightBody ?? string.Empty);
            }
        }
    }
}
