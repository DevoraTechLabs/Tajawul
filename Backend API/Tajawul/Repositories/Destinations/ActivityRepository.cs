using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Models.ViewModels.SearchBar;
using Tajawul.Services;

namespace Tajawul.Repositories.Destinations
{
    public class ActivityRepository
    {
        private readonly Neo4jService _neo4jService;

        public ActivityRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }

        public async Task<List<Activity>> GetDestinationActivitiesAsync(string destinationId)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"
                  MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadActivity}]->(activity:Activity)
                  RETURN activity.id AS Id, activity.name AS ActivityName, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    destinationId,
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Activity
                    {
                        ActivityId = record["Id"].As<string>(),
                        Name = record["ActivityName"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<List<ActivityDto>> GetAllActivitiesAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (activity:Activity)
                OPTIONAL MATCH (activity)<-[r1:{GraphRelations.Destination.HadActivity}]-(:Destination)
                OPTIONAL MATCH (activity)<-[r2:{GraphRelations.User.PreferedActivity}]-(:User)
                RETURN activity.name AS Name, (COALESCE(COUNT(r1), 0) + COALESCE(COUNT(r2), 0)) AS RelationCount
                ORDER BY RelationCount DESC
                LIMIT 50",
                new { },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new ActivityDto
                    {
                        Name = record["Name"].As<string>()
                    }).ToList();
                }
            ) ?? [];
        }

        public async Task<Activity> AssignActivityAsync(string activityName, string destinationId, string userId)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (d:Destination {{id: $destinationId}}), (u:User {{id: $userId}})
                  OPTIONAL MATCH (a:Activity)
                  WHERE toLower(a.name) = toLower($activityName)

                  WITH d, a, u, apoc.text.capitalize(toLower($activityName)) AS capitalizedActivityName

                  CALL apoc.do.when(
                      a IS NULL,
                      'CREATE (newA:Activity {{id: randomUUID(), name: $capitalizedActivityName}}) RETURN newA',
                      'RETURN a AS newA',
                      {{capitalizedActivityName: capitalizedActivityName, a: a}}
                  ) YIELD value

                  WITH d, u, value.newA AS activity

                  MERGE (d)-[r:{GraphRelations.Destination.HadActivity}]->(activity)
                  ON CREATE SET r.id = randomUUID(), r.date = datetime(), r.userId = u.id
                
                  MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                  SET e.date = datetime()
                    
                  RETURN activity.id AS Id, activity.name AS ActivityName, r.userId AS UserId, r.date AS Date
                ",
                new
                {
                    userId,
                    activityName,
                    destinationId
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Activity
                    {
                        ActivityId = record["Id"].As<string>(),
                        Name = record["ActivityName"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        AssignedDate = DateTime.TryParse(record["Date"].As<string>(), out var assignedDate)
                        ? assignedDate
                        : throw new InvalidOperationException("Date is required and cannot be null or invalid.")
                    };
                }
            );
        }

        public async Task<bool> DeleteDestinationActivityAsync(string activityName, string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})-[r:{GraphRelations.Destination.HadActivity}]->(a:Activity)
                WHERE toLower(a.name) = toLower($activityName)
                DELETE r
                
                WITH u, d

                MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                SET e.date = datetime()
                ",
                new
                {
                    userId,
                    destinationId,
                    activityName,
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
