using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tajawul.Models.Domain.Translation
{
    public class Translation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string TranslationDocumentId { get; set; }

        public required string UserId { get; set; }

        public required List<TranslationItem> TranslationItems { get; set; } = [];

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
