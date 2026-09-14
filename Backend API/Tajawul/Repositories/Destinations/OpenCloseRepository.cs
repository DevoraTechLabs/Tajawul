using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Services;

namespace Tajawul.Repositories.Destinations
{
    public class OpenCloseRepository
    {
        private readonly Neo4jService _neo4jService;

        public OpenCloseRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }

        public async Task<DestinationOpenClose> UpdateOpenCloseTimesAsync(string destinationId, TimeOnly openAt, TimeOnly closeAt)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (d:Destination {{id: $destinationId}})
                OPTIONAL MATCH (d)-[r1:{GraphRelations.Destination.OpenAt}]->(:Time)
                OPTIONAL MATCH (d)-[r2:{GraphRelations.Destination.CloseAt}]->(:Time)
                
                
                DELETE r1, r2
                
                WITH d
                OPTIONAL MATCH (o:Time {{value: $openAt}})
                OPTIONAL MATCH (c:Time {{value: $closeAt}})

                WITH d, o, c, $openAt AS openAt, $closeAt AS closeAt

                CALL apoc.do.when(
                    o IS NULL, 
                    'CREATE (newO:Time {{id: randomUUID(), value: $openAt}}) RETURN newO', 
                    'RETURN o AS newO', 
                    {{openAt: openAt, o: o}}
                ) YIELD value AS openTime

                CALL apoc.do.when(
                    c IS NULL, 
                    'CREATE (newC:Time {{id: randomUUID(), value: $closeAt}}) RETURN newC', 
                    'RETURN c AS newC', 
                    {{closeAt: closeAt, c: c}}
                ) YIELD value AS closeTime

                WITH d, openTime.newO AS openT, closeTime.newC AS closeT

                MERGE (d)-[rOpen:{GraphRelations.Destination.OpenAt}]->(openT)
                MERGE (d)-[rClose:{GraphRelations.Destination.CloseAt}]->(closeT)

                RETURN openT.value AS OpenAt, closeT.value AS CloseAt
                ",
                new {
                    destinationId,
                    openAt,
                    closeAt
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new DestinationOpenClose
                    {
                        OpenAt = TimeOnly.TryParse(record["OpenAt"].As<string>(), out var openAt)
                        ? openAt
                        : throw new InvalidOperationException("Open Time is required and cannot be null or invalid."),
                        CloseAt = TimeOnly.TryParse(record["CloseAt"].As<string>(), out var closeAt)
                        ? closeAt
                        : throw new InvalidOperationException("Close Time is required and cannot be null or invalid.")
                    };
                }
            );
        }
    }
}
