using Tajawul.Models.DTOs.Destination;
using Tajawul.Helpers.Filters;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Interfaces.Destinations;
using Tajawul.Repositories.Destinations;
using Tajawul.Interfaces.Media;
using Tajawul.Models.Domain.Uploads;
using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.Destinations;


namespace Tajawul.Services.Destinations
{
    public class DestinationService : IDestinationService
    {

        private readonly DestinationRepository _destinationRepository;
        private readonly IPriceRangeService _priceRangeService;
        private readonly ITypeService _typeService;
        private readonly IOpenCloseService _openCloseService;
        private readonly IDLocationService _dLocationService;
        private readonly IAzureStorageService _azureStorageService;


        public DestinationService(DestinationRepository destinationRepository, IPriceRangeService priceRangeService,
            ITypeService typeService, IOpenCloseService openCloseService,
            IDLocationService dLocationService, 
            IAzureStorageService azureStorageService)
        {
            _destinationRepository = destinationRepository;
            _priceRangeService = priceRangeService;
            _typeService = typeService;
            _openCloseService = openCloseService;
            _dLocationService = dLocationService;
            _azureStorageService = azureStorageService;

        }

        public async Task<(Destination destination, List<string> failures)> CreateDestinationAsync(CreateDestinationDto destinationDto, string userId)
        {

            if (destinationDto.EstablishedAt.HasValue && destinationDto.EstablishedAt.Value > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("EstablishedAt cannot be in the future.");
            }

            List<string> failures = new();
            Destination destination = await _destinationRepository.CreateDestinationAsync(destinationDto, userId);

            try
            {
                var type = await _typeService.AssignTypeAsync(destinationDto.Type, destination.DestinationId, userId);
                destination.Type = type.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Type assignment failed: {ex.Message}");
            }

            try
            {
                var priceRange = await _priceRangeService.AssignPriceRangeAsync(destinationDto.PriceRange, destination.DestinationId, userId);
                destination.PriceRange = priceRange.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Price range assignment failed: {ex.Message}");
            }

            try
            {
                if (destinationDto.OpenTime != null && destinationDto.CloseTime != null)
                {
                    var time = await _openCloseService.UpdateOpenCloseTimesAsync(destination.DestinationId, destinationDto.OpenTime.Value, destinationDto.CloseTime.Value);
                    destination.OpenTime = time.OpenAt;
                    destination.CloseTime = time.CloseAt;
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Open/Close time update failed: {ex.Message}");
            }

            try
            {
                var city = await _dLocationService.AssignLocationAsync(destination.DestinationId, destinationDto.Country, destinationDto.City);
                destination.City = city.Name;
                destination.Country = city.Country.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Location assignment failed: {ex.Message}");
            }

            return (destination, failures);
        }

        public async Task<(Destination destination, List<string> failures)> UpdateDestinationAsync(UpdateDestinationDto destinationDto, string userId)
        {
            if (destinationDto.EstablishedAt.HasValue && destinationDto.EstablishedAt.Value > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("EstablishedAt cannot be in the future.");
            }

            List<string> failures = new();
            Destination destination =  await _destinationRepository.UpdateDestinationAsync(destinationDto, userId);

            try
            {
                var type = await _typeService.AssignTypeAsync(destinationDto.Type, destination.DestinationId, userId);
                destination.Type = type.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Type assignment failed: {ex.Message}");
            }

            try
            {
                var priceRange = await _priceRangeService.AssignPriceRangeAsync(destinationDto.PriceRange, destination.DestinationId, userId);
                destination.PriceRange = priceRange.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Price range assignment failed: {ex.Message}");
            }

            try
            {
                if (destinationDto.OpenTime != null && destinationDto.CloseTime != null)
                {
                    var time = await _openCloseService.UpdateOpenCloseTimesAsync(destination.DestinationId, destinationDto.OpenTime.Value, destinationDto.CloseTime.Value);
                    destination.OpenTime = time.OpenAt;
                    destination.CloseTime = time.CloseAt;
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Open/Close time update failed: {ex.Message}");
            }

            try
            {
                var city = await _dLocationService.AssignLocationAsync(destination.DestinationId, destinationDto.Country, destinationDto.City);
                destination.City = city.Name;
                destination.Country = city.Country.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Location assignment failed: {ex.Message}");
            }

            return (destination, failures);
        }

        public async Task<List<Destination?>> GetDestinationsAsync(DestinationFilter destinationFilter)
        {

            if (destinationFilter.CreationDate.HasValue && destinationFilter.CreationDate.Value > DateTime.Now)
            {
                throw new ArgumentException("CreationDateStart cannot be in the future.");
            }
            if (destinationFilter.LastEditDate.HasValue && destinationFilter.LastEditDate.Value > DateTime.Now)
            {
                throw new ArgumentException("LastEditDate cannot be in the future.");
            }
            if (destinationFilter.EstablishedAt.HasValue && destinationFilter.EstablishedAt.Value > DateOnly.FromDateTime(DateTime.Now))
            {
                throw new ArgumentException("EstablishedAt cannot be in the future.");
            }

            return await _destinationRepository.GetDestinationsAsync(destinationFilter);
        }

        public async Task<List<DestinationUserDto>> GetDestinationUsersAsync(DestinationUsersFilter usersFilter, string destinationId)
        {
            return await _destinationRepository.GetDestinationUsersAsync(usersFilter, destinationId);
        }

        public async Task<string> UpdateDestinationCoverImageAsync(IFormFile file, string destinationId, string userId)
        {
            string imageUrl;

            try
            {
                imageUrl = await _azureStorageService.UploadImageAsync(file, "destinations", destinationId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload image to storage.", ex);
            }

            try
            {
                await _destinationRepository.UpdateDestinationCoverImageAsync(imageUrl, destinationId, userId);
                return imageUrl;
            }
            catch (Exception ex)
            {
                try
                {
                    await _azureStorageService.DeleteImagesAsync([imageUrl]);
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete the file.", ex);
                }
                throw new Exception("Failed to update destination cover image.", ex);
            }
        }

        public async Task<ImageUploadResult> UpdateDestinationImagesAsync(List<IFormFile> imageFiles, string destinationId, string userId)
        {

            var existingImageCount = await _destinationRepository.GetImageCountForDestinationAsync(destinationId);
            int newImageCount = imageFiles.Count;

            if (existingImageCount + newImageCount > 5)
                throw new InvalidOperationException($"Adding {newImageCount} image(s) exceeds the limit of 5 per destination. Currently has {existingImageCount}.");

            ImageUploadResult uploadResults;

            try
            {
                uploadResults = await _azureStorageService.UploadImagesAsync(imageFiles, "destinations", destinationId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload images to storage.", ex);
            }
            try
            {
                await _destinationRepository.UpdateDestinationImagesAsync(uploadResults.Success, destinationId, userId);
                return uploadResults;
            }
            catch (Exception ex)
            {
                try
                {
                    await _azureStorageService.DeleteImagesAsync(uploadResults.Success);
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete the file.", ex);
                }
                throw new Exception("Failed to update destination images.", ex);
            }
        }

        public async Task<ImageUploadResult> DeleteDestinationImagesAsync(List<string> imageUrls, string destinationId, string userId)
        {
            if (imageUrls == null || !imageUrls.Any())
            {
                throw new ArgumentException("No image URLs provided for deletion.", nameof(imageUrls));
            }

            // Validate all URLs belong to the given destination
            var invalidUrls = imageUrls
                .Where(url => !url.Contains($"/{destinationId}/"))
                .ToList();

            if (invalidUrls.Any())
            {
                throw new ArgumentException($"Some URLs do not belong to the given destination: {string.Join(", ", invalidUrls)}");
            }

            try
            {
                var deletionResult = await _azureStorageService.DeleteImagesAsync(imageUrls);

                if (deletionResult.Success.Any())
                {
                    var dbUpdateSuccess = await _destinationRepository.DeleteDestinationImagesAsync(deletionResult.Success, destinationId, userId);

                    if (!dbUpdateSuccess)
                    {
                        throw new Exception("Image URLs deleted from storage but failed to update database.");
                    }
                }

                return deletionResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete destination images.", ex);
            }
        }

    }
}