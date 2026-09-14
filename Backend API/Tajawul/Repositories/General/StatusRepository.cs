using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Services;

namespace Tajawul.Repositories
{
    public class StatusRepository
    {
        private readonly Neo4jService _neo4jService;

        public StatusRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }

        public async Task<Status> AssignTripStatusAsync(string tripId, string statusName, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (t:Trip {{id: $tripId}}), (u:User {{id: $userId}})
                OPTIONAL MATCH (t)-[r:{GraphRelations.Trip.HadStatus}]->(oldStatus:Status)
                DELETE r 
                WITH t, u
                MERGE (newStatus:Status {{name: apoc.text.capitalize(toLower($statusName))}})
                ON CREATE SET newStatus.id = randomUUID(), 
                newStatus.description = COALESCE(newStatus.description, 'Default description for ' + $statusName)
                MERGE (t)-[newR:{GraphRelations.Trip.HadStatus}]->(newStatus)
                ON CREATE SET newR.id = randomUUID(), newR.userId = u.id
                RETURN 
                        newStatus.id AS Id, 
                        newStatus.name AS Name, 
                        newStatus.description AS Description, 
                        newR.userId AS UserId",
                new
                {
                    userId,
                    statusName,
                    tripId,
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new Status
                    {
                        StatusId = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        Description = record["Description"].As<string>()
                    };
                }
            );
        }

        public async Task<List<Status>> GetTripStatusesAsync(string tripId, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadStatus}]->(status:Status)
                RETURN status.id AS Id, 
                status.name AS Name, 
                status.description AS Description, 
                r.userId AS UserId",
                new
                {
                    userId,
                    tripId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Status
                    {
                        StatusId = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                    }).ToList();
                }
            );
        }

        public async Task<bool> DeleteTripStatusAsync(string tripId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadStatus}]->(s:Status)
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

        public async Task<Status> AssignEventStatusAsync(string eventId, string statusName, string destinationId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (d:Destination {{id: $destinationId}}), (e:Event {{id: $eventId}})
                OPTIONAL MATCH (e)-[r:{GraphRelations.Event.HadStatus}]->(oldStatus:Status)
                DELETE r 
                WITH e, d
                MERGE (newStatus:Status {{name: apoc.text.capitalize(toLower($statusName))}})
                ON CREATE SET newStatus.id = randomUUID(), 
                newStatus.description = COALESCE(newStatus.description, 'Default description for ' + $statusName)
                MERGE (e)-[newR:{GraphRelations.Event.HadStatus}]->(newStatus)
                ON CREATE SET newR.id = randomUUID()
                RETURN 
                    newStatus.id AS Id, 
                    newStatus.name AS Name, 
                    newStatus.description AS Description",
                new
                {
                    statusName,
                    eventId,
                    destinationId,
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new Status
                    {
                        StatusId = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>()
                    };
                }
            );
        }
        
        public async Task<List<Status>> GetEventStatusesAsync(string eventId, string destinationId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (d:Destination {{id: $destinationId}})
                MATCH (e:Event {{id: $eventId}})-[r:{GraphRelations.Event.HadStatus}]->(status:Status)
                RETURN status.id AS Id, 
                status.name AS Name, 
                status.description AS Description",
                new
                {
                    eventId,
                    destinationId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Status
                    {
                        StatusId = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                    }).ToList();
                }
            );
        }

        public async Task<bool> DeleteEventStatusAsync(string eventId, string destinationId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
        MATCH (d:Destination {{id: $destinationId}})
        MATCH (e:Event {{id: $eventId}})-[r:{GraphRelations.Event.HadStatus}]->(s:Status)
        DELETE r",
                new
                {
                    eventId,
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
