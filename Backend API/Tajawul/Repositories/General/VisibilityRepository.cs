using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Services;

namespace Tajawul.Repositories
{
    public class VisibilityRepository
    {
        private readonly Neo4jService _neo4jService;

        public VisibilityRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }


        public async Task<Visibility> AssignVisibilityAsync(string entityId, string visibilityName, string relationName)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (n)
                WHERE n.id = $entityId
                OPTIONAL MATCH (n)-[:{relationName}]->(oldVisibility:Visibility)
                OPTIONAL MATCH (v:Visibility {{name: apoc.text.capitalizeAll($visibilityName)}})
                WITH n, v, oldVisibility
                CALL apoc.do.when(
                        v IS NOT NULL,
                        'OPTIONAL MATCH (n)-[r:{relationName}]->() DELETE r
                        MERGE (n)-[:{relationName}]->(v)
                        RETURN v.name AS name, v.id AS id',
                        'RETURN oldVisibility.name AS name, oldVisibility.id AS id',
                        {{ n: n, v: v, oldVisibility: oldVisibility }}
                ) YIELD value

                RETURN 
                        value.id AS Id,
                        value.name AS Name",
                new
                {
                    entityId,
                    visibilityName,
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new Visibility
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>()
                    };
                }
            );
        }

        public async Task<Visibility> GetEntityVisibilityAsync(string entityId, string relationName)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (n)
                WHERE n.id = $entityId
                MATCH (n)-[r:{relationName}]->(v:Visibility)
                RETURN v.id AS Id, v.name AS Name",

                new
                {
                    entityId
                },

                async result =>
                {
                    var record = await result.SingleAsync();
                    return new Visibility
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>()
                    };
                }
            );
        }

        public async Task<Visibility> AssignTripVisibilityAsync(string tripId, string visibilityName, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (t:Trip {{id: $tripId}}), (u:User {{id: $userId}})
                OPTIONAL MATCH (t)-[r:{GraphRelations.Trip.HadVisibility}]->(oldVisibility:Visibility)
                DELETE r 
                WITH t, u
                MERGE (newVisibility:Visibility {{name: apoc.text.capitalize(toLower($visibilityName))}})
                ON CREATE SET newVisibility.id = randomUUID()
                MERGE (t)-[newR:{GraphRelations.Trip.HadVisibility}]->(newVisibility)
                ON CREATE SET newR.id = randomUUID(), newR.userId = u.id

                RETURN 
                        newVisibility.id AS Id,
                        newVisibility.name AS Name, 
                        newR.userId AS UserId",
                new
                {
                    userId,
                    visibilityName,
                    tripId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new Visibility
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                    };
                }
            );
        }

        public async Task<List<Visibility>> GetTripVisibilityAsync(string tripId, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadVisibility}]->(v:Visibility)
                RETURN v.id AS Id, v.name AS Name, r.userId AS UserId",

                new
                {
                    userId,
                    tripId,
                },

                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Visibility
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                    }).ToList();
                }
            );
        }

        public async Task<bool> DeleteTripVisibilityAsync(string tripId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadVisibility}]->(v:Visibility)
                DELETE r",
                new
                {
                    userId,
                    tripId
                }
            );
            int relationshipsDeleted = summary.Counters.RelationshipsDeleted;
            if (relationshipsDeleted > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
