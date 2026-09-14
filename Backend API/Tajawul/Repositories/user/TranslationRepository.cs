using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using Neo4j.Driver;
using Tajawul.Data;
using Tajawul.Data.Configuration.MongoConfiguration;
using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Translation;

namespace Tajawul.Repositories.User
{
    public class TranslationRepository
    {
        private readonly MongoContext _mongoContext;

        public TranslationRepository(MongoContext mongoContext, Db14639Context dbContext)
        {
            _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));
        }

        public async Task<bool> AddTranslationAsync(TranslationItem translationItem, string userId)
        {

            var filter = Builders<Translation>.Filter.Eq(t => t.UserId, userId);
            var update = Builders<Translation>.Update
                .Push(t => t.TranslationItems, translationItem);

            var options = new FindOneAndUpdateOptions<Translation>
            {
                IsUpsert = true
            };

            await _mongoContext.Translations.FindOneAndUpdateAsync(filter, update, options);
            return true;
        }


        public async Task<(bool Success, bool IsFavorite)> MarkAsFavoriteAsync(string translationId, string userId)
        {
            // Step 1: Find the current item to get the current IsFavorite value
            var filter = Builders<Translation>.Filter.And(
                Builders<Translation>.Filter.Eq(t => t.UserId, userId),
                Builders<Translation>.Filter.ElemMatch(t => t.TranslationItems, ti => ti.TranslationId == translationId)
            );

            var projection = Builders<Translation>.Projection
                .ElemMatch(t => t.TranslationItems, ti => ti.TranslationId == translationId);

            var document = await _mongoContext.Translations.Find(filter)
                .Project<Translation>(projection)
                .FirstOrDefaultAsync();

            if (document == null || document.TranslationItems == null || !document.TranslationItems.Any())
                return (false, false);

            var item = document.TranslationItems.First();
            var newFavoriteStatus = !item.IsFavorite;

            // Step 2: Update the IsFavorite flag
            var update = Builders<Translation>.Update
                .Set("TranslationItems.$.IsFavorite", newFavoriteStatus);

            var updateResult = await _mongoContext.Translations.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
                return (false, false);

            // Step 3: Return the new status
            return (true, newFavoriteStatus);
        }

        public async Task<List<TranslationItem>> GetUserTranslationHistoryAsync(string userId, TranslationFilter filter)
        {
            var sortDirection = filter.SortDescending ? -1 : 1;

            // Build aggregation pipeline
            var translationItems = await _mongoContext.Translations.Aggregate()
                .Match(t => t.UserId == userId) // Filter by user ID
                .Unwind(t => t.TranslationItems) // Flatten the TranslationItems array
                .Sort(new BsonDocument { { "TranslationItems.CreatedAt", sortDirection } }) // Sort by CreatedAt
                .Skip((filter.PageNumber - 1) * filter.PageSize) // Implement pagination
                .Limit(filter.PageSize) // Limit results
                .Project<TranslationItem>(new BsonDocument
                {
                    { "_id", "$TranslationItems._id" },
                    { "TranslationId", "$TranslationItems.TranslationId" },
                    { "SourceText", "$TranslationItems.SourceText" },
                    { "TranslatedText", "$TranslationItems.TranslatedText" },
                    { "InputLanguage", "$TranslationItems.InputLanguage" },
                    { "OutputLanguage", "$TranslationItems.OutputLanguage" },
                    { "IsFavorite", "$TranslationItems.IsFavorite" },
                    { "CreatedAt", "$TranslationItems.CreatedAt" }
                })
                .ToListAsync();

            return translationItems;
        }

        public async Task<List<TranslationItem>> GetFavoritesAsync(string userId, TranslationFilter filter)
        {
            var sortDirection = filter.SortDescending ? -1 : 1;

            var translationItems = await _mongoContext.Translations.Aggregate()
                .Match(t => t.UserId == userId) // Match by user ID
                .Unwind(t => t.TranslationItems) // Split array into individual items
                .Match(new BsonDocument("TranslationItems.IsFavorite", true)) // Match only favorite items
                .Sort(new BsonDocument { { "TranslationItems.CreatedAt", sortDirection } }) // Sort by CreatedAt
                .Skip(Math.Max(0, (filter.PageNumber - 1) * filter.PageSize)) // Handle pagination safely
                .Limit(filter.PageSize)
                .Project<TranslationItem>(new BsonDocument // Project only necessary fields
                {
            { "_id", "$TranslationItems._id" },
            { "TranslationId", "$TranslationItems.TranslationId" },
            { "SourceText", "$TranslationItems.SourceText" },
            { "TranslatedText", "$TranslationItems.TranslatedText" },
            { "InputLanguage", "$TranslationItems.InputLanguage" },
            { "OutputLanguage", "$TranslationItems.OutputLanguage" },
            { "IsFavorite", "$TranslationItems.IsFavorite" },
            { "CreatedAt", "$TranslationItems.CreatedAt" }
                })
                .ToListAsync();

            return translationItems;
        }

        public async Task<bool> DeleteTranslationItemAsync(string translationId, string userId)
        {
            var filter = Builders<Translation>.Filter.Eq(t => t.UserId, userId);

            var update = Builders<Translation>.Update
                .PullFilter(t => t.TranslationItems,
                    Builders<TranslationItem>.Filter.Eq(ti => ti.TranslationId, translationId));
            var result = await _mongoContext.Translations.UpdateOneAsync(filter, update);
            return result.ModifiedCount > 0;

        }
        public async Task<TranslationItem?> GetTranslationItemAsync(string translationId, string userId)
        {
            var filter = Builders<Translation>.Filter.And(
                Builders<Translation>.Filter.Eq(t => t.UserId, userId),
                Builders<Translation>.Filter.ElemMatch(t => t.TranslationItems, ti => ti.TranslationId == translationId)
            );

            var projection = Builders<Translation>.Projection
                .ElemMatch(t => t.TranslationItems, ti => ti.TranslationId == translationId);

            var document = await _mongoContext.Translations
                .Find(filter)
                .Project<Translation>(projection)
                .FirstOrDefaultAsync();

            return document?.TranslationItems?.FirstOrDefault();
        }


        public async Task<bool?> ClearTranslationHistoryAsync(string userId)
        {
            var filter = Builders<Translation>.Filter.Eq(t => t.UserId, userId);
            var update = Builders<Translation>.Update.Set(t => t.TranslationItems, []);
            var result = await _mongoContext.Translations.UpdateOneAsync(filter, update);
            if (result.MatchedCount == 0) return false;
            else if (result.ModifiedCount == 0) return null;
            else return true;
           
        }
    }
}
