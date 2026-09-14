using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Translation;
using Tajawul.Models.DTOs.user.Translation;

namespace Tajawul.Interfaces.User.Translation
{
    public interface ITranslationService
    {

        Task<TranslationItem?> AddTranslationItemAsync(TranslationItemDto translationItemDto, string userId);
        Task<(bool Success, bool IsFavorite)> MarkAsFavoriteAsync(string translationId, string userId);
        Task<List<TranslationItem>> GetHistoryAsync(string userId, TranslationFilter filter);
        Task<List<TranslationItem>> GetFavoritesAsync(string userId, TranslationFilter filter);
        Task<TranslationItem?> GetTranslationItemAsync(string translationId, string userId);
        Task<bool> DeleteTranslationItemAsync(string translationId, string userId);
        Task<bool?> ClearTranslationHistoryAsync(string userId);

        Task<TranslationItem?> TranslateTextAsync(TranslationItemDto translationItemDto);
    }
}
