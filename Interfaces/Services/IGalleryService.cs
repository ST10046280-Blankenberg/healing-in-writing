using HealingInWriting.Domain.Gallery;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HealingInWriting.Interfaces.Services
{
    public interface IGalleryService
    {
        Task<GalleryItem?> GetByIdAsync(int id);
        Task<List<GalleryItem>> GetAllAsync();
        Task<List<GalleryItem>> GetByCollectionIdAsync(string collectionId);
        Task<(List<GalleryItem> items, int totalCount)> GetPagedAsync(int page, int pageSize);
        Task<GalleryItem> AddAsync(GalleryItem entity, string uploadedBy);
        Task UpdateAsync(GalleryItem entity, string updatedBy);
        Task DeleteAsync(int id);

        /// <summary>
        /// Uploads multiple gallery images with validation, blob storage upload, and database persistence.
        /// Returns a result indicating success count, failure count, and any error messages.
        /// </summary>
        Task<(int successCount, int failCount, string? lastError)> AddMultipleGalleryItemsAsync(
            List<IFormFile> images,
            string altText,
            bool isAlbum,
            int? albumPhotoCount,
            string? collectionId,
            string uploadedBy);

        /// <summary>
        /// Deletes a gallery item including removing the associated image from blob storage or file system.
        /// Handles both Azure Blob Storage URLs and legacy local file paths.
        /// </summary>
        Task DeleteGalleryItemWithImageAsync(int id);
    }
}

