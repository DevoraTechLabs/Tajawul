using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.ViewModels.user.Chatbot;

namespace Tajawul.Mappers.Chatbot
{
    public static class MessageMapper
    {

        public static MessageDto ToMessageDto(this Message message)
        {
            return new MessageDto
            {
                Prompt = message.Prompt,
                Response = message.Response,
                CreatedAt = message.CreatedAt,
            };
        }
    }
}
