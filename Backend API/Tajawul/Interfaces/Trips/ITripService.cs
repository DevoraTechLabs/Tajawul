using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Trips;
using Tajawul.Models.DTOs.Trip;
using Tajawul.Models.ViewModels.Trip;
using Tajawul.Repositories.Trips;

namespace Tajawul.Interfaces.Trips
{
    public interface ITripService
    {
        Task<(Trip trip, List<string> failures)> CreateTripAsync(CreateTripDto tripDto, string userId);

        Task<(Trip trip, List<string> failures)> UpdateTripAsync(UpdateTripDto tripDto, string userId);

        Task<bool> DeleteTripAsync(string tripId, string userId);

        Task<List<TripDestination>> AssignDestinationToTripAsync(string userId, string tripId, string destinationId, TripDestinationDto tripDestinationDto);

        Task<bool> RemoveDestinationFromTripAsync(string userId, string tripId, string destinationId);

        Task<List<TripDestination>> GetTripDestinationsAsync(string tripId);

        Task<List<Trip?>> GetTripsAsync(TripFilter TripFilter);

        Task<List<TripUserDto>> GetTripUsersAsync(TripUserFilter usersFilter, string tripId);

        Task<string> UpdateTripCoverImageAsync(IFormFile file, string tripId, string userId);

        Task<(Trip trip, List<string> failures)> CloneTripAsync(string tripId, string userId);


    }
}
