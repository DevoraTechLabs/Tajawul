//using MongoDB.Driver;
//using Tajawul.Models.Domain.Translation;
//namespace Tajawul.Data.Configuration.MongoConfiguration
//{
//    public class MongoDbIndexInitializer : IHostedService
//    {
//        private readonly IServiceProvider _serviceProvider;
//        private readonly ILogger<MongoDbIndexInitializer> _logger;

//        public MongoDbIndexInitializer(IServiceProvider serviceProvider, ILogger<MongoDbIndexInitializer> logger)
//        {
//            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        public async Task StartAsync(CancellationToken cancellationToken)
//        {
//            _logger.LogInformation("MongoDB Index Initializer is starting.");

//            // Create a new scope to retrieve scoped services
//            using (var scope = _serviceProvider.CreateScope())
//            {
//                var scopedProvider = scope.ServiceProvider;
//                try
//                {
//                    // Resolve the MongoContext within the scope
//                    var mongoContext = scopedProvider.GetRequiredService<MongoContext>();
//                    var translationsCollection = mongoContext.Translations; 

//                    // --- Define and Create the Compound Index for Translations ---
//                    _logger.LogInformation("Creating index on {CollectionName}...", translationsCollection.CollectionNamespace.CollectionName);

//                    var indexKeysDefinition = Builders<Translation>.IndexKeys
//                            .Ascending(t => t.UserId)                   
//                            .Descending("TranslationItems.TranslationId"); 

//                    var indexModel = new CreateIndexModel<Translation>(
//                        indexKeysDefinition,
//                        // Naming the index is highly recommended!
//                        new CreateIndexOptions { Name = "UserId_TranslationItems_TranslationId_Idx", Background = true }
//                    );

//                    await translationsCollection.Indexes.CreateOneAsync(indexModel, cancellationToken: cancellationToken);
//                    _logger.LogInformation("Index '{IndexName}' on collection '{CollectionName}' checked/created successfully.",
//                        indexModel.Options.Name, translationsCollection.CollectionNamespace.CollectionName);

//                    // --- You can add more index creations here for other collections ---
//                    // Example: await CreateChatbotIndexesAsync(mongoContext, cancellationToken);

//                }
//                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
//                {
//                    _logger.LogWarning("Index creation was canceled.");
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "An error occurred while creating MongoDB indexes.");
//                     throw;
//                }
//            }

//            _logger.LogInformation("MongoDB Index Initializer has finished.");
//        }

//        // StopAsync is usually simple for initializers unless cleanup is needed
//        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
//    }
//}

