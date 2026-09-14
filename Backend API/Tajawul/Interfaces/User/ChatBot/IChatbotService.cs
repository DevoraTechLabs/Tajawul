using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.ViewModels.user.Chatbot;

namespace Tajawul.Interfaces.User.ChatBot
{
    public interface IChatbotService
    {
        Task<ChatbotChatDto?> CreateChatAsync(string prompt, string token, string userId);
        Task<MessageDto?> SendPromptAsync(string chatId, string userId, string prompt, string token);
        //Task<Message> EditPromptAsync(string chatId, Message message, string prompt);
        Task<bool> DeleteChatAsync(string chatId, string userId);
        Task<ChatbotChatDto?> GetChatAsync(string chatId, string userId);
        Task<List<ChatbotChatDto>> GetAllUserChatsAsync(string userId);
        Task<List<Message>> GetLastMessagesForUserAsync(string userId, int count);
    }
}
