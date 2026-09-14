using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.Trips;
using Tajawul.Services;

namespace Tajawul.Repositories
{
    public class TripDurationRepository
    {
        private readonly Neo4jService _neo4jService;

        public TripDurationRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<TripDuration?> GetTripDurationByNameAsync(string oldName)
        {
            return await _neo4jService.ExecuteReadAsync(@"
                MATCH (c:TripDuration {name: apoc.text.capitalizeAll($oldName)})
                RETURN c.id AS id, c.name AS name;",
                 new { oldName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new TripDuration { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
             );
        }


        public async Task<TripDuration?> CreateTripDurationAsync(string name)
        {
            return await _neo4jService.
                ExecuteReadAsync(@"
                MERGE (c:TripDuration {id: randomUUID(), name: apoc.text.capitalizeAll($name)}) 
                RETURN c.name AS name, c.id As id",
                new { name },

                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new TripDuration { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
                );
        }


        public async Task<bool?> DeleteTripDurationAsync(string name)
        {

            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (c:TripDuration {{name: apoc.text.capitalizeAll($name)}})
                OPTIONAL MATCH (c)-[r:{GraphRelations.User.PreferedDuration}]-()
                RETURN COUNT(r) AS relCount",
                new { name },
                async result =>
                {
                    var record = await result.ToListAsync();
                    if (record.SingleOrDefault()?["relCount"].As<int>() > 0)
                    {
                        return false; // Node has relationships, refuse deletion
                    }

                    return await _neo4jService.ExecuteReadAsync(@"
                    MATCH (c:TripDuration {name: apoc.text.capitalizeAll($name)})
                    DELETE c RETURN true",
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
        public async Task<List<TripDuration>> GetAllTripDurationsAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
                "MATCH (c:TripDuration) RETURN c.name AS name, c.id As id",
                new { },
            async result =>
            {
                var list = new List<TripDuration>();
                await result.ForEachAsync(r => list.Add(new TripDuration { Id = r["id"].As<string>(), Name = r["name"].As<string>() }));
                return list;
            }
            ) ?? [];
        }



        public async Task<TripDuration?> UpdateTripDurationNameAsync(string oldName, string newName)
        {
            return await _neo4jService.ExecuteReadAsync(@"
                 MATCH (c:TripDuration {name: apoc.text.capitalizeAll($oldName)}) 
                 SET c.name = apoc.text.capitalizeAll($newName) RETURN c.name AS name, c.id As id",
                 new { oldName, newName },
                 async result =>
                 {
                     var record = await result.ToListAsync();
                     return record.SingleOrDefault() == null ? null : new TripDuration { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                 }
             );

        }

        public async Task<TripDuration> AssignTripDurationAsync(string tripId, string durationName, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
        MATCH (t:Trip {{id: $tripId}}), (u:User {{id: $userId}})
        OPTIONAL MATCH (t)-[r:{GraphRelations.Trip.HadDuration}]->(oldDuration:TripDuration)
        DELETE r // Remove old relationship if it exists
        WITH t, u
        MERGE (newDuration:TripDuration {{name: apoc.text.capitalizeAll($durationName)}})
        ON CREATE SET newDuration.id = randomUUID()
        MERGE (t)-[newR:{GraphRelations.Trip.HadDuration}]->(newDuration)
        ON CREATE SET newR.id = randomUUID(), newR.userId = u.id
        RETURN newDuration.id AS Id, newDuration.name AS Name, newR.userId AS UserId",
                new
                {
                    tripId,
                    durationName,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new TripDuration
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>()
                    };
                }
            );
        }

        public async Task<TripDuration> GetTripDurationAsync(string tripId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
        MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadDuration}]->(m:TripDuration)
        RETURN m.name AS name, m.id AS id",
                new { tripId },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return new TripDuration { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
            );
        }

        public async Task<string> RemoveTripDurationAsync(string tripId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
        MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadDuration}]->(m:TripDuration)
        DELETE r RETURN type(r) AS relationshipType;",
                new { tripId },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.Single()["relationshipType"].As<string>();
                }
            );
        }

    }
}