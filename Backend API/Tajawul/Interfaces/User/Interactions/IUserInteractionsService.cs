using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.user;
using Tajawul.Models.ViewModels.user.UserInteractions;

namespace Tajawul.Interfaces.User.Interactions
{
    public interface IUserInteractionsService
    {
        Task<FollowToggleResult?> ToggleFollowUserAsync(string followedId, string userId);
        Task<PaginatedResultDto<UserBasicInfoDto>?> GetFollowersAsync(string userId, int pageNumber, int pageSize);
        Task<PaginatedResultDto<UserBasicInfoDto>?> GetFollowingsAsync(string userId, int pageNumber, int pageSize);
    }
}
