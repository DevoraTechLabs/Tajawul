using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.Domain.Uploads;
using Tajawul.Models.DTOs.Destination;
using Tajawul.Models.ViewModels.Destination;

namespace Tajawul.Interfaces.Destinations
{
    public interface IDestinationService
    {
        Task<(Destination destination, List<string> failures)> CreateDestinationAsync(CreateDestinationDto destinationDto, string userId);

        Task<List<Destination?>> GetDestinationsAsync(DestinationFilter destinationFilter);

        Task<(Destination destination, List<string> failures)> UpdateDestinationAsync(UpdateDestinationDto destinationDto, string userId);

        Task<string> UpdateDestinationCoverImageAsync(IFormFile file, string destinationId, string userId);

        Task<ImageUploadResult> UpdateDestinationImagesAsync(List<IFormFile> imageFiles, string destinationId, string userId);

        Task<ImageUploadResult> DeleteDestinationImagesAsync(List<string> imageUrls, string destinationId, string userId);

        Task<List<DestinationUserDto>> GetDestinationUsersAsync(DestinationUsersFilter destinationFilter, string destinationId);

    }
}
