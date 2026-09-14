using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.user.Chatbot
{
    public class DeleteChatbotChatDto
    {
        [Required(ErrorMessage = "ChatId is required")]
        [MongoObjectId]
        public required string ChatId { get; set; }
    }
}
