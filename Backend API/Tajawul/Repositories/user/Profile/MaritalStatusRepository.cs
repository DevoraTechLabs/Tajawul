using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs.user.Profile;
using Tajawul.Queries;
using Tajawul.Services;

namespace Tajawul.Repositories
{
    public class MaritalStatusRepository
    {
        private readonly Neo4jService _neo4jService;

        public MaritalStatusRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<MaritalStatus?> GetMaritalStatusByNameAsync(string maritalStatusName)
        {
            return await _neo4jService.ExecuteReadAsync(
                MaritalStatusQueries.GetMaritalStatusByNameQuery,
                new { maritalStatusName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new MaritalStatus { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
                );
        }

        public async Task<MaritalStatus?> CreateMaritalStatusAsync(MaritalStatusDto maritalState)
        {
            return await _neo4jService.ExecuteReadAsync(
               MaritalStatusQueries.CreateMaritalStatusQuery,
                new { maritalStatusName = maritalState.Name },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new MaritalStatus { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }

                );
        }

        public async Task<bool?> DeleteMaritalStatusAsync(string maritalStatusName)
        {

            return await _neo4jService.ExecuteReadAsync(
                MaritalStatusQueries.CountMaritalStatusQuery,
                new { maritalStatusName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    if (record.SingleOrDefault()?["relCount"].As<int>() > 0)
                    {
                        return false; // Node has relationships, refuse deletion
                    }

                    return await _neo4jService.ExecuteReadAsync(
                        MaritalStatusQueries.DeleteMaritalStatusQuery,
                        new { maritalStatusName },

                        async result =>
                        {
                            var record = await result.ToListAsync();
                            return record.SingleOrDefault() == null ? false : true;
                        }

                    );
                }
            );
        }

        public async Task<List<MaritalStatus>> GetAllMaritalStatusAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
               MaritalStatusQueries.GetAllMaritalStatusQuery,
                new { },
            async result =>
            {
                var list = new List<MaritalStatus>();
                await result.ForEachAsync(r => list.Add(new MaritalStatus { Id = r["id"].As<string>(), Name = r["name"].As<string>() }));
                return list;
            }
            ) ?? [];
        }

        public async Task<MaritalStatus?> UpdateMaritalStatusNameAsync(string oldName, string newName)
        {
            return await _neo4jService.ExecuteReadAsync(
                 MaritalStatusQueries.UpdateMaritalStatusNameQuery,
                 new { oldName, newName },

                 async result =>
                 {
                     var record = await result.ToListAsync();
                     return record.SingleOrDefault() == null ? null : new MaritalStatus { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                 }
             );


        }

        public async Task<int> AddUserMaritalStatusAsync(string userId, string maritalStatusName)
        {
            return await _neo4jService.ExecuteReadAsync(
                MaritalStatusQueries.AddUserMaritalStatusQuery,
                new { userId, maritalStatusName },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return
                        record["relationshipsCreated"].As<int>()
                    ;
                }
            );
        }

        // public async Task<bool> DeleteUserMaritalStatusAsync(string userId)
        // {
        //     return await _neo4jService.ExecuteReadAsync(
        //         MaritalStatusQueries.DeleteUserMaritalStatusQuery,
        //         new { userId },

        //         async result =>
        //         {
        //             var record = await result.ToListAsync();
        //             return record.SingleOrDefault()?["relCount"].As<int>() > 0 ? true : false;
        //         }
        //     );
        // }

        // public async Task<MaritalStatus?> GetUserMaritalStatusAsync(string userId)
        // {
        //     return await _neo4jService.ExecuteReadAsync(
        //         MaritalStatusQueries.GetUserMaritalStatusQuery,
        //         new { userId },
        //         async result =>
        //         {
        //             var record = await result.ToListAsync();
        //             return record.SingleOrDefault() == null ? null : new MaritalStatus { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
        //         }
        //         );
        // }
    }
}