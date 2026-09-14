using Microsoft.AspNetCore.Mvc;
using Tajawul.Interfaces.User.Interactions;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Repositories.User.Interaction;

namespace Tajawul.Services.User.Profile.Interactions
{
    public class DestinationInteractionsService : IDestinationInteractionsService
    {

        private readonly DestinationInteractionsRepository _destinationInteractionsRepository;

        public DestinationInteractionsService(DestinationInteractionsRepository destinationInteractionsRepository)
        {
            _destinationInteractionsRepository = destinationInteractionsRepository;
        }

        public async Task<UserDestinationStatus> GetUserStatusAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.GetUserStatusAsync(destinationId, userId);
        }

        public async Task<int> FollowDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.FollowDestinationAsync(destinationId, userId);
        }

        public async Task<int> UnfollowDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.UnfollowDestinationAsync(destinationId, userId);
        }

        public async Task<int> VisitDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.VisitDestinationAsync(destinationId, userId);
        }

        public async Task<int> UnVisitDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.UnVisitDestinationAsync(destinationId, userId);
        }

        public async Task<int> WishDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.WishDestinationAsync(destinationId, userId);
        }

        public async Task<int> UnwishDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.UnwishDestinationAsync(destinationId, userId);
        }

        public async Task<int> FavoriteDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.FavoriteDestinationAsync(destinationId, userId);
        }

        public async Task<int> UnfavoriteDestinationAsync(string destinationId, string userId)
        {
            return await _destinationInteractionsRepository.UnfavoriteDestinationAsync(destinationId, userId);
        }

    }
}
