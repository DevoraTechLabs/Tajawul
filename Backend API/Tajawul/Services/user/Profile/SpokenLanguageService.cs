using Tajawul.Interfaces.User.Profile;
using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs.user.Profile;
using Tajawul.Repositories;

namespace Tajawul.Services
{
    public class SpokenLanguageService : ISpokenLanguageService
    {
        private readonly SpokenLanguageRepository _spokenLanguageRepository;

        public SpokenLanguageService(SpokenLanguageRepository spokenLanguageRepository)
        {
            _spokenLanguageRepository = spokenLanguageRepository ?? throw new ArgumentNullException(nameof(spokenLanguageRepository));
        }

        public async Task<SpokenLanguage?> CreateSpokenLanguageAsync(SpokenLanguageDto spokenLanguage)
        {
            return await _spokenLanguageRepository.CreateSpokenLanguageAsync(spokenLanguage);
        }

        public async Task<bool?> DeleteSpokenLanguageAsync(string spokenLanguageName)
        {
            var spokenLanguage = await _spokenLanguageRepository.GetSpokenLanguageByNameAsync(spokenLanguageName);
            if (spokenLanguage == null)
            {
                return null;
            }
            return await _spokenLanguageRepository.DeleteSpokenLanguageAsync(spokenLanguageName);
        }

        public async Task<SpokenLanguage?> GetSpokenLanguageByNameAsync(string spokenLanguageName)
        {
            return await _spokenLanguageRepository.GetSpokenLanguageByNameAsync(spokenLanguageName);
        }

        public async Task<List<SpokenLanguage>> GetAllSpokenLanguagesAsync()
        {
            return await _spokenLanguageRepository.GetAllSpokenLanguagesAsync();
        }

        public async Task<SpokenLanguage?> UpdateSpokenLanguageNameAsync(string oldName, string newName)
        {
            return await _spokenLanguageRepository.UpdateSpokenLanguageNameAsync(oldName, newName);
        }

        public async Task<(int relationshipsCreated, List<string> spokenLanguages)> AddUserSpokenLanguagesAsync(string userId, List<string> languages)
        {
            return await _spokenLanguageRepository.AddUserSpokenLanguagesAsync(userId, languages);
        }

    }
}
