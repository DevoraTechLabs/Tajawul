using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.user.Chatbot
{
    public class SendPromptDto
    {
        [Required(ErrorMessage = "ChatId is required")]
        [MongoObjectId]
        public required string ChatId { get; set; }
        [Required]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Prompt must be between 3 and 1000 characters")]
        public required string Prompt { get; set; }
    }
}
