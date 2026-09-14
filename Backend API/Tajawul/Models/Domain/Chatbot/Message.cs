using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.Domain.Chatbot
{
    public class Message
    {
        [BsonId] // MongoDB will use this field as the unique identifier for the document
        [BsonRepresentation(BsonType.ObjectId)] // Ensures MongoDB treats this as an ObjectId
        public  string? MessageId { get; set; } = ObjectId.GenerateNewId().ToString();
        [Required]
        public required string Prompt { get; set; }
        public required string Response { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}
