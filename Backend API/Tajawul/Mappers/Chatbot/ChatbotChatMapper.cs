using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.ViewModels.user.Chatbot;

namespace Tajawul.Mappers.Chatbot
{
    public static class ChatbotChatMapper
    {

        public static ChatbotChatDto ToChatbotChatDto(this ChatbotChat chatbotChat)
        {
            return new ChatbotChatDto
            {
                ChatId = chatbotChat.ChatId,
                Messages = chatbotChat.Messages.Select(m => m.ToMessageDto()).ToList()
            };
        }
    }
}
