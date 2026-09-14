using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs.user.Profile;

namespace Tajawul.Interfaces.User.Profile
{
    public interface IMaritalStatusService
    {
        Task<MaritalStatus?> CreateMaritalStatusAsync(MaritalStatusDto maritalStatus);
        Task<bool?> DeleteMaritalStatusAsync(string maritalStatusName);
        Task<MaritalStatus?> GetMaritalStatusByNameAsync(string maritalStatusName);
        Task<List<MaritalStatus>> GetAllMaritalStatusAsync();
        Task<MaritalStatus?> UpdateMaritalStatusNameAsync(string oldName, string newName);
        Task<int> AddUserMaritalStatusAsync(string userId, string maritalStatusName);
        // Task<bool> DeleteUserMaritalStatusAsync(string userId);
        // Task<MaritalStatus?> GetUserMaritalStatusAsync(string userId);

    }
}
