using Microsoft.AspNetCore.Mvc;
using Tajawul.Models.ViewModels.Destination;

namespace Tajawul.Interfaces.User.Interactions
{
    public interface IDestinationInteractionsService
    {

        Task<UserDestinationStatus> GetUserStatusAsync(string destinationId, string userId);

        Task<int> FollowDestinationAsync(string destinationId, string userId);

        Task<int> UnfollowDestinationAsync(string destinationId, string userId);

        Task<int> VisitDestinationAsync(string destinationId, string userId);

        Task<int> UnVisitDestinationAsync(string destinationId, string userId);

        Task<int> WishDestinationAsync(string destinationId, string userId);

        Task<int> UnwishDestinationAsync(string destinationId, string userId);

        Task<int> FavoriteDestinationAsync(string destinationId, string userId);

        Task<int> UnfavoriteDestinationAsync(string destinationId, string userId);
    }
}
