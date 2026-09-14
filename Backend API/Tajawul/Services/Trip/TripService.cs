using Tajawul.Interfaces.Trips;
using Tajawul.Models.DTOs.Trip;
using Tajawul.Repositories.Trips;
using Tajawul.Helpers.Filters;
using Tajawul.Models.ViewModels.Trip;
using Tajawul.Interfaces.Media;
using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.Trips;

namespace Tajawul.Services
{
    public class TripService : ITripService
    {
        private readonly TripRepository _tripRepository;
        private readonly IPriceRangeService _priceRangeService;
        private readonly IStatusService _statusService;
        private readonly ITripDurationService _tripDurationService;
        private readonly IVisibilityService _visibilityService;
        private readonly IAzureStorageService _azureStorageService;

        public TripService(
                TripRepository tripRepository,
                IPriceRangeService priceRangeService,
                IStatusService statusService,
                ITripDurationService tripDurationService,
                IVisibilityService visibilityService,
                IAzureStorageService azureStorageService)
        {
            _tripRepository = tripRepository;
            _priceRangeService = priceRangeService;
            _statusService = statusService;
            _tripDurationService = tripDurationService;
            _visibilityService = visibilityService;
            _azureStorageService = azureStorageService;
        }

        public async Task<(Trip trip, List<string> failures)> CreateTripAsync(CreateTripDto tripDto, string userId)
        {
            List<string> failures = new();
            Trip trip = await _tripRepository.CreateTripAsync(tripDto, userId);

            try
            {
                var priceRange = await _priceRangeService.AssignTripPriceRangeAsync(
                            tripDto.PriceRange,
                            trip.TripId,
                            userId);
                trip.PriceRange = priceRange.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign price range: {ex.Message}");
            }

            try
            {
                var status = await _statusService.AssignTripStatusAsync(
                    trip.TripId,
                    tripDto.Status,
                    userId
                );
                trip.Status = status.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign status: {ex.Message}");
            }

            try
            {
                var tripDuration = await _tripDurationService.AssignTripDurationAsync(
                    trip.TripId,
                    tripDto.TripDuration,
                    userId
                );
                trip.TripDuration = tripDuration.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign trip duration: {ex.Message}");
            }

            try
            {
                var visibility = await _visibilityService.AssignTripVisibilityAsync(
                   trip.TripId,
                   tripDto.Visibility,
                   userId
                );
                trip.Visibility = visibility.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign date range: {ex.Message}");
            }

            return (trip, failures);
        }

        public async Task<(Trip trip, List<string> failures)> UpdateTripAsync(UpdateTripDto tripDto, string userId)
        {
            List<string> failures = new();
            Trip trip = await _tripRepository.UpdateTripAsync(tripDto, userId);

            try
            {
                var priceRange = await _priceRangeService.AssignTripPriceRangeAsync(
                            tripDto.PriceRange,
                            trip.TripId,
                            userId);
                trip.PriceRange = priceRange.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign price range: {ex.Message}");
            }

            try
            {
                var status = await _statusService.AssignTripStatusAsync(
                    trip.TripId,
                    tripDto.Status,
                    userId
                );
                trip.Status = status.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign status: {ex.Message}");
            }

            try
            {
                var tripDuration = await _tripDurationService.AssignTripDurationAsync(
                    trip.TripId,
                    tripDto.TripDuration,
                    userId
                );
                trip.TripDuration = tripDuration.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign trip duration: {ex.Message}");
            }

            try
            {
                var visibility = await _visibilityService.AssignTripVisibilityAsync(
                   trip.TripId,
                   tripDto.Visibility,
                   userId
                );
                trip.Visibility = visibility.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign date range: {ex.Message}");
            }

            return (trip, failures);
        }

        public async Task<bool> DeleteTripAsync(string tripId, string userId)
        {
            return await _tripRepository.DeleteTripAsync(tripId, userId);
        }

        public async Task<List<TripDestination>> AssignDestinationToTripAsync(string userId, string tripId, string destinationId, TripDestinationDto tripDestinationDto)
        {
            try
            {
                var assigned = await _tripRepository.AssignDestinationAsync(userId, tripId, destinationId, tripDestinationDto.Day);

                var tripDestinations = await _tripRepository.GetTripDestinationsAsync(tripId);

                return tripDestinations;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> RemoveDestinationFromTripAsync(string userId, string tripId, string destinationId)
        {
            return await _tripRepository.RemoveDestinationFromTripAsync(userId,tripId, destinationId);
        }

        public async Task<List<TripDestination>> GetTripDestinationsAsync(string tripId)
        {
            return await _tripRepository.GetTripDestinationsAsync(tripId);
        }

        public async Task<List<Trip?>> GetTripsAsync(TripFilter tripFilter)
        {
            var trips = await _tripRepository.GetTripsAsync(tripFilter);

            if (tripFilter.TripId != null)
            {
                var tripDestinations = await _tripRepository.GetTripDestinationsAsync(tripFilter.TripId);
                if (trips.Any())
                {
                    trips[0].Destinations = tripDestinations;
                }
            }
            return trips;
        }

        public async Task<List<TripUserDto>> GetTripUsersAsync(TripUserFilter usersFilter, string tripId)
        {
            return await _tripRepository.GetTripUsersAsync(usersFilter, tripId);
        }

        public async Task<string> UpdateTripCoverImageAsync(IFormFile file, string tripId, string userId)
        {
            string imageUrl;

            try
            {
                imageUrl = await _azureStorageService.UploadImageAsync(file, "trips", tripId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload image to storage.", ex);
            }

            try
            {
                await _tripRepository.UpdateTripCoverImageAsync(imageUrl, tripId, userId);
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
                throw new Exception("Failed to update trip cover image.", ex);
            }
        }

        public async Task<(Trip trip, List<string> failures)> CloneTripAsync(string tripId, string userId)
        {
            List<string> failures = new();
            var trip = await _tripRepository.CloneAsync(tripId, userId);

            try
            {
                var status = await _statusService.AssignTripStatusAsync(
                    trip.TripId,
                    "Notstarted",
                    userId
                );
                trip.Status = status.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign status: {ex.Message}");
            }

            try
            {
                var visibility = await _visibilityService.AssignTripVisibilityAsync(
                   trip.TripId,
                   "Private",
                   userId
                );
                trip.Visibility = visibility.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign date range: {ex.Message}");
            }

            return (trip, failures);
        }
    }
}