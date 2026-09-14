using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.user.Chatbot
{
    public class CreateChatbotChatDto
    {
        [Required(ErrorMessage = "Prompt is required")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Prompt must be between 3 and 1000 characters")]
        public required string Prompt { get; set; }
    }
}
