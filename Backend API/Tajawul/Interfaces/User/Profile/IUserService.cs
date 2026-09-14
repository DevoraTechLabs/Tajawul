using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs;
using Tajawul.Models.ViewModels.user;

namespace Tajawul.Interfaces.User.Profile
{
    public interface IUserService
    {

        Task<UserModel?> GetUserProfileAsync(string userId);
        Task<(UserModel, List<string>)> UpdateUserInfoAsync(string userId, UpdateUserProfileDto userProfileDto);
        Task<(UserModel, List<string>)> UpdateUserInterestsAsync(string userId, UserInterests updatedInterests);
        Task<string> UpdateProfileImageAsync(IFormFile file, string userId);
        Task<UserInfoDto> GetUserInfoAsync(string userId);
    }
}
