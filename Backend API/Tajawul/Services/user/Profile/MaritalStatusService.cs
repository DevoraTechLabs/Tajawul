using Tajawul.Interfaces.User.Profile;
using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs.user.Profile;
using Tajawul.Repositories;

namespace Tajawul.Services
{
    public class MaritalStatusService : IMaritalStatusService
    {
        private readonly MaritalStatusRepository _maritalStatusRepository;

        public MaritalStatusService(MaritalStatusRepository maritalStatusRepository)
        {
            _maritalStatusRepository = maritalStatusRepository ?? throw new ArgumentNullException(nameof(maritalStatusRepository));
        }

        public async Task<MaritalStatus?> CreateMaritalStatusAsync(MaritalStatusDto maritalStatus)
        {
            return await _maritalStatusRepository.CreateMaritalStatusAsync(maritalStatus);
        }

        public async Task<bool?> DeleteMaritalStatusAsync(string maritalStatusName)
        {

            var existingMaritalStatus = await _maritalStatusRepository.GetMaritalStatusByNameAsync(maritalStatusName);
            if (existingMaritalStatus == null)
                return null;

            return await _maritalStatusRepository.DeleteMaritalStatusAsync(maritalStatusName);

        }

        public async Task<MaritalStatus?> GetMaritalStatusByNameAsync(string maritalStatusName)
        {
            return await _maritalStatusRepository.GetMaritalStatusByNameAsync(maritalStatusName);
        }

        public async Task<List<MaritalStatus>> GetAllMaritalStatusAsync()
        {
            return await _maritalStatusRepository.GetAllMaritalStatusAsync();
        }

        public async Task<MaritalStatus?> UpdateMaritalStatusNameAsync(string oldName, string newName)
        {
            return await _maritalStatusRepository.UpdateMaritalStatusNameAsync(oldName, newName);
        }
        public async Task<int> AddUserMaritalStatusAsync(string userId, string maritalStatusName)
        {
            return await _maritalStatusRepository.AddUserMaritalStatusAsync(userId, maritalStatusName);
        }

        // public async Task<MaritalStatus?> GetUserMaritalStatusAsync(string userId)
        // {
        //     return await _maritalStatusRepository.GetUserMaritalStatusAsync(userId);
        // }

        // public async Task<bool> DeleteUserMaritalStatusAsync(string userId)
        // {
        //     return await _maritalStatusRepository.DeleteUserMaritalStatusAsync(userId);
        // }
    }
}
