using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Services;

namespace Tajawul.Repositories.Destinations
{
    public class GroupSizeRepository
    {

        private readonly Neo4jService _neo4jService;

        public GroupSizeRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }

        public async Task<List<GroupSize>> GetDestinationGroupSizesAsync(string destinationId)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"
                  MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.PreferedGroupSize}]->(g:GroupSize)
                  RETURN g.id AS Id, g.name AS Size, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    destinationId = destinationId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new GroupSize
                    {
                        GroupId = record["Id"].As<string>(),
                        Group = record["Size"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<GroupSize> AssignGroupSizeAsync(string size, string destinationId, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                  MATCH (u:User {{id: $userId}})
                  MATCH (d:Destination {{id: $destinationId}})
                  MATCH (g:GroupSize)
                  WHERE toLower(g.name) = toLower($size)
                  WITH d, g, u
                  MERGE (d)-[r:{GraphRelations.Destination.PreferedGroupSize}]->(g)
                  ON CREATE SET r.id = randomUUID(), r.date = datetime(), r.userId = u.id
                  MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                  SET e.date = datetime()
                  RETURN g.id AS Id, g.name AS Size, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    userId = userId,
                    size = size,
                    destinationId = destinationId,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new GroupSize
                    {
                        GroupId = record["Id"].As<string>(),
                        Group = record["Size"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    };
                }
            );
        }

        public async Task<bool> RemoveDestinationGroupSizeAsync(string size, string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(

                $@"MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.PreferedGroupSize}]->(g:GroupSize)
                WHERE g.name = $size
                DELETE r
               
                WITH u, d

                MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                SET e.date = datetime()
                ",
                new
                {
                    userId = userId,
                    destinationId = destinationId,
                    size = size,
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
