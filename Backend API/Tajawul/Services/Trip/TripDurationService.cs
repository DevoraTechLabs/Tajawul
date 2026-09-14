using Tajawul.Interfaces.Trips;
using Tajawul.Models.Domain.Trips;
using Tajawul.Models.DTOs.Trip;
using Tajawul.Repositories;

namespace Tajawul.Services
{
    public class TripDurationService : ITripDurationService
    {

        private readonly TripDurationRepository _tripDurationRepository;

        public TripDurationService(TripDurationRepository tripDurationRepository)
        {
            _tripDurationRepository = tripDurationRepository;
        }

        public async Task<TripDuration?> GetTripDurationByNameAsync(string name)
        {
            return await _tripDurationRepository.GetTripDurationByNameAsync(name);
        }

        public async Task<TripDuration?> CreateTripDurationAsync(TripDurationDto tripDuration)
        {
            var existingMaritalStatus = await _tripDurationRepository.GetTripDurationByNameAsync(tripDuration.Name);
            if (existingMaritalStatus != null)
                return null;


            return await _tripDurationRepository.CreateTripDurationAsync(tripDuration.Name);

        }

        public async Task<bool?> DeleteTripDurationAsync(string name)
        {
            var existingMaritalStatus = await _tripDurationRepository.GetTripDurationByNameAsync(name);
            if (existingMaritalStatus == null)
                return null;

            return await _tripDurationRepository.DeleteTripDurationAsync(name);

        }

        public async Task<TripDuration?> UpdateTripDurationNameAsync(string oldeName, string newName)
        {
            var existingMaritalStatus = await _tripDurationRepository.GetTripDurationByNameAsync(oldeName);
            if (existingMaritalStatus == null)
                return null;

            return await _tripDurationRepository.UpdateTripDurationNameAsync(oldeName, newName);
        }

        public async Task<List<TripDuration>> GetAllTripDurationAsync()
        {
            return await _tripDurationRepository.GetAllTripDurationsAsync();
        }

        public async Task<TripDuration> AssignTripDurationAsync(string tripId, string durationName, string userId)
        {
            return await _tripDurationRepository.AssignTripDurationAsync(tripId, durationName, userId);
        }

        public async Task<TripDuration> GetTripDurationAsync(string tripId)
        {
            return await _tripDurationRepository.GetTripDurationAsync(tripId);
        }

        public async Task<string> RemoveTripDurationAsync(string tripId)
        {
            return await _tripDurationRepository.RemoveTripDurationAsync(tripId);
        }

    }

}