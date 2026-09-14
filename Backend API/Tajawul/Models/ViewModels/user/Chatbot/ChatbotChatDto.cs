using Tajawul.Models.Domain.Chatbot;

namespace Tajawul.Models.ViewModels.user.Chatbot
{
    public class ChatbotChatDto
    {
        public required string ChatId { get; set; }
        public required List<MessageDto> Messages { get; set; }
    }
}
