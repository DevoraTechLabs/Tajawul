using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.Domain.Chatbot
{
    public class ChatbotChat
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public  string? ChatId { get; set; }
        [Required]
        public required string UserId { get; set; }

        [Required]
        public required List<Message> Messages { get; set; }
    }
}
