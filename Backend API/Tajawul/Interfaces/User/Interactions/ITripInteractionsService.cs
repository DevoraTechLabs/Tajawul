using Tajawul.Models.ViewModels.Trip;

namespace Tajawul.Interfaces.User.Interactions
{
    public interface ITripInteractionsService
    {
        Task<UserTripStatus> GetUserStatusAsync(string tripId, string userId);

        Task<int> WishTripAsync(string userId, string tripId);

        Task<int> UnwishTripAsync(string userId, string tripId);

        Task<int> FavoriteTripAsync(string userId, string tripId);

        Task<int> UnfavoriteTripAsync(string userId, string tripId);

    }
}
