using Tajawul.Models.Domain.Trips;
using Tajawul.Models.DTOs.Trip;

namespace Tajawul.Interfaces.Trips
{
    public interface ITripDurationService
    {

        Task<TripDuration?> CreateTripDurationAsync(TripDurationDto TripDuration);

        Task<bool?> DeleteTripDurationAsync(string name);

        Task<TripDuration?> GetTripDurationByNameAsync(string name);

        Task<List<TripDuration>> GetAllTripDurationAsync();

        Task<TripDuration?> UpdateTripDurationNameAsync(string oldeName, string newName);

        Task<TripDuration> AssignTripDurationAsync(string tripId, string durationName, string userId);

        Task<TripDuration> GetTripDurationAsync(string tripId);

        Task<string> RemoveTripDurationAsync(string tripId);
    }
}