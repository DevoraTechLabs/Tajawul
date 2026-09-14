using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs.user.Profile;

namespace Tajawul.Interfaces.User.Profile
{
    public interface ISpokenLanguageService
    {
        Task<SpokenLanguage?> CreateSpokenLanguageAsync(SpokenLanguageDto spokenLanguage);
        Task<bool?> DeleteSpokenLanguageAsync(string spokenLanguageName);
        Task<SpokenLanguage?> GetSpokenLanguageByNameAsync(string spokenLanguageName);
        Task<List<SpokenLanguage>> GetAllSpokenLanguagesAsync();
        Task<SpokenLanguage?> UpdateSpokenLanguageNameAsync(string oldName, string newName);
        Task<(int relationshipsCreated, List<string> spokenLanguages)> AddUserSpokenLanguagesAsync(string userId, List<string> languages);
        // Task<bool> DeleteUserSpokenLanguageAsync(string userId, string spokenLanguageName);
        // Task<List<SpokenLanguage>> GetUserSpokenLanguagesAsync(string userId);
    }
}
