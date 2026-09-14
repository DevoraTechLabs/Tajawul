using Tajawul.Data.Configuration.MongoConfigraton;
using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.Domain.Translation;
using MongoDB.Driver;


namespace Tajawul.Data.Configuration.MongoConfiguration
{

    public class MongoContext
    {
        private readonly IMongoDatabase _database;

        public MongoContext(IMongoClient mongoClient, MongoDbSettings settings)
        {
            _database = mongoClient.GetDatabase(settings.DatabaseName);
        }

        public IMongoCollection<ChatbotChat> ChatbotChats =>
            _database.GetCollection<ChatbotChat>("ChatbotCollection");

        public IMongoCollection<ChatbotChat> DeletedChatbotChats =>
            _database.GetCollection<ChatbotChat>("DeletedChatbotCollection");

        public IMongoCollection<Translation> Translations =>
            _database.GetCollection<Translation>("TranslationCollection");
    }
}

