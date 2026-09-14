using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Tajawul.Data.Configuration.MongoConfigraton;
using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.Domain.Translation;

namespace Tajawul.Data.Configuration.MongoConfiguration
{
    public static class MongoConfiguration
    {
        public static IServiceCollection AddMongoDbServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register MongoDB settings
            services.Configure<MongoDbSettings>(configuration.GetSection("MongoDbSettings"));

            // Register MongoDB client as singleton
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                return new MongoClient(settings.ConnectionString);
            });

            // Register MongoContext as scoped
            services.AddScoped<MongoContext>(sp =>
            {
                var client = sp.GetRequiredService<IMongoClient>();
                var settings = configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
                return new MongoContext(client, settings);
            });
            //// Register MongoDbIndexInitializer
            //services.AddHostedService<MongoDbIndexInitializer>();

            return services;
        }
    }
}