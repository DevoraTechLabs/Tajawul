using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Interfaces.Destinations;
using Tajawul.Models.Domain;
using Tajawul.Repositories.Destinations;
using Tajawul.Services;

namespace Tajawul.Repositories.Destinations
{
    public class DLocationRepository
    {
        private readonly Neo4jService _neo4jService;

        public DLocationRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }
        public async Task<City?> AssignLocationAsync(string destinationId, string countryName, string cityName)
        {
            var query = $@"
                        WITH apoc.text.capitalizeAll($cityName) AS cityName,
                             apoc.text.capitalizeAll($countryName) AS countryName
                        MATCH (d:Destination {{id: $destinationId}})
                        OPTIONAL MATCH (d)-[oldRel:{GraphRelations.Destination.LocatedIn}]->(oldCity:City)
                        DELETE oldRel

                        WITH d, cityName, countryName
                        MERGE (c:Country {{name: countryName}})
                        ON CREATE SET c.id = randomUUID()

                        WITH d, c, cityName
                        OPTIONAL MATCH (ci:City {{name: cityName}})-[:{GraphRelations.Destination.LocatedIn}]->(c)

                        WITH d, c, cityName, ci
                        FOREACH (ignore IN CASE WHEN ci IS NULL THEN [1] ELSE [] END |
                            CREATE (newCi:City {{name: cityName, id: randomUUID()}})
                            CREATE (newCi)-[:{GraphRelations.Destination.LocatedIn}]->(c)
                        )

                        WITH d, c, cityName
                        MATCH (ci:City {{name: cityName}})-[:{GraphRelations.Destination.LocatedIn}]->(c)
                        MERGE (d)-[:{GraphRelations.Destination.LocatedIn}]->(ci)

                        RETURN
                            c.name AS country, c.id AS countryId,
                            ci.name AS city, ci.id AS cityId;
                    ";

            return await _neo4jService.ExecuteReadAsync(
                query,
                new { destinationId, countryName, cityName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    var singleRecord = record.SingleOrDefault();
                    if (singleRecord == null) return null;

                    return new City
                    {
                        Id = singleRecord["cityId"].As<string>(),
                        Name = singleRecord["city"].As<string>(),
                        Country = new Country
                        {
                            Id = singleRecord["countryId"].As<string>(),
                            Name = singleRecord["country"].As<string>()
                        }
                    };
                }
            );
        }

        public async Task<City?> GetDestinationLocationAsync(string destinationId)
        {
            var query = $@"
                        MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.LocatedIn}]->(ci:City)-[{GraphRelations.Destination.LocatedIn}]->(c:Country)
                        RETURN 
                            c.name AS country, c.id AS countryId,
                            ci.name AS city, ci.id AS cityId
                    ";

            return await _neo4jService.ExecuteReadAsync(
                query,
                new { destinationId },
                async result =>
                {
                    var record = await result.ToListAsync();
                    var singleRecord = record.SingleOrDefault();
                    if (singleRecord == null) return null;

                    return new City
                    {
                        Id = singleRecord["cityId"].As<string>(),
                        Name = singleRecord["city"].As<string>(),
                        Country = new Country
                        {
                            Id = singleRecord["countryId"].As<string>(),
                            Name = singleRecord["country"].As<string>()
                        }
                    };
                });
        }

        public async Task<bool> DeleteLocationAssignmentsAsync(string destinationId)
        {
            var query = $@"
                        MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.LocatedIn}]->(ci:City)
                        DELETE r
                        RETURN COUNT(r) AS relCount
                    ";

            return await _neo4jService.ExecuteReadAsync(
                query,
                new { destinationId },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault()?["relCount"].As<int>() > 0;
                }
            );
        }

        public async Task<City?> AssignEventLocationAsync(string eventId, string countryName, string cityName)
                {
                    var query = $@"
                                WITH apoc.text.capitalizeAll($cityName) AS cityName,
                                     apoc.text.capitalizeAll($countryName) AS countryName
                                MATCH (e:Event {{id: $eventId}})
                                OPTIONAL MATCH (e)-[oldRel:{GraphRelations.Event.LocatedIn}]->(oldCity:City)
                                DELETE oldRel

                                WITH e, cityName, countryName
                                MERGE (c:Country {{name: countryName}})
                                ON CREATE SET c.id = randomUUID()

                                WITH e, c, cityName
                                OPTIONAL MATCH (ci:City {{name: cityName}})-[:{GraphRelations.Event.LocatedIn}]->(c)

                                WITH e, c, cityName, ci
                                FOREACH (ignore IN CASE WHEN ci IS NULL THEN [1] ELSE [] END |
                                    CREATE (newCi:City {{name: cityName, id: randomUUID()}})
                                    CREATE (newCi)-[:{GraphRelations.Event.LocatedIn}]->(c)
                                )

                                WITH e, c, cityName
                                MATCH (ci:City {{name: cityName}})-[:{GraphRelations.Event.LocatedIn}]->(c)
                                MERGE (e)-[:{GraphRelations.Event.LocatedIn}]->(ci)

                                RETURN
                                    c.name AS country, c.id AS countryId,
                                    ci.name AS city, ci.id AS cityId;
                                ";

                    return await _neo4jService.ExecuteReadAsync(
                        query,
                        new { eventId, countryName, cityName },
                        async result =>
                        {
                            var record = await result.ToListAsync();
                            var singleRecord = record.SingleOrDefault();
                            if (singleRecord == null) return null;

                            return new City
                            {
                                Id = singleRecord["cityId"].As<string>(),
                                Name = singleRecord["city"].As<string>(),
                                Country = new Country
                                {
                                    Id = singleRecord["countryId"].As<string>(),
                                    Name = singleRecord["country"].As<string>()
                                }
                            };
                        }
                    );
                }

        public async Task<City?> GetEventLocationAsync(string eventId)
        {
            var query = $@"
                        MATCH (e:Event {{id: $eventId}})-[r:{GraphRelations.Event.LocatedIn}]->(ci:City)-[:{GraphRelations.Event.LocatedIn}]->(c:Country)
                        RETURN 
                            c.name AS country, c.id AS countryId,
                            ci.name AS city, ci.id AS cityId
                        ";

            return await _neo4jService.ExecuteReadAsync(
                query,
                new { eventId },
                async result =>
                {
                    var record = await result.ToListAsync();
                    var singleRecord = record.SingleOrDefault();
                    if (singleRecord == null) return null;

                    return new City
                    {
                        Id = singleRecord["cityId"].As<string>(),
                        Name = singleRecord["city"].As<string>(),
                        Country = new Country
                        {
                            Id = singleRecord["countryId"].As<string>(),
                            Name = singleRecord["country"].As<string>()
                        }
                    };
                });
        }

        public async Task<bool> DeleteEventLocationAssignmentsAsync(string eventId)
        {
            var query = $@"
                        MATCH (e:Event {{id: $eventId}})-[r:{GraphRelations.Event.LocatedIn}]->(ci:City)
                        DELETE r
                        RETURN COUNT(r) AS relCount
                        ";

            return await _neo4jService.ExecuteReadAsync(
                query,
                new { eventId },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault()?["relCount"].As<int>() > 0;
                }
            );
        }
    }
}
