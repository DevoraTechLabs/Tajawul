using Tajawul.Helpers;

namespace Tajawul.Queries
{
        public static class SpokenLanguageQueries
        {


                public readonly static string GetSpokenLanguageByNameQuery = @"
                MATCH (c:SpokenLanguage {name: apoc.text.capitalizeAll($spokenLanguageName)})
                RETURN c.name AS name, c.id As id";

                public readonly static string CreateSpokenLanguageQuery = @"
                MERGE (c:SpokenLanguage {name: apoc.text.capitalizeAll($spokenLanguageName)})
                ON CREATE SET c.id = randomUUID()
                RETURN c.name AS name, c.id As id";

                public readonly static string CountSpokenLanguageRelationshipsQuery = $@"
                MATCH (c:SpokenLanguage {{name: apoc.text.capitalizeAll($spokenLanguageName)}})
                OPTIONAL MATCH (c)-[r:{GraphRelations.User.Speak}]-()
                RETURN COUNT(r) AS relCount";


                public readonly static string DeleteSpokenLanguageQuery = @"
                MATCH (c:SpokenLanguage {name: apoc.text.capitalizeAll($spokenLanguageName)})
                DELETE c RETURN true";


                public readonly static string GetAllSpokenLanguagesQuery = @"
                MATCH (c:SpokenLanguage)
                RETURN c.name AS name, c.id As id";


                public readonly static string UpdateSpokenLanguageNameQuery = @"
                MATCH (c:SpokenLanguage {name: apoc.text.capitalizeAll($oldName)})
                WHERE NOT EXISTS {
                    MATCH (other:SpokenLanguage {name: apoc.text.capitalizeAll($newName)})
                }
                SET c.name = apoc.text.capitalizeAll($newName)
                RETURN c.name AS name, c.id AS id";

                public readonly static string AddUserSpokenLanguagesQuery = $@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[k:{GraphRelations.User.Speak}]->(:SpokenLanguage)
                DELETE k
                WITH u
                UNWIND $spokenLanguages AS spokenLanguage
                MATCH (s:SpokenLanguage {{name: apoc.text.capitalizeAll(spokenLanguage)}})
                MERGE (u)-[r:{GraphRelations.User.Speak}]->(s)
                WITH collect(s.name) AS spokenLanguages, count(r) AS relationshipsCreated
                RETURN relationshipsCreated, spokenLanguages";

                // public readonly static string DeleteUserSpokenLanguageQuery = $@"
                //  MATCH (u:User {{id: $userId}})
                //  MATCH (s:SpokenLanguage {{name: apoc.text.capitalizeAll($spokenLanguageName)}})
                //  MATCH (u)-[r:{GraphRelations.User.Speak}]->(s)
                //  DELETE r
                //  RETURN COUNT(r) AS relCount;";


                // public readonly static string GetUserSpokenLanguagesQuery = $@"
                //  MATCH (u:User {{id: $userId}})
                //  MATCH (u)-[r:{GraphRelations.User.Speak}]->(s:SpokenLanguage)
                //  RETURN s.name AS name, s.id As id";

        }
}
