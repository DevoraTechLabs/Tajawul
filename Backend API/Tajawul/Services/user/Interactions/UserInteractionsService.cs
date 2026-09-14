using Tajawul.Interfaces.User.Interactions;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.user;
using Tajawul.Models.ViewModels.user.UserInteractions;
using Tajawul.Repositories.User.Interaction;

namespace Tajawul.Services.User.Profile.Interactions
{
    public class UserInteractionsService(UserInteractionsRepository userInteractionsRepository) : IUserInteractionsService
    {
        private readonly UserInteractionsRepository userInteractionsRepository = userInteractionsRepository;

        public async Task<PaginatedResultDto<UserBasicInfoDto>?> GetFollowersAsync(string userId, int pageNumber, int pageSize)
        {
            return await userInteractionsRepository.GetFollowersAsync(userId, pageNumber, pageSize);
        }

        public async Task<PaginatedResultDto<UserBasicInfoDto>?> GetFollowingsAsync(string userId, int pageNumber, int pageSize)
        {
            return await userInteractionsRepository.GetFollowingsAsync(userId, pageNumber, pageSize);
        }

        public async Task<FollowToggleResult?> ToggleFollowUserAsync(string followedId, string userId)
        {
            if (followedId == userId) return null;
            return await userInteractionsRepository.ToggleFollowUserAsync(followedId, userId);
        }
    }
}
