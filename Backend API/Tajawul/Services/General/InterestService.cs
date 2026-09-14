using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs.General;
using Tajawul.Repositories.General;

namespace Tajawul.Services
{
    public class InterestService : IInterestService
    {
        private readonly InterestRepository _interestRepository;

        public InterestService(InterestRepository interestRepository)
        {
            _interestRepository = interestRepository ?? throw new ArgumentNullException(nameof(interestRepository));
        }

        public async Task<Interest?> CreateInterestAsync(InterestDto interestDto)
        {

            return await _interestRepository.CreateInterestAsync(interestDto);
        }

        public async Task<bool?> DeleteInterestAsync(string name)
        {
            var interest = await _interestRepository.GetInterestByNameAsync(name);
            if (interest == null)
                return null;

            return await _interestRepository.DeleteInterestAsync(name);
        }

        public async Task<List<Interest>> GetAllInterestsAsync()
        {
            return await _interestRepository.GetAllInterestsAsync();
        }

        public async Task<Interest?> UpdateInterestNameAsync(string oldName, string newName)
        {
            return await _interestRepository.UpdateInterestNameAsync(oldName, newName);
        }

        public async Task<List<string>?> AddUserInterestAsync(string userId, List<string> interestsNames)
        {
            return await _interestRepository.AddUserInterestsAsync(userId, interestsNames);
        }

        public async Task<bool> DeleteUserInterestAsync(string userId, string interestName)
        {
            return await _interestRepository.DeleteUserInterestAsync(userId, interestName);
        }

        public async Task<List<Interest>> GetUserInterestsAsync(string userId)
        {
            return await _interestRepository.GetUserInterestsAsync(userId);
        }

        public async Task<Interest?> GetInterestByNameAsync(string interestName)
        {
            return await _interestRepository.GetInterestByNameAsync(interestName);
        }
    }

}
