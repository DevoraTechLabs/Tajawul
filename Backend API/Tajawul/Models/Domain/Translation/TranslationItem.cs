using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Tajawul.Models.Domain.Translation
{
    public class TranslationItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public required string TranslationId { get; set; }

        public required string SourceText { get; set; }

        public required string TranslatedText { get; set; }

        public required string InputLanguage { get; set; }

        public required string OutputLanguage { get; set; }

        public bool IsFavorite { get; set; }

        //public string? SourceAudioUrl { get; set; }
        //public string? TranslatedAudioUrl { get; set; }

        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}