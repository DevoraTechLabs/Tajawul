using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs;
using Tajawul.Services;
using static Tajawul.Helpers.GraphRelations;

namespace Tajawul.Repositories.Destinations
{
    public class TagRepository
    {
        private readonly Neo4jService _neo4jService;

        public TagRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }

        public async Task<bool> IsRelationExist(string relation, string initialId, string initialLabel, string terminalId, string terminalLabel)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (a:{initialLabel} {{id: $initialId}})-[r:{relation}]->(b:{terminalLabel} {{id: $terminalId}}) 
                RETURN COUNT(r) > 0 AS RelationExists",
                new
                {
                    initialId,
                    terminalId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["RelationExists"].As<bool>();
                }
            );
        }

        public async Task<bool> IsNodeExist(string nodeId, string nodeLabel)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (n:{nodeLabel} {{id: $nodeId}})
                RETURN COUNT(n) > 0 AS NodeExists",
                new
                {
                    nodeId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["NodeExists"].As<bool>();
                }
            );
        }

        public async Task<List<string>> AssignTagsAsync(List<string> tagNames, string entityId, string relationName)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (n)
                WHERE n.id = $entityId
                OPTIONAL MATCH (n)-[r:{relationName}]->(t:Tag)
                DELETE r
                WITH n
                UNWIND $tagNames AS tagName
                MERGE (t:Tag {{name: apoc.text.capitalizeAll(tagName)}})
                ON CREATE SET t.id = randomUUID()


                // Create the relationship only if it doesn't exist
                MERGE (n)-[:{relationName}]->(t)

                RETURN DISTINCT t.name AS name, t.id AS id;",
                new
                {
                    entityId,
                    tagNames
                },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.Select(x => x["name"].As<string>()).ToList();
                }
                ) ?? [];
        }

        public async Task<List<string>> GetEntityTagsAsync(string entityId, string relationName)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (n)
                WHERE n.id = $entityId
                MATCH (n)-[r:{relationName}]->(t:Tag)
                RETURN t.name AS name, t.id As id;",
                new
                {
                    entityId
                },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.Select(x => x["name"].As<string>()).ToList();
                }
                ) ?? [];
        }

        public async Task<Tag> AssignDestinationTagAsync(string tagName, string destinationId, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (d:Destination {{id: $destinationId}}), (u:User {{id: $userId}})
                  OPTIONAL MATCH (t:Tag)
                  WHERE toLower(t.name) = toLower($tagName)

                  WITH d, t, u, apoc.text.capitalize(toLower($tagName)) AS capitalizedTagName
                  // If the tag doesn't exist, create it
                  CALL apoc.do.when(
                      t IS NULL,
                      'CREATE (newT:Tag {{id: randomUUID(), name: $capitalizedTagName}}) RETURN newT',
                      'RETURN t AS newT',
                      {{capitalizedTagName: capitalizedTagName, t: t}}
                  ) YIELD value

                  WITH d, u, value.newT AS tag

                  MERGE (d)-[r:{GraphRelations.Destination.HadTag}]->(tag)
                  ON CREATE SET r.id = randomUUID(), r.date = datetime(), r.userId = u.id

                  WITH u, d, tag, r

                  MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                  SET e.date = datetime()

                  RETURN tag.id AS Id, tag.name AS TagName, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    userId,
                    tagName,
                    destinationId
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Tag
                    {
                        TagId = record["Id"].As<string>(),
                        Name = record["TagName"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    };
                }
            );
        }

        public async Task<List<Tag>> GetDestinationTagsAsync(string destinationId)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"
                  MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadTag}]->(tag:Tag)
                  RETURN tag.id AS Id, tag.name AS Name, r.userId AS UserId, r.date AS Date",
                new
                {
                    destinationId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Tag
                    {
                        TagId = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<bool> DeleteDestinationTagAsync(string tagName, string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadTag}]->(t:Tag)
                WHERE toLower(t.name) = toLower($tagName)
                DELETE r

                WITH u, d

                MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                SET e.date = datetime()
                ",
                new
                {
                    userId,
                    destinationId,
                    tagName,
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

        public async Task<Tag> AssignTripTagAsync(string tagName, string tripId, string userId)
        {
            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            var tripExist = await IsNodeExist(tripId, "Trip");
            if (!tripExist)
                throw new Exception("Trip not found");

            var owned = await IsRelationExist(GraphRelations.User.CreatedTrip, userId, "User", tripId, "Trip");
            var cloned = await IsRelationExist(GraphRelations.User.Cloned, userId, "User", tripId, "Trip");

            if (!owned && !cloned)
                throw new Exception("User not authorized to edit this trip");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (t:Trip {{id: $tripId}}), (u:User {{id: $userId}})
                  OPTIONAL MATCH (tag:Tag)
                  WHERE toLower(tag.name) = toLower($tagName)

                  WITH t, tag, u, apoc.text.capitalize(toLower($tagName)) AS capitalizedTagName
                  // If the tag doesn't exist, create it
                  CALL apoc.do.when(
                      tag IS NULL,
                      'CREATE (newTag:Tag {{id: randomUUID(), name: $capitalizedTagName}}) RETURN newTag',
                      'RETURN tag AS newTag',
                      {{capitalizedTagName: capitalizedTagName, tag: tag}}
                  ) YIELD value

                  WITH t, u, value.newTag AS tag

                  MERGE (t)-[r:{GraphRelations.Trip.HadTag}]->(tag)
                  ON CREATE SET r.id = randomUUID(), r.date = datetime(), r.userId = u.id
                  RETURN tag.id AS Id, tag.name AS TagName, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    userId,
                    tagName,
                    tripId,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Tag
                    {
                        TagId = record["Id"].As<string>(),
                        Name = record["TagName"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.Parse(record["Date"].As<string>())
                    };
                }
            );
        }

        public async Task<List<Tag>> GetTripTagsAsync(string tripId, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
          MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadTag}]->(tag:Tag)
          RETURN tag.id AS Id, tag.name AS Name, r.userId AS UserId, r.date AS Date",
                new
                {
                    userId,
                    tripId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Tag
                    {
                        TagId = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.Parse(record["Date"].As<string>())
                    }).ToList();
                }
            );
        }

        public async Task<bool> DeleteTripTagAsync(string tagName, string tripId, string userId)
        {
            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            var tripExist = await IsNodeExist(tripId, "Trip");
            if (!tripExist)
                throw new Exception("Trip not found");

            var owned = await IsRelationExist(GraphRelations.User.CreatedTrip, userId, "User", tripId, "Trip");
            var cloned = await IsRelationExist(GraphRelations.User.Cloned, userId, "User", tripId, "Trip");

            if (!owned && !cloned)
                throw new Exception("User not authorized to edit this trip");

            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
        MATCH (u:User {{id: $userId}})
        MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadTag}]->(tag:Tag)
        WHERE toLower(tag.name) = toLower($tagName)
        DELETE r",
                new
                {
                    userId,
                    tripId,
                    tagName,
                }
            );

            return summary.Counters.RelationshipsDeleted > 0;
        }

        public async Task<Tag> AssignEventTagAsync(string tagName, string eventId, string destinationId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (e:Event {{id: $eventId}}), (d:Destination {{id: $destinationId}})
          OPTIONAL MATCH (t:Tag)
          WHERE toLower(t.name) = toLower($tagName)

          WITH e, t, d, apoc.text.capitalize(toLower($tagName)) AS capitalizedTagName
          // If the tag doesn't exist, create it
          CALL apoc.do.when(
              t IS NULL,
              'CREATE (newT:Tag {{id: randomUUID(), name: $capitalizedTagName}}) RETURN newT',
              'RETURN t AS newT',
              {{capitalizedTagName: capitalizedTagName, t: t}}
          ) YIELD value

          WITH e, d, value.newT AS tag

          MERGE (e)-[r:{GraphRelations.Event.HadTag}]->(tag)
          ON CREATE SET r.id = randomUUID(), r.date = datetime(), r.destinationId = d.id

          WITH d, e, tag, r  // Added tag and r here to carry them forward

          MERGE (d)-[ed:{GraphRelations.Destination.Edited}]->(e)
          SET ed.date = datetime()

          RETURN tag.id AS Id, tag.name AS TagName, r.date AS Date
        ",
                new
                {
                    destinationId,
                    tagName,
                    eventId
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Tag
                    {
                        TagId = record["Id"].As<string>(),
                        Name = record["TagName"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    };
                }
            );
        }

        public async Task<List<Tag>> GetEventTagsAsync(string eventId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                  MATCH (e:Event {{id: $eventId}})-[r:{GraphRelations.Event.HadTag}]->(tag:Tag)
                  RETURN tag.id AS Id, tag.name AS Name, r.destinationId AS DestinationId, r.date AS Date",
                new
                {
                    eventId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Tag
                    {
                        TagId = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<bool> DeleteEventTagAsync(string tagName, string eventId, string destinationId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                    MATCH (d:Destination {{id: $destinationId}})
                    MATCH (e:Event {{id: $eventId}})-[r:{GraphRelations.Event.HadTag}]->(t:Tag)
                    WHERE toLower(t.name) = toLower($tagName)
                    DELETE r

                    WITH d, e

                    MERGE (d)-[ed:{GraphRelations.Destination.Edited}]->(e)
                    SET ed.date = datetime()
                    ",
                new
                {
                    destinationId,
                    eventId,
                    tagName,
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

        public async Task<List<TagDto>> GetAllTagsAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (tag:Tag)<-[r:{GraphRelations.User.HadTag}]-()
                RETURN tag.id AS Id, tag.name AS Name, COUNT(r) AS RelationCount
                ORDER BY RelationCount DESC
                LIMIT 50",
                new { },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new TagDto
                    {
                        Name = record["Name"].As<string>()
                    }).ToList();
                }
            ) ?? [];
        }
    }
}