using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Services;
namespace Tajawul.Repositories.User.Interaction
{
    public class EventInteractionsRepository
    {
        private readonly Neo4jService _neo4jService;

        public EventInteractionsRepository(Neo4jService neo4jService)
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

        public async Task<int> InterestedInEventAsync(string eventId, string userId)
        {
            string relation = GraphRelations.User.InterestedIn;

            bool exist = await IsRelationExist(relation, userId, "User", eventId, "Event");

            if (exist)
                throw new Exception("Relation already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (e:Event {{id: $eventId}}) 
                WHERE NOT (u)-[:{relation}]->(e) 
                CREATE (u)-[i:{relation}]->(e)
                SET e.interestedInCount = e.interestedInCount + 1, i.date = datetime()
                RETURN e.interestedInCount AS InterestedInCount",
                new
                {
                    userId,
                    eventId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["InterestedInCount"].As<int>();
                }
            );
        }

        public async Task<int> NotInterestedInEventAsync(string eventId, string userId)
        {

            string relation = GraphRelations.User.InterestedIn;

            bool exist = await IsRelationExist(relation, userId, "User", eventId, "Event");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[i:{GraphRelations.User.InterestedIn}]->(e:Event {{id: $eventId}}) 
                DELETE i
                SET e.interestedInCount = e.interestedInCount - 1 
                RETURN e.interestedInCount AS InterestedInCount",
                new
                {
                    eventId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["InterestedInCount"].As<int>();
                }
            );
        }

        public async Task<int> AttendEventAsync(string eventId, string userId)
        {
            string relation = GraphRelations.User.Attended;

            bool exist = await IsRelationExist(relation, userId, "User", eventId, "Event");

            if (exist)
                throw new Exception("Relation already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                   MATCH (e:Event {{id: $eventId}}) 
                   WHERE NOT (u)-[:{relation}]->(e) 
                   CREATE (u)-[a:{relation}]->(e)
                   SET e.attendeesCount = e.attendeesCount + 1, a.date = datetime()
                   RETURN e.attendeesCount AS AttendeesCount",
                new
                {
                    userId,
                    eventId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["AttendeesCount"].As<int>();
                }
            );
        }

        public async Task<int> UnattendEventAsync(string eventId, string userId)
        {
            string relation = GraphRelations.User.Attended;

            bool exist = await IsRelationExist(relation, userId, "User", eventId, "Event");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[a:{relation}]->(e:Event {{id: $eventId}}) 
           DELETE a
           SET e.attendeesCount = e.attendeesCount - 1 
           RETURN e.attendeesCount AS AttendeesCount",
                new
                {
                    eventId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["AttendeesCount"].As<int>();
                }
            );
        }
    }
}
