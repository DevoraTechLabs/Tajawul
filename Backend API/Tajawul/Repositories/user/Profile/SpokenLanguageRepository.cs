using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Queries;
using Tajawul.Services;
using Neo4j.Driver.Mapping;
using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs.user.Profile;

namespace Tajawul.Repositories
{
    public class SpokenLanguageRepository
    {

        private readonly Neo4jService _neo4jService;

        public SpokenLanguageRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<SpokenLanguage?> GetSpokenLanguageByNameAsync(string spokenLanguageName)
        {
            return await _neo4jService.ExecuteReadAsync(
                SpokenLanguageQueries.GetSpokenLanguageByNameQuery,
                new { spokenLanguageName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new SpokenLanguage { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
                );
        }

        public async Task<SpokenLanguage?> CreateSpokenLanguageAsync(SpokenLanguageDto spokenLanguage)
        {
            return await _neo4jService.ExecuteReadAsync(
                SpokenLanguageQueries.CreateSpokenLanguageQuery,
                new { spokenLanguageName = spokenLanguage.Name },

                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new SpokenLanguage { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }

                );
        }

        public async Task<bool?> DeleteSpokenLanguageAsync(string spokenLanguageName)
        {
            return await _neo4jService.ExecuteReadAsync(
                SpokenLanguageQueries.CountSpokenLanguageRelationshipsQuery,
                new { spokenLanguageName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    if (record.SingleOrDefault()?["relCount"].As<int>() > 0)
                    {
                        return false; // Node has relationships, refuse deletion
                    }

                    return await _neo4jService.ExecuteReadAsync(
                        SpokenLanguageQueries.DeleteSpokenLanguageQuery,
                        new { spokenLanguageName },

                        async result =>
                        {
                            var record = await result.ToListAsync();
                            return record.SingleOrDefault() == null ? false : true;
                        }

                    );
                }
            );
        }

        public async Task<List<SpokenLanguage>> GetAllSpokenLanguagesAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
               SpokenLanguageQueries.GetAllSpokenLanguagesQuery,
                new { },
            async result =>
            {
                var list = new List<SpokenLanguage>();
                await result.ForEachAsync(r => list.Add(new SpokenLanguage { Id = r["id"].As<string>(), Name = r["name"].As<string>() }));
                return list;
            }
            ) ?? [];
        }

        public async Task<SpokenLanguage?> UpdateSpokenLanguageNameAsync(string oldName, string newName)
        {
            return await _neo4jService.ExecuteReadAsync(
               SpokenLanguageQueries.UpdateSpokenLanguageNameQuery,
                new { oldName, newName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new SpokenLanguage { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
            );
        }

        public async Task<(int relationshipsCreated, List<string> spokenLanguages)> AddUserSpokenLanguagesAsync(string userId, List<string> spokenLanguages)
        {
            return await _neo4jService.ExecuteReadAsync(
                SpokenLanguageQueries.AddUserSpokenLanguagesQuery,
                new { userId, spokenLanguages },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return (
                        record["relationshipsCreated"].As<int>(),
                        record["spokenLanguages"].As<List<string>>()
                    );
                }
            );
        }

        // public async Task<bool> DeleteUserSpokenLanguageAsync(string userId, string spokenLanguageName)
        // {
        //     return await _neo4jService.ExecuteReadAsync(
        //        SpokenLanguageQueries.DeleteUserSpokenLanguageQuery,
        //         new { userId, spokenLanguageName },

        //         async result =>
        //         {
        //             var record = await result.ToListAsync();
        //             return record.SingleOrDefault()?["relCount"].As<int>() > 0 ? true : false;
        //         }
        //     );
        // }

        // public async Task<List<SpokenLanguage>> GetUserSpokenLanguagesAsync(string userId)
        // {
        //     return await _neo4jService.ExecuteReadAsync(
        //         SpokenLanguageQueries.GetUserSpokenLanguagesQuery,
        //          new { userId },
        //          async result =>
        //          {
        //              var list = new List<SpokenLanguage>();
        //              await result.ForEachAsync(r => list.Add(new SpokenLanguage { Id = r["id"].As<string>(), Name = r["name"].As<string>() }));
        //              return list;
        //          }
        //      ) ?? [];
        // }
    }
}