using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Models.Common;
using HealingInWriting.Models.Gallery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace HealingInWriting.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class SiteSettingsController : Controller
    {
        private readonly IBankDetailsService _bankDetailsService;
        private readonly IPrivacyPolicyService _privacyPolicyService;
        private readonly IOurImpactService _ourImpactService;
        private readonly IGalleryService _galleryService;
        private readonly IBlobStorageService _blobStorageService;

        public SiteSettingsController(
            IBankDetailsService bankDetailsService,
            IPrivacyPolicyService privacyPolicyService,
            IOurImpactService ourImpactService,
            IGalleryService galleryService,
            IBlobStorageService blobStorageService)
        {
            _bankDetailsService = bankDetailsService;
            _privacyPolicyService = privacyPolicyService;
            _ourImpactService = ourImpactService;
            _galleryService = galleryService;
            _blobStorageService = blobStorageService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var bankDetails = await _bankDetailsService.GetAsync();
            var privacyPolicy = await _privacyPolicyService.GetAsync();
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGalleryItem(IFormFile image, string altText, bool isAlbum, int? albumPhotoCount, string collectionId)
        {
            if (image == null || image.Length == 0)
            {
                TempData["GalleryError"] = "Please select an image to upload.";
                return RedirectToAction("Index");
            }

            if (string.IsNullOrWhiteSpace(altText))
            {
                TempData["GalleryError"] = "Please provide alt text/description for the image.";
                return RedirectToAction("Index");
            }

            // Validate collection ID for albums
            if (isAlbum && string.IsNullOrWhiteSpace(collectionId))
            {
                TempData["GalleryError"] = "Album photos require a collection ID. Please select an existing collection or create a new one.";
                return RedirectToAction("Index");
            }

            try
            {
                // Upload image to Azure Blob Storage (public container)
                // BlobStorageService handles validation (file type, size, etc.)
                var imageUrl = await _blobStorageService.UploadImageAsync(image, "gallery", isPublic: true);

                var entity = new HealingInWriting.Domain.Gallery.GalleryItem
                {
                    ImageUrl = imageUrl,
                    AltText = altText,
                    IsAlbum = isAlbum,
                    AlbumPhotoCount = albumPhotoCount,
                    CollectionId = !string.IsNullOrWhiteSpace(collectionId) ? collectionId : null,
                    CreatedDate = DateTime.UtcNow
                };
                await _galleryService.AddAsync(entity, User.Identity?.Name ?? "System");
                TempData["GallerySuccess"] = "Photo added successfully.";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                // Validation errors from BlobStorageService
                TempData["GalleryError"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (InvalidOperationException ex)
            {
                // Blob storage not configured
                TempData["GalleryError"] = $"Storage error: {ex.Message}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["GalleryError"] = $"Error uploading photo: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGalleryItem(int id)
        {
            try
            {
                var item = await _galleryService.GetByIdAsync(id);
                if (item != null)
                {
                    // Check if this is a blob storage URL or local file path
                    if (item.ImageUrl.StartsWith("https://") || item.ImageUrl.StartsWith("http://"))
                    {
                        // Delete from Azure Blob Storage
                        await _blobStorageService.DeleteImageAsync(item.ImageUrl, isPublic: true);
                    }
                    else
                    {
                        // Legacy: Delete physical file from disk (for old local images)
                        var imagePath = item.ImageUrl.TrimStart('/');
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }

                    // Delete from database
                    await _galleryService.DeleteAsync(id);
                    TempData["GallerySuccess"] = "Photo deleted successfully.";
                }
                else
                {
                    TempData["GalleryError"] = "Photo not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["GalleryError"] = $"Error deleting photo: {ex.Message}";
            }

            return RedirectToAction("Index");
        }
    }
}