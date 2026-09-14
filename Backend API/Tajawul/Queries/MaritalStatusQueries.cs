using Tajawul.Helpers;

namespace Tajawul.Queries
{
        public class MaritalStatusQueries
        {


                public readonly static string CreateMaritalStatusQuery = @"
                MERGE (c:MaritalStatus {name: apoc.text.capitalizeAll($maritalStatusName)})
                ON CREATE SET c.id = randomUUID()
                RETURN c.name AS name, c.id As id";


                public readonly static string GetMaritalStatusByNameQuery = @"
                MATCH (c:MaritalStatus {name: apoc.text.capitalizeAll($maritalStatusName)})
                RETURN c.name AS name, c.id As id";


                public readonly static string CountMaritalStatusQuery = $@"
                MATCH (c:MaritalStatus {{name: apoc.text.capitalizeAll($maritalStatusName)}})
                OPTIONAL MATCH (c)-[r:{GraphRelations.User.HasMaritalStatus}]-(:User)
                RETURN COUNT(r) AS relCount";


                public readonly static string DeleteMaritalStatusQuery = @"
                MATCH (c:MaritalStatus {name: apoc.text.capitalizeAll($maritalStatusName)})
                DELETE c RETURN true";


                public readonly static string GetAllMaritalStatusQuery = @"
                MATCH (c:MaritalStatus)
                RETURN c.name AS name, c.id As id";


                public readonly static string UpdateMaritalStatusNameQuery = @"
                MATCH (c:MaritalStatus {name: apoc.text.capitalizeAll($oldName)})
                WHERE NOT EXISTS {
                    MATCH (other:MaritalStatus {name: apoc.text.capitalizeAll($newName)})
                }
                SET c.name = apoc.text.capitalizeAll($newName)
                RETURN c.name AS name, c.id AS id";


                public readonly static string AddUserMaritalStatusQuery = $@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HasMaritalStatus}]->(oldStatus:MaritalStatus)
                OPTIONAL MATCH (m:MaritalStatus {{name: apoc.text.capitalizeAll($maritalStatusName)}})
                WITH u, m, oldStatus
                CALL apoc.do.when(
                        m IS NOT NULL,
                        'OPTIONAL MATCH (u)-[r:{GraphRelations.User.HasMaritalStatus}]->() DELETE r
                        MERGE (u)-[r_new:{GraphRelations.User.HasMaritalStatus}]->(m)
                        RETURN r_new AS rel',
                        'RETURN oldStatus.name AS name, oldStatus.id AS id, null AS rel',
                        {{ u: u, m: m, oldStatus: oldStatus }}
                ) YIELD value
                RETURN count(value.rel) AS relationshipsCreated";

                // public readonly static string DeleteUserMaritalStatusQuery = $@"
                // MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.HasMaritalStatus}]->
                //                                     (m:MaritalStatus)
                // DELETE r RETURN COUNT(r) AS relCount;";


                // public readonly static string GetUserMaritalStatusQuery = $@"
                // MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.HasMaritalStatus}]->
                //                                     (m:MaritalStatus)
                // RETURN m.name AS name, m.id As id";


        }
}
