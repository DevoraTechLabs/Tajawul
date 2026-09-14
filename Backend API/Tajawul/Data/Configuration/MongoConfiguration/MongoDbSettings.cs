namespace Tajawul.Data.Configuration.MongoConfigraton
{
    public class MongoDbSettings
    {
        public required string ConnectionString { get; set; }
        public required string DatabaseName { get; set; }
        public required string ChatbotCollection { get; set; }
        public required string DeletedChatbotCollection { get; set; }
        public required string TranslationCollection { get; set; }

    }
}
