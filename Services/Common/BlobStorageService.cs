using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using HealingInWriting.Interfaces.Services;
using Microsoft.AspNetCore.Http;

namespace HealingInWriting.Services.Common
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient? _blobServiceClient;
        private readonly ILogger<BlobStorageService> _logger;
        private readonly IConfiguration _configuration;
        private readonly bool _isAvailable;
        private const string PublicContainerName = "uploads";
        private const string PrivateContainerName = "private";

        public BlobStorageService(
            ILogger<BlobStorageService> logger,
            IConfiguration configuration,
            BlobServiceClient? blobServiceClient = null)
        {
            _logger = logger;
            _configuration = configuration;
            _blobServiceClient = blobServiceClient;
            _isAvailable = blobServiceClient != null;

            if (!_isAvailable)
            {
                _logger.LogWarning("BlobStorageService initialized without BlobServiceClient. Blob storage features will be unavailable.");
            }
        }

        public bool IsBlobStorageAvailable()
        {
            return _isAvailable;
        }

        private BlobContainerClient GetContainerClient(bool isPublic)
        {
            if (_blobServiceClient == null)
            {
                throw new InvalidOperationException("Blob storage is not configured.");
            }

            var containerName = isPublic ? PublicContainerName : PrivateContainerName;
            return _blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task<string> UploadImageAsync(IFormFile file, string containerPath = "", bool isPublic = true)
        {
            if (!_isAvailable || _blobServiceClient == null)
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
                var containerClient = GetContainerClient(isPublic);

                // Generate unique blob name
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var blobName = string.IsNullOrEmpty(containerPath)
                    ? fileName
                    : $"{containerPath.TrimEnd('/')}/{fileName}";

                var blobClient = containerClient.GetBlobClient(blobName);

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
                    CacheControl = isPublic ? "public, max-age=31536000" : "private, max-age=3600"
                };

                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                _logger.LogInformation("Successfully uploaded blob to {Container}: {BlobName}",
                    isPublic ? "public" : "private", blobName);

                // For private blobs, return URL with SAS token
                if (!isPublic)
                {
                    return GenerateSasUrl(blobClient, expiryHours: 1);
                }

                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to blob storage: {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<string> UploadFileAsync(IFormFile file, string containerPath = "", bool isPublic = true)
        {
            if (!_isAvailable || _blobServiceClient == null)
            {
                throw new InvalidOperationException("Blob storage is not configured. Please configure ConnectionStrings:StorageConnection in your settings.");
            }

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File cannot be null or empty.", nameof(file));
            }

            // Validate file type - allow common document and image types only
            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", ".gif", ".webp", // Images
                ".pdf", ".doc", ".docx", ".xls", ".xlsx" // Documents
            };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new ArgumentException($"File type '{fileExtension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
            }

            // Validate file size (10MB max for documents)
            const long maxFileSize = 10 * 1024 * 1024;
            if (file.Length > maxFileSize)
            {
                throw new ArgumentException($"File size exceeds maximum allowed size of {maxFileSize / (1024 * 1024)}MB.");
            }

            try
            {
                var containerClient = GetContainerClient(isPublic);

                // Generate unique blob name
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var blobName = string.IsNullOrEmpty(containerPath)
                    ? fileName
                    : $"{containerPath.TrimEnd('/')}/{fileName}";

                var blobClient = containerClient.GetBlobClient(blobName);

                // Set content type based on extension
                var contentType = fileExtension switch
                {
                    ".jpg" or ".jpeg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".webp" => "image/webp",
                    ".pdf" => "application/pdf",
                    ".doc" => "application/msword",
                    ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                    ".xls" => "application/vnd.ms-excel",
                    ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    _ => "application/octet-stream"
                };

                // Upload the file
                var blobHttpHeaders = new BlobHttpHeaders
                {
                    ContentType = contentType,
                    CacheControl = isPublic ? "public, max-age=31536000" : "private, max-age=3600"
                };

                await using var stream = file.OpenReadStream();
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = blobHttpHeaders
                });

                _logger.LogInformation("Successfully uploaded file to {Container}: {BlobName}",
                    isPublic ? "public" : "private", blobName);

                // Return the blob URL without SAS token (will be generated on-demand when accessed)
                return blobClient.Uri.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file to blob storage: {FileName}", file.FileName);
                throw;
            }
        }

        public async Task<bool> DeleteImageAsync(string blobUrl, bool isPublic = true)
        {
            if (!_isAvailable || _blobServiceClient == null)
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
                var containerClient = GetContainerClient(isPublic);

                // Extract blob name from URL (remove query string if present - SAS token)
                var uri = new Uri(blobUrl.Split('?')[0]);
                var blobName = uri.Segments.Skip(2).Aggregate((a, b) => a + b).TrimStart('/');

                var blobClient = containerClient.GetBlobClient(blobName);
                var response = await blobClient.DeleteIfExistsAsync();

                if (response.Value)
                {
                    _logger.LogInformation("Successfully deleted blob from {Container}: {BlobName}",
                        isPublic ? "public" : "private", blobName);
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

        public async Task<string> GetBlobUrlAsync(string blobName, string containerPath = "", bool isPublic = true, int expiryHours = 1)
        {
            if (!_isAvailable || _blobServiceClient == null)
            {
                throw new InvalidOperationException("Blob storage is not configured.");
            }

            var containerClient = GetContainerClient(isPublic);

            var fullBlobName = string.IsNullOrEmpty(containerPath)
                ? blobName
                : $"{containerPath.TrimEnd('/')}/{blobName}";

            var blobClient = containerClient.GetBlobClient(fullBlobName);

            // Verify the blob exists
            if (!await blobClient.ExistsAsync())
            {
                throw new FileNotFoundException($"Blob not found: {fullBlobName}");
            }

            // For private blobs, generate SAS token
            if (!isPublic)
            {
                return GenerateSasUrl(blobClient, expiryHours);
            }

            return blobClient.Uri.ToString();
        }

        private string GenerateSasUrl(BlobClient blobClient, int expiryHours)
        {
            // Check if the client can generate SAS tokens
            if (!blobClient.CanGenerateSasUri)
            {
                _logger.LogError("BlobClient cannot generate SAS URI. Ensure you're using AccountKey authentication.");
                throw new InvalidOperationException("Cannot generate SAS token. Storage account key is required.");
            }

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = blobClient.BlobContainerName,
                BlobName = blobClient.Name,
                Resource = "b", // b = blob
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5), // Start 5 minutes ago to account for clock skew
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(expiryHours)
            };

            // Set read permissions
            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasUri = blobClient.GenerateSasUri(sasBuilder);
            return sasUri.ToString();
        }

        public string GenerateSasUrl(string blobUrl, int expiryHours = 1)
        {
            if (string.IsNullOrEmpty(blobUrl))
            {
                throw new ArgumentException("Blob URL cannot be null or empty", nameof(blobUrl));
            }

            // If blob storage is not configured, return the URL as-is
            if (!IsBlobStorageAvailable() || _blobServiceClient == null)
            {
                return blobUrl;
            }

            // Parse the blob URL to extract container and blob name
            var uri = new Uri(blobUrl);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length < 2)
            {
                throw new ArgumentException("Invalid blob URL format", nameof(blobUrl));
            }

            var containerName = segments[0];
            var blobName = string.Join("/", segments.Skip(1));

            // Get the appropriate container client
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);

            // Generate SAS token
            return GenerateSasUrl(blobClient, expiryHours);
        }
    }
}