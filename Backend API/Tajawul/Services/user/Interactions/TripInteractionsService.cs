using Tajawul.Interfaces.User.Interactions;
using Tajawul.Models.ViewModels.Trip;
using Tajawul.Repositories.User.Interaction;

namespace Tajawul.Services.User.Interactions
{
    public class TripInteractionsService : ITripInteractionsService
    {
        private readonly TripInteractionsRepository _tripInteractionsRepository;

        public TripInteractionsService(TripInteractionsRepository tripInteractionsRepository)
        {
            _tripInteractionsRepository = tripInteractionsRepository;
        }

        public async Task<UserTripStatus> GetUserStatusAsync(string tripId, string userId)
        {
            return await _tripInteractionsRepository.GetUserStatusAsync(tripId, userId);
        }

        public async Task<int> WishTripAsync(string userId, string tripId)
        {
            return await _tripInteractionsRepository.WishTripAsync(userId, tripId);
        }

        public async Task<int> UnwishTripAsync(string userId, string tripId)
        {
            return await _tripInteractionsRepository.UnwishTripAsync(userId, tripId);
        }

        public async Task<int> FavoriteTripAsync(string userId, string tripId)
        {
            return await _tripInteractionsRepository.FavoriteTripAsync(userId, tripId);
        }

        public async Task<int> UnfavoriteTripAsync(string userId, string tripId)
        {
            return await _tripInteractionsRepository.UnfavoriteTripAsync(userId, tripId);
        }
    }
}
