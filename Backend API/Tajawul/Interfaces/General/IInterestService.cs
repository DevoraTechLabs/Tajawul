using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs.General;

namespace Tajawul.Interfaces.General
{

    public interface IInterestService
    {
        Task<Interest?> CreateInterestAsync(InterestDto interestDto);
        Task<bool?> DeleteInterestAsync(string interestName);
        Task<Interest?> GetInterestByNameAsync(string interestName);
        Task<List<Interest>> GetAllInterestsAsync();
        Task<Interest?> UpdateInterestNameAsync(string oldName, string newName);
        Task<List<string>?> AddUserInterestAsync(string userId, List<string> interestsNames);
        Task<bool> DeleteUserInterestAsync(string userId, string interestName);
        Task<List<Interest>> GetUserInterestsAsync(string userId);
    }
}


