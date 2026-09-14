using Tajawul.Helpers;

namespace Tajawul.Queries
{
    public static class InterestQueries
    {

        public static readonly string CreateInterest = @"
                MERGE (c:Interest {name: apoc.text.capitalizeAll($name)})
                ON CREATE SET c.id = randomUUID()
                RETURN c.name AS name, c.id As id";

        public static readonly string GetInterestByName = @"
                MATCH (c:Interest {name: apoc.text.capitalizeAll($name)})
                RETURN c.name AS name, c.id As id";

        public static readonly string CountInterestRelationships = $@"
                MATCH (c:Interest {{name: apoc.text.capitalizeAll($name)}})
                OPTIONAL MATCH (c)-[r:{GraphRelations.User.InterestedIn}]-(u:User)
                RETURN COUNT(r) AS relCount";


        public static readonly string DeleteInterest = @"
                MATCH (c:Interest {name: apoc.text.capitalizeAll($name)})
                DELETE c RETURN true";


        public static readonly string UpdateInterestName = @"
                MATCH (c:Interest {name: apoc.text.capitalizeAll($oldName)})
                WHERE NOT EXISTS {
                    MATCH (other:Interest {name: apoc.text.capitalizeAll($newName)})
                }
                SET c.name = apoc.text.capitalizeAll($newName)
                RETURN c.name AS name, c.id AS id";


        public static readonly string GetAllInterests = @"
                MATCH (c:Interest)
                RETURN c.name AS name, c.id As id";


        public static readonly string AddUserInterests = $@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[K:{GraphRelations.User.InterestedIn}]->(c:Interest)
                DELETE K
                WITH u
                UNWIND $interestNames AS interestName
                MERGE (c:Interest {{name: apoc.text.capitalizeAll(interestName)}})
                ON CREATE SET c.id = randomUUID()


                // Create the relationship only if it doesn't exist
                MERGE (u)-[:{GraphRelations.User.InterestedIn}]->(c)

                RETURN DISTINCT c.name AS name, c.id AS id;";
               

        public static readonly string DeleteUserInterest = $@"
                MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.InterestedIn}]->
                (c:Interest {{name: apoc.text.capitalizeAll($interestName)}})
                DELETE r
                RETURN COUNT(r) AS relCount;";

        public static readonly string GetUserInterests = $@"
                MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.InterestedIn}]->
                (c:Interest)
                RETURN c.name AS name, c.id As id;";

    }
}
