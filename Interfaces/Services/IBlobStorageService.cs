using Microsoft.AspNetCore.Http;

namespace HealingInWriting.Interfaces.Services
{
    public interface IBlobStorageService
    {
        /// <summary>
        /// Uploads an image file to Azure Blob Storage
        /// </summary>
        /// <param name="file">The image file to upload</param>
        /// <param name="containerPath">Optional subfolder path within the container (e.g., "gallery", "events", "profiles")</param>
        /// <param name="isPublic">If true, uploads to public container; if false, uploads to private container</param>
        /// <returns>The full URL to the uploaded blob (with SAS token if private)</returns>
        Task<string> UploadImageAsync(IFormFile file, string containerPath = "", bool isPublic = true);

        /// <summary>
        /// Deletes an image from Azure Blob Storage
        /// </summary>
        /// <param name="blobUrl">The full URL of the blob to delete</param>
        /// <param name="isPublic">If true, deletes from public container; if false, deletes from private container</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteImageAsync(string blobUrl, bool isPublic = true);

        /// <summary>
        /// Gets the full URL for a blob by its name (with SAS token if private)
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="containerPath">Optional subfolder path within the container</param>
        /// <param name="isPublic">If true, gets URL from public container; if false, generates SAS token for private container</param>
        /// <param name="expiryHours">Hours until SAS token expires (default 1 hour, only applies to private blobs)</param>
        /// <returns>The full URL to the blob (with SAS token if private)</returns>
        Task<string> GetBlobUrlAsync(string blobName, string containerPath = "", bool isPublic = true, int expiryHours = 1);

        /// <summary>
        /// Checks if blob storage is configured and available
        /// </summary>
        /// <returns>True if blob storage is available, false if using fallback (local storage)</returns>
        bool IsBlobStorageAvailable();

        /// <summary>
        /// Generates a SAS token URL for an existing blob URL
        /// </summary>
        /// <param name="blobUrl">The existing blob URL</param>
        /// <param name="expiryHours">Hours until SAS token expires (default 1 hour)</param>
        /// <returns>The blob URL with a SAS token appended</returns>
        string GenerateSasUrl(string blobUrl, int expiryHours = 1);
    }
}
