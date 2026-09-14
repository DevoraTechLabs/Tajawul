using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Tajawul.Interfaces.Media;
using Tajawul.Models.Domain.Uploads;

namespace Tajawul.Services.UploadService
{
    public class AzureStorageService: IAzureStorageService
    {
        private readonly string _connectionString;

        public AzureStorageService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureStorage:ConnectionString"]!;
        }

        public async Task<string> UploadImageAsync(IFormFile imageFile, string entityType, string entityId)
        {
            if (imageFile == null || imageFile.Length == 0)
                throw new ArgumentException("Image file is required", nameof(imageFile));

            // Ensure container name is lowercase (Azure requires lowercase container names)
            string containerName = entityType.ToLower();

            // Initialize container client
            var containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync();

            await foreach (var blobItem in containerClient.GetBlobsAsync(prefix: $"{entityId}/"))
            {
                var existingBlobClient = containerClient.GetBlobClient(blobItem.Name);
                await existingBlobClient.DeleteIfExistsAsync();
            }

            // Create a unique blob name using entityId + timestamp + file extension
            string fileExtension = Path.GetExtension(imageFile.FileName);
            string contentType = imageFile.ContentType;
            string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string blobName = $"{entityId}/{timestamp}{fileExtension}";

            // Get the blob client
            var blobClient = containerClient.GetBlobClient(blobName);

            // Upload with content type metadata
            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = contentType
            };

            // Upload the image
            using (var stream = imageFile.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, new BlobUploadOptions
                {
                    HttpHeaders = httpHeaders
                });
            }

            return blobClient.Uri.ToString();
        }

        public async Task<ImageUploadResult> UploadImagesAsync(List<IFormFile> imageFiles, string entityType, string entityId)
        {
            if (imageFiles == null || !imageFiles.Any())
                throw new ArgumentException("At least one image file is required.", nameof(imageFiles));

            string containerName = entityType.ToLower();
            var containerClient = new BlobContainerClient(_connectionString, containerName);
            await containerClient.CreateIfNotExistsAsync();

            var result = new ImageUploadResult();

            foreach (var imageFile in imageFiles)
            {
                if (imageFile.Length == 0)
                {
                    result.Failed.Add(imageFile.FileName);
                    continue;
                }

                try
                {
                    string fileExtension = Path.GetExtension(imageFile.FileName);
                    string contentType = imageFile.ContentType;
                    string timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
                    string blobName = $"{entityId}/gallery/{timestamp}{fileExtension}";

                    var blobClient = containerClient.GetBlobClient(blobName);

                    var httpHeaders = new BlobHttpHeaders
                    {
                        ContentType = contentType
                    };

                    using (var stream = imageFile.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobUploadOptions
                        {
                            HttpHeaders = httpHeaders
                        });
                    }

                    result.Success.Add(blobClient.Uri.ToString());
                }
                catch
                {
                    result.Failed.Add(imageFile.FileName);
                }
            }

            return result;
        }

        public async Task<ImageUploadResult> DeleteImagesAsync(List<string> imageUrls)
        {
            if (imageUrls == null || !imageUrls.Any())
                throw new ArgumentException("Image URL list is empty.", nameof(imageUrls));

            var result = new ImageUploadResult();

            foreach (var imageUrl in imageUrls)
            {
                try
                {
                    var uri = new Uri(imageUrl);
                    var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    if (segments.Length < 2)
                    {
                        result.Failed.Add(imageUrl);
                        continue;
                    }

                    string containerName = segments[0];
                    string blobName = string.Join('/', segments.Skip(1));

                    var containerClient = new BlobContainerClient(_connectionString, containerName);
                    var blobClient = containerClient.GetBlobClient(blobName);

                    var existsResponse = await blobClient.ExistsAsync();
                    if (!existsResponse.Value)
                    {
                        result.Failed.Add(imageUrl);
                        continue;
                    }

                    var deleteResponse = await blobClient.DeleteIfExistsAsync();

                    if (deleteResponse.Value)
                    {
                        result.Success.Add(imageUrl);
                    }
                    else
                    {
                        result.Failed.Add(imageUrl);
                    }
                }
                catch
                {
                    result.Failed.Add(imageUrl);
                }
            }

            return result;
        }

    }
}
