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
        /// <returns>The full URL to the uploaded blob</returns>
        Task<string> UploadImageAsync(IFormFile file, string containerPath = "");

        /// <summary>
        /// Deletes an image from Azure Blob Storage
        /// </summary>
        /// <param name="blobUrl">The full URL of the blob to delete</param>
        /// <returns>True if deletion was successful, false otherwise</returns>
        Task<bool> DeleteImageAsync(string blobUrl);

        /// <summary>
        /// Gets the full URL for a blob by its name
        /// </summary>
        /// <param name="blobName">The name of the blob</param>
        /// <param name="containerPath">Optional subfolder path within the container</param>
        /// <returns>The full URL to the blob</returns>
        Task<string> GetBlobUrlAsync(string blobName, string containerPath = "");

        /// <summary>
        /// Checks if blob storage is configured and available
        /// </summary>
        /// <returns>True if blob storage is available, false if using fallback (local storage)</returns>
        bool IsBlobStorageAvailable();
    }
}
