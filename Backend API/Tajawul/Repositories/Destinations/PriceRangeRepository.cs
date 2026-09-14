using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Services;
using static Tajawul.Helpers.GraphRelations;

namespace Tajawul.Repositories.Destinations
{
    public class PriceRangeRepository
    {
        private readonly Neo4jService _neo4jService;

        public PriceRangeRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<List<PriceRange>> GetDestinationPriceRangesAsync(string destinationId, string userId)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                  MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadPriceRange}]->(p:PriceRange)
                  RETURN p.id AS Id, p.name AS Range, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    userId = userId,
                    destinationId = destinationId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new PriceRange
                    {
                        PriceId = record["Id"].As<string>(),
                        Name = record["Range"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<PriceRange> AssignPriceRangeAsync(string range, string destinationId, string userId)
        {
            var currentDate = DateTime.UtcNow.ToString("o"); // ISO 8601 format

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (d:Destination {{id: $destinationId}}), (u:User {{id: $userId}}), (p:PriceRange)
                  WHERE toLower(p.name) = toLower($range)
                  WITH d, p, u
                  OPTIONAL MATCH (d)-[r:{GraphRelations.Destination.HadPriceRange}]->(p)
                  DELETE r
                  WITH d, u, p
                  MERGE (d)-[r:{GraphRelations.Destination.HadPriceRange}]->(p)
                  ON CREATE SET r.id = randomUUID(), r.date = $currentDate, r.userId = u.id
                  RETURN p.id AS Id, p.name AS Range, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    userId = userId,
                    range = range,
                    destinationId = destinationId,
                    currentDate = currentDate,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new PriceRange
                    {
                        PriceId = record["Id"].As<string>(),
                        Name = record["Range"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    };
                }
            );
        }

        public async Task<bool> RemoveDestinationPriceRangeAsync(string range, string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(

                $@"MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadPriceRange}]->(p:PriceRange)
                WHERE p.name = $range
                DELETE r",
                new
                {
                    userId = userId,
                    destinationId = destinationId,
                    range = range,
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
        public async Task<List<PriceRange>> GetEventPriceRangesAsync(string eventId, string destinationId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (d:Destination {{id: $destinationId}})-[:{GraphRelations.Destination.Organized}]->(e:Event {{id: $eventId}})-[r:{GraphRelations.Event.HadPriceRange}]->(p:PriceRange)
                   RETURN p.id AS Id, p.name AS Range, r.date AS Date",
                new
                {
                    destinationId = destinationId,
                    eventId = eventId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new PriceRange
                    {
                        PriceId = record["Id"].As<string>(),
                        Name = record["Range"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                            ? assignedDate
                            : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<PriceRange> AssignPriceRangeToEventAsync(string range, string eventId, string destinationId)
        {
            var currentDate = DateTime.UtcNow.ToString("o"); // ISO 8601 format

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (d:Destination {{id: $destinationId}})-[:{GraphRelations.Destination.Organized}]->(e:Event {{id: $eventId}}), (p:PriceRange)
                   WHERE toLower(p.name) = toLower($range)
                   WITH e, p, d
                   OPTIONAL MATCH (e)-[r:{GraphRelations.Event.HadPriceRange}]->(p)
                   DELETE r
                   WITH e, p
                   MERGE (e)-[r:{GraphRelations.Event.HadPriceRange}]->(p)
                   ON CREATE SET r.id = randomUUID(), r.date = $currentDate
                   RETURN p.id AS Id, p.name AS Range, r.date AS Date",
                new
                {
                    range = range,
                    eventId = eventId,
                    destinationId = destinationId,
                    currentDate = currentDate,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new PriceRange
                    {
                        PriceId = record["Id"].As<string>(),
                        Name = record["Range"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                            ? assignedDate
                            : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    };
                }
            );
        }
        public async Task<bool> RemoveEventPriceRangeAsync(string range, string eventId, string destinationId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"MATCH (d:Destination {{id: $destinationId}})-[:{GraphRelations.Destination.Organized}]->(e:Event {{id: $eventId}})-[r:{GraphRelations.Event.HadPriceRange}]->(p:PriceRange)
                   WHERE p.name = $range
                   DELETE r",
                new
                {
                    destinationId = destinationId,
                    eventId = eventId,
                    range = range,
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
        public async Task<PriceRange> AssignTripPriceRangeAsync(string range, string tripId, string userId)
        {
            var currentDate = DateTime.UtcNow.ToString("o"); // ISO 8601 format

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (t:Trip {{id: $tripId}}), (u:User {{id: $userId}}), (p:PriceRange)
                  WHERE toLower(p.name) = toLower($range)
                  WITH t, p, u
                  OPTIONAL MATCH (t)-[r:{GraphRelations.Trip.HadPriceRange}]->()
                  DELETE r
                  WITH t, u, p
                  MERGE (t)-[r:{GraphRelations.Trip.HadPriceRange}]->(p)
                  ON CREATE SET r.id = randomUUID(), r.date = $currentDate, r.userId = u.id
                  RETURN p.id AS Id, p.name AS Range, r.userId AS UserId, r.date AS Date
                ",

                new
                {
                    userId = userId,
                    range = range,
                    tripId = tripId,
                    currentDate = currentDate,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new PriceRange
                    {
                        PriceId = record["Id"].As<string>(),
                        Name = record["Range"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.Parse(record["Date"].As<string>())
                    };
                }
            );
        }

        public async Task<List<PriceRange>> GetTripPriceRangesAsync(string tripId, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                  MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadPriceRange}]->(p:PriceRange)
                  RETURN p.id AS Id, p.name AS Range, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    userId = userId,
                    tripId = tripId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new PriceRange
                    {
                        PriceId = record["Id"].As<string>(),
                        Name = record["Range"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.Parse(record["Date"].As<string>())
                    }).ToList();
                }
            );
        }
        public async Task<bool> RemoveTripPriceRangeAsync(string range, string tripId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(

                $@"MATCH (u:User {{id: $userId}})
                MATCH (t:Trip {{id: $tripId}})-[r:{GraphRelations.Trip.HadPriceRange}]->(p:PriceRange)
                WHERE p.name = $range
                DELETE r",

                new
                {
                    userId = userId,
                    tripId = tripId,
                    range = range,
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
