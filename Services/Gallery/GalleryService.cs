using HealingInWriting.Domain.Gallery;
using HealingInWriting.Interfaces.Services;
using HealingInWriting.Interfaces.Repository;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;

namespace HealingInWriting.Services.Gallery
{
    public class GalleryService : IGalleryService
    {
        private readonly IGalleryRepository _repository;
        private readonly IBlobStorageService _blobStorageService;

        public GalleryService(IGalleryRepository repository, IBlobStorageService blobStorageService)
        {
            _repository = repository;
            _blobStorageService = blobStorageService;
        }

        public async Task<GalleryItem?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task<List<GalleryItem>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<List<GalleryItem>> GetByCollectionIdAsync(string collectionId)
        {
            return await _repository.GetByCollectionIdAsync(collectionId);
        }

        public async Task<(List<GalleryItem> items, int totalCount)> GetPagedAsync(int page, int pageSize)
        {
            return await _repository.GetPagedAsync(page, pageSize);
        }

        public async Task<GalleryItem> AddAsync(GalleryItem entity, string uploadedBy)
        {
            entity.UploadedBy = uploadedBy;
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            return await _repository.AddAsync(entity);
        }

        public async Task UpdateAsync(GalleryItem entity, string updatedBy)
        {
            entity.UploadedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            await _repository.UpdateAsync(entity);
        }

        public async Task DeleteAsync(int id)
        {
            await _repository.DeleteAsync(id);
        }

        /// <summary>
        /// Uploads multiple gallery images with validation, blob storage upload, and database persistence.
        /// Handles error tracking for individual image uploads and returns aggregate results.
        /// </summary>
        public async Task<(int successCount, int failCount, string? lastError)> AddMultipleGalleryItemsAsync(
            List<IFormFile> images,
            string altText,
            bool isAlbum,
            int? albumPhotoCount,
            string? collectionId,
            string uploadedBy)
        {
            int successCount = 0;
            int failCount = 0;
            string? lastError = null;

            foreach (var image in images)
            {
                // Skip empty files
                if (image.Length == 0)
                {
                    continue;
                }

                try
                {
                    // Upload image to Azure Blob Storage (public container)
                    // BlobStorageService handles validation (file type, size, etc.)
                    var imageUrl = await _blobStorageService.UploadImageAsync(image, "gallery", isPublic: true);

                    // Create gallery item entity
                    var entity = new GalleryItem
                    {
                        ImageUrl = imageUrl,
                        AltText = altText, // Use same alt text for all images in batch
                        IsAlbum = isAlbum,
                        AlbumPhotoCount = albumPhotoCount,
                        CollectionId = !string.IsNullOrWhiteSpace(collectionId) ? collectionId : null,
                        CreatedDate = DateTime.UtcNow
                    };

                    // Persist to database
                    await AddAsync(entity, uploadedBy);
                    successCount++;
                }
                catch (Exception ex)
                {
                    failCount++;
                    lastError = ex.Message;
                }
            }

            return (successCount, failCount, lastError);
        }

        /// <summary>
        /// Deletes a gallery item including removing the associated image from blob storage or file system.
        /// Handles both Azure Blob Storage URLs and legacy local file paths for backwards compatibility.
        /// </summary>
        public async Task DeleteGalleryItemWithImageAsync(int id)
        {
            var item = await GetByIdAsync(id);

            if (item == null)
            {
                throw new InvalidOperationException("Gallery item not found.");
            }

            // Check if this is a blob storage URL or local file path
            if (item.ImageUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                item.ImageUrl.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                // Delete from Azure Blob Storage
                await _blobStorageService.DeleteImageAsync(item.ImageUrl, isPublic: true);
            }
            else
            {
                // Legacy: Delete physical file from disk (for old local images)
                var imagePath = item.ImageUrl.TrimStart('/');
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }

            // Delete from database
            await DeleteAsync(id);
        }
    }
}

