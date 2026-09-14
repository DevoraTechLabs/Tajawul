using MongoDB.Driver;
using Tajawul.Data.Configuration.MongoConfiguration;
using Tajawul.Models.Domain.Chatbot;

namespace Tajawul.Repositories.User
{
    public class ChatbotRepository
    {
        private readonly MongoContext _mongoContext;

        public ChatbotRepository(MongoContext mongoContext)
        {
            _mongoContext = mongoContext ?? throw new ArgumentNullException(nameof(mongoContext));
        }

        public async Task CreateChatAsync(ChatbotChat chatbotChat)
        {
            await _mongoContext.ChatbotChats.InsertOneAsync(chatbotChat);
        }

        public async Task<ChatbotChat?> GetChatAsync(string chatId)
        {
            return await _mongoContext.ChatbotChats.Find(x => x.ChatId == chatId).FirstOrDefaultAsync();
        }

        public async Task<UpdateResult> SaveMessageAsync(string chatId, Message message)
        {
            return await _mongoContext.ChatbotChats.UpdateOneAsync(
                x => x.ChatId == chatId,
                Builders<ChatbotChat>.Update.Push(x => x.Messages, message));
        }

        public async Task<bool> DeleteChatAsync(ChatbotChat chatbotChat)
        {
            await _mongoContext.DeletedChatbotChats.InsertOneAsync(chatbotChat);
            var deletedResult = await _mongoContext.ChatbotChats.DeleteOneAsync(x => x.ChatId == chatbotChat.ChatId);
            return deletedResult.DeletedCount > 0;
        }

        public async Task<List<ChatbotChat>> GetAllUserChatsAsync(string userId)
        {
            return await _mongoContext.ChatbotChats.Find(x => x.UserId == userId).ToListAsync();
        }

        public async Task<List<Message>> GetLastMessagesForUserAsync(string userId, int count)
        {
            var chats = await _mongoContext.ChatbotChats
                .Find(x => x.UserId == userId)
                .ToListAsync();

            var messages = chats
                .SelectMany(chat => chat.Messages)
                .OrderByDescending(m => m.CreatedAt)
                .Take(count)
                .ToList();

            return messages;
        }

    }
}