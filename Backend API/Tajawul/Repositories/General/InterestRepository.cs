using Neo4j.Driver;
using System.Threading.Tasks;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs.General;
using Tajawul.Queries;
using Tajawul.Services;

namespace Tajawul.Repositories.General
{
    public class InterestRepository
    {
        private readonly Neo4jService _neo4jService;

        public InterestRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<Interest?> GetInterestByNameAsync(string name)
        {
            return await _neo4jService.ExecuteReadAsync(
                InterestQueries.GetInterestByName,
                new { name },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new Interest { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
            );
        }

        public async Task<Interest?> CreateInterestAsync(InterestDto interestDto)
        {
            return await _neo4jService.ExecuteReadAsync(
                InterestQueries.CreateInterest,
                new { name = interestDto.Name },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new Interest { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
                );
        }

        public async Task<bool?> DeleteInterestAsync(string name)
        {

            return await _neo4jService.ExecuteReadAsync(
                InterestQueries.CountInterestRelationships,
               new { name },
               async result =>
               {
                   var record = await result.ToListAsync();
                   if (record.SingleOrDefault()?["relCount"].As<int>() > 0)
                   {
                       return false; // Node has relationships, refuse deletion
                   }

                   return await _neo4jService.ExecuteReadAsync(
                       InterestQueries.DeleteInterest,
                       new { name },

                       async result =>
                       {
                           var record = await result.ToListAsync();
                           return record.SingleOrDefault() == null ? false : true;
                       }

                   );
               }
           );
        }

        public async Task<Interest?> UpdateInterestNameAsync(string oldName, string newName)
        {
            return await _neo4jService.ExecuteReadAsync(
                InterestQueries.UpdateInterestName,
                 new { oldName, newName },
                 async result =>
                 {
                     var record = await result.ToListAsync();
                     return record.SingleOrDefault() == null ? null : new Interest { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                 }
             );
        }

        public async Task<List<Interest>> GetAllInterestsAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
                  InterestQueries.GetAllInterests,
                  new { },
                  async result =>
                  {
                      var list = new List<Interest>();
                      await result.ForEachAsync(r => list.Add(new Interest { Id = r["id"].As<string>(), Name = r["name"].As<string>() }));
                      return list;
                  }
                ) ?? [];
        }

        //public async Task<Interest?> AddUserInterestAsync(string userId, string interestName)
        //{
        //    return await _neo4jService.ExecuteReadAsync(
        //          InterestQueries.AddUserInterest,
        //           new { userId, interestName },

        //           async result =>
        //           {
        //               var record = await result.ToListAsync();
        //               return record.SingleOrDefault() == null ? null : new Interest { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
        //           }
        //           );
        //}

        public async Task<List<string>> AddUserInterestsAsync(string userId, List<string> interestNames)
        {

            return await _neo4jService.ExecuteReadAsync(
                InterestQueries.AddUserInterests,
                new { userId, interestNames },
                async result =>
                {
                    var interests = new List<string>();
                    await result.ForEachAsync(r => interests.Add(r["name"].As<string>()));
                    return interests;
                }
            ) ?? [];
        }



        public async Task<bool> DeleteUserInterestAsync(string userId, string interestName)
        {
            return await _neo4jService.ExecuteReadAsync(
                  InterestQueries.DeleteUserInterest,
                   new { userId, interestName },

                   async result =>
                   {
                       var record = await result.ToListAsync();
                       return record.SingleOrDefault()?["relCount"].As<int>() > 0 ? true : false;
                   }
                   );
        }

        public async Task<List<Interest>> GetUserInterestsAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                InterestQueries.GetUserInterests,
                new { userId },
                async result =>
                {
                    var list = new List<Interest>();
                    await result.ForEachAsync(r => list.Add(new Interest { Id = r["id"].As<string>(), Name = r["name"].As<string>() }));
                    return list;
                }
            ) ?? [];
        }
    }
}

