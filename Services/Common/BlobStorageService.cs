using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HealingInWriting.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace HealingInWriting.Services.Common
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobContainerClient? _containerClient;
        private readonly ILogger<BlobStorageService> _logger;
        private readonly bool _isAvailable;

        public BlobStorageService(
            ILogger<BlobStorageService> logger,
            BlobContainerClient? containerClient = null)
        {
            _logger = logger;
            _containerClient = containerClient;
            _isAvailable = containerClient != null;

            if (!_isAvailable)
            {
                _logger.LogWarning("BlobStorageService initialized without BlobContainerClient. Blob storage features will be unavailable.");
            }
        }

        public bool IsBlobStorageAvailable()
        {
            return _isAvailable;
        }

        public async Task<string> UploadImageAsync(IFormFile file, string containerPath = "")
        {
            if (!_isAvailable || _containerClient == null)
            {
                throw new InvalidOperationException("Blob storage is not configured. Please configure ConnectionStrings:StorageConnection in your settings.");
            }

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.", nameof(file));
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"File type '{fileExtension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
            }

            // Validate file size (5MB max)
            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB.");
            }

            try
            {
                // Generate unique blob name
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var blobName = string.IsNullOrEmpty(containerPath)
                    ? fileName
                    : $"{containerPath.TrimEnd('/')}/{fileName}";

                var blobClient = _containerClient.GetBlobClient(blobName);

                // Set content type based on extension
                var contentType = fileExtension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    _ => "application/octet-stream"
                };

                // Upload the file
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType,
                    CacheControl = "public, max-age=31536000" // Cache for 1 year
                };

                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                _logger.LogInformation("Successfully uploaded blob: {BlobName}", blobName);

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to blob storage: {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string blobUrl)
        {
            if (!_isAvailable || _containerClient == null)
            {
                _logger.LogWarning("Attempted to delete blob but storage is not configured: {BlobUrl}", blobUrl);
                return false;
            }

            if (string.IsNullOrWhiteSpace(blobUrl))
            {
                throw new ArgumentException("Blob URL cannot be null or empty.", nameof(blobUrl));
            }

            try
            {
                // Extract blob name from URL
                var uri = new Uri(blobUrl);
                var blobName = uri.Segments.Skip(2).Aggregate((a, b) => a + b).TrimStart('/');

                var blobClient = _containerClient.GetBlobClient(blobName);
                var response = await blobClient.DeleteIfExistsAsync();

                if (response.Value)
                {
                    _logger.LogInformation("Successfully deleted blob: {BlobName}", blobName);
                }
                else
                {
                    _logger.LogWarning("Blob not found for deletion: {BlobName}", blobName);
                }

                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting blob from storage: {BlobUrl}", blobUrl);
                return false;
            }
        }

        public async Task<string> GetBlobUrlAsync(string blobName, string containerPath = "")
        {
            if (!_isAvailable || _containerClient == null)
            {
                throw new InvalidOperationException("Blob storage is not configured.");
            }

            var fullBlobName = string.IsNullOrEmpty(containerPath)
                ? blobName
                : $"{containerPath.TrimEnd('/')}/{blobName}";

            var blobClient = _containerClient.GetBlobClient(fullBlobName);

            // Optionally verify the blob exists
            if (await blobClient.ExistsAsync())
            {
                return blobClient.Uri.ToString();
            }

            throw new FileNotFoundException($"Blob not found: {fullBlobName}");
        }
    }
}