using Microsoft.Extensions.Options;
using Neo4j.Driver;
using Tajawul.Models.Domain;


namespace Tajawul.Services
{

    public class Neo4jService : IDisposable
    {
        private readonly IDriver _driver;
        private readonly Neo4jSettings _neo4jSettings;

        public Neo4jService(IOptions<Neo4jSettings> neo4jSettings)
        {
            _neo4jSettings = neo4jSettings.Value;

            _driver = GraphDatabase.Driver(
               _neo4jSettings.Uri,
                AuthTokens.Basic(_neo4jSettings.Username, _neo4jSettings.Password)
            );
        }

        public IAsyncSession GetSession()
        {
            return _driver.AsyncSession();
        }

        public async Task<IResultSummary> ExecuteWriteAsync(string cypherQuery, object parameters)
        {
            if (string.IsNullOrEmpty(cypherQuery)) throw new ArgumentNullException(nameof(cypherQuery));

            if (parameters == null) throw new ArgumentNullException(nameof(parameters));

            await using var session = GetSession();
            await using var tx = await session.BeginTransactionAsync();

            try
            {
                var result = await tx.RunAsync(cypherQuery, parameters);
                await tx.CommitAsync();
                return await result.ConsumeAsync();
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                throw new ApplicationException("Write operation failed.", ex);
            }
        }

        public async Task<T?> ExecuteReadAsync<T>(string cypherQuery, object parameters, Func<IResultCursor, Task<T?>> handleResult)
        {
            if (string.IsNullOrEmpty(cypherQuery)) throw new ArgumentNullException(nameof(cypherQuery));
            if (parameters == null) throw new ArgumentNullException(nameof(parameters));
            if (handleResult == null) throw new ArgumentNullException(nameof(handleResult));

            await using var session = GetSession();
            var result = await session.RunAsync(cypherQuery, parameters);
            return await handleResult(result);
        }


        public void Dispose() => _driver?.Dispose();
    }

}
