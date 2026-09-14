using MongoDB.Bson;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.User.Translation;
using Tajawul.Models.Domain.Translation;
using Tajawul.Models.DTOs.user.Translation;
using Tajawul.Repositories.User;

namespace Tajawul.Services.User.Translation
{
    public class TranslationService(TranslationRepository translationRepository, IMTService mtService) : ITranslationService
    {

        private readonly TranslationRepository _translationRepository = translationRepository;
        private readonly IMTService _mtService = mtService;


        public async Task<TranslationItem?> AddTranslationItemAsync(TranslationItemDto translationItemDto, string userId)
        {
            var translatedText = await _mtService.GetMTResponseAsync(translationItemDto.SourceText, translationItemDto.OutputLanguage, translationItemDto.InputLanguage);
            // var translatedText = "lkfjlkaj;";
            if (string.IsNullOrEmpty(translatedText))
                return null;

            var translationItem = new TranslationItem
            {
                TranslationId = ObjectId.GenerateNewId().ToString(),
                InputLanguage = translationItemDto.InputLanguage.ToLower(),
                OutputLanguage = translationItemDto.OutputLanguage.ToLower(),
                SourceText = translationItemDto.SourceText,
                TranslatedText = translatedText,
                CreatedAt = DateTime.UtcNow
            };

            var isAdded = await _translationRepository.AddTranslationAsync(translationItem, userId);

            if (isAdded == false)
                return null;


            return translationItem;

        }

        public async Task<List<TranslationItem>> GetFavoritesAsync(string userId, TranslationFilter filter)
        {
            return await _translationRepository.GetFavoritesAsync(userId, filter);
        }

        public async Task<List<TranslationItem>> GetHistoryAsync(string userId, TranslationFilter filter)
        {
            return await _translationRepository.GetUserTranslationHistoryAsync(userId, filter);
        }

        public async Task<(bool Success, bool IsFavorite)> MarkAsFavoriteAsync(string translationId, string userId)
        {
            return await _translationRepository.MarkAsFavoriteAsync(translationId, userId);
        }

        public async Task<bool> DeleteTranslationItemAsync(string translationId, string userId)
        {
            return await _translationRepository.DeleteTranslationItemAsync(translationId, userId);
        }
        public async Task<bool?> ClearTranslationHistoryAsync(string userId)
        {
            return await _translationRepository.ClearTranslationHistoryAsync(userId);
        }

        public Task<TranslationItem?> GetTranslationItemAsync(string translationId, string userId)
        {
            return _translationRepository.GetTranslationItemAsync(translationId, userId);
        }

        public async Task<TranslationItem?> TranslateTextAsync(TranslationItemDto translationItemDto)
        {
            var translatedText = await _mtService.GetMTResponseAsync(translationItemDto.SourceText, translationItemDto.OutputLanguage, translationItemDto.InputLanguage);
            // var translatedText = "lkfjlkaj;";
            if (string.IsNullOrEmpty(translatedText))
                return null;

            return new TranslationItem
            {
                TranslationId = ObjectId.GenerateNewId().ToString(),
                InputLanguage = translationItemDto.InputLanguage.ToLower(),
                OutputLanguage = translationItemDto.OutputLanguage.ToLower(),
                SourceText = translationItemDto.SourceText,
                TranslatedText = translatedText,
                CreatedAt = DateTime.UtcNow
            };
        }
    }
}
