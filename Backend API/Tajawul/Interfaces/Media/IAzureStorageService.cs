using Tajawul.Models.Domain.Uploads;

namespace Tajawul.Interfaces.Media
{
    public interface IAzureStorageService
    {
        public Task<string> UploadImageAsync(IFormFile imageFile, string entityType, string entityId);

        public Task<ImageUploadResult> UploadImagesAsync(List<IFormFile> imageFiles, string entityType, string entityId);

        public Task<ImageUploadResult> DeleteImagesAsync(List<string> imageUrls);
    }
}
