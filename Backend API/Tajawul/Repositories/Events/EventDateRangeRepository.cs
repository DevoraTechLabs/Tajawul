using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.Events;
using Tajawul.Services;

namespace Tajawul.Repositories.Events
{
    public class EventDateRangeRepository
    {
        private readonly Neo4jService _neo4jService;

        public EventDateRangeRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }
        public async Task<EventDateRange> SetEventDatesAsync(string eventId, DateTime startOn, DateTime endOn)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
        MATCH (e:Event {{id: $eventId}})
        OPTIONAL MATCH (e)-[r1:{GraphRelations.Event.StartedOn}]->(:DateTime)
        OPTIONAL MATCH (e)-[r2:{GraphRelations.Event.EndedOn}]->(:DateTime)
        DELETE r1, r2

        WITH e
        OPTIONAL MATCH (startD:DateTime {{value: $startOn}})
        OPTIONAL MATCH (endD:DateTime {{value: $endOn}})

        WITH e, startD, endD, $startOn AS startOn, $endOn AS endOn

        CALL apoc.do.when(
            startD IS NULL, 
            'CREATE (newStartD:DateTime {{id: randomUUID(), value: $startOn}}) RETURN newStartD', 
            'RETURN startD AS newStartD', 
            {{startOn: startOn, startD: startD}}
        ) YIELD value AS startDate

        CALL apoc.do.when(
            endD IS NULL, 
            'CREATE (newEndD:DateTime {{id: randomUUID(), value: $endOn}}) RETURN newEndD', 
            'RETURN endD AS newEndD', 
            {{endOn: endOn, endD: endD}}
        ) YIELD value AS endDate

        WITH e, startDate.newStartD AS startD, endDate.newEndD AS endD

        MERGE (e)-[rStarted:{GraphRelations.Event.StartedOn}]->(startD)
        MERGE (e)-[rEnded:{GraphRelations.Event.EndedOn}]->(endD)

        RETURN startD.value AS StartOn, endD.value AS EndOn
        ",
                new { eventId, startOn, endOn },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return new EventDateRange
                    {
                        StartOn = DateTime.TryParse(record["StartOn"].As<string>(), out var startDate)
                            ? startDate
                            : throw new InvalidOperationException("Start Date is required and cannot be null or invalid."),
                        EndOn = DateTime.TryParse(record["EndOn"].As<string>(), out var endDate)
                            ? endDate
                            : throw new InvalidOperationException("End Date is required and cannot be null or invalid.")
                    };
                }
            );
        }

    }
}
