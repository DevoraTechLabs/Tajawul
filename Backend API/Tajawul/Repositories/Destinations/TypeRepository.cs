using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Models.ViewModels.SearchBar;
using Tajawul.Services;

namespace Tajawul.Repositories.Destinations
{
    public class TypeRepository
    {
        private readonly Neo4jService _neo4jService;

        public TypeRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }
        
        public async Task<Types> AssignTypeAsync(string typeName, string destinationId, string userId)
        {
            var currentDate = DateTime.UtcNow.ToString("o"); // ISO 8601 format
            return await _neo4jService.ExecuteReadAsync(
                $@"
        MATCH (d:Destination {{id: $destinationId}}), (u:User {{id: $userId}})
        OPTIONAL MATCH (d)-[r:{GraphRelations.Destination.HadType}]->(oldType:Type)
        DELETE r // Remove old relationship if it exists
        WITH d, u
        MERGE (newType:Type {{name: apoc.text.capitalize(toLower($typeName))}})
        ON CREATE SET newType.id = randomUUID()
        MERGE (d)-[newR:{GraphRelations.Destination.HadType}]->(newType)
        ON CREATE SET newR.id = randomUUID(), newR.date = $currentDate, newR.userId = u.id
        RETURN newType.id AS Id, newType.name AS Name, newR.userId AS UserId, newR.date AS Date",
                new
                {
                    userId,
                    typeName,
                    destinationId,
                    currentDate
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new Types
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    };
                }
            );
        }
        
        public async Task<List<Types>> GetDestinationTypesAsync(string destinationId, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadType}]->(type:Type)
                RETURN type.id AS Id, type.name AS Name, r.userId AS UserId, r.date AS Date",

                new
                {
                    userId,
                    destinationId,
                },

                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Types
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<List<DestinationTypeDto>> GetAllTypesAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (type:Type)
                OPTIONAL MATCH (type)<-[r1:{GraphRelations.Destination.HadType}]-(:Destination)
                OPTIONAL MATCH (type)<-[r2:{GraphRelations.User.HadDestinationType}]-(:User)
                RETURN type.name AS Name, (COALESCE(COUNT(r1), 0) + COALESCE(COUNT(r2), 0)) AS RelationCount
                ORDER BY RelationCount DESC
                LIMIT 50",
                new { },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new DestinationTypeDto
                    {
                        Name = record["Name"].As<string>()
                    }).ToList();
                }
            ) ?? [];
        }

        public async Task<bool> DeleteDestinationTypeAsync(string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadType}]->(t:Type)
                DELETE r",
                new
                {
                    userId,
                    destinationId
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
