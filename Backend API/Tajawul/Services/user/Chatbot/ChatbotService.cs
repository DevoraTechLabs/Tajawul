using MongoDB.Driver;
using Tajawul.Data;
using Tajawul.Interfaces.User.ChatBot;
using Tajawul.Mappers.Chatbot;
using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.ViewModels.user.Chatbot;
using Tajawul.Repositories.User;

namespace Tajawul.Services.User.Chatbot
{
    public class ChatbotService : IChatbotService
    {
        private readonly ChatbotRepository _chatbotRepository;
        private readonly ILlmService _llmService;

        public ChatbotService(ChatbotRepository chatbotRepository, ILlmService llmService)
        {
            _chatbotRepository = chatbotRepository;
            _llmService = llmService;
        }

        public async Task<ChatbotChatDto?> CreateChatAsync(string prompt, string token, string userId)
        {
            // Create the first message with the prompt and GPT response
            var response = await _llmService.GetLlmResponseAsync(prompt, token);
            if (response == null)
            {
                return null;
            }
            var message = new Message
            {
                Prompt = prompt,
                Response = response,
                CreatedAt = DateTime.UtcNow
            };

            // Initialize a new chat with the user's ID and the first message
            var chat = new ChatbotChat
            {
                UserId = userId, // Store the authenticated user's ID here
                Messages = [message]

            };


            // Insert the chat into the MongoDB collection
            await _chatbotRepository.CreateChatAsync(chat);

            return chat.ToChatbotChatDto();
        }


        public async Task<MessageDto?> SendPromptAsync(string chatId, string userId, string prompt, string token)
        {

            var chatbotChat = await _chatbotRepository.GetChatAsync(chatId);

            if (chatbotChat == null || chatbotChat.UserId != userId) return null;
            var response = await _llmService.GetLlmResponseAsync(prompt, token);
            if (response == null)
            {
                return null;
            }
            // Create a new message
            var message = new Message
            {
                Prompt = prompt,
                Response = response,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _chatbotRepository.SaveMessageAsync(chatId, message);

            if (result.IsAcknowledged == false) return null;

            return message.ToMessageDto();
        }

        public async Task<bool> DeleteChatAsync(string chatId, string userId)
        {

            var chat = await _chatbotRepository.GetChatAsync(chatId);

            if (chat == null || chat.UserId != userId) return false;

            return await _chatbotRepository.DeleteChatAsync(chat);

        }

        public async Task<ChatbotChatDto?> GetChatAsync(string chatId, string userId)
        {
            var chat = await _chatbotRepository.GetChatAsync(chatId);

            if (chat == null || chat.UserId != userId) return null;

            return chat.ToChatbotChatDto();
        }

        public async Task<List<ChatbotChatDto>> GetAllUserChatsAsync(string userId)
        {
            return [.. (await _chatbotRepository.GetAllUserChatsAsync(userId)).Select(chat => chat.ToChatbotChatDto())];
        }

        public async Task<List<Message>> GetLastMessagesForUserAsync(string userId, int count)
        {
            return await _chatbotRepository.GetLastMessagesForUserAsync(userId, count);
        }
    }
}
