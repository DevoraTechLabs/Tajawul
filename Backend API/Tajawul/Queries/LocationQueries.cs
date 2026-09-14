using Tajawul.Helpers;
using Tajawul.Models.Domain;

namespace Tajawul.Queries
{
    public static class LocationQueries
    {

        public static readonly string CreateCountryQuery = @"
                MERGE (c:Country {name: apoc.text.capitalizeAll($name)})
                ON CREATE SET c.id = randomUUID()
                RETURN c.name AS name, c.id As id;";


        public static readonly string CreateCityQuery = $@"
                WITH apoc.text.capitalizeAll($countryName) AS countryName,
                     apoc.text.capitalizeAll($cityName) AS cityName
                MERGE (cou:Country {{name: countryName}})
                ON CREATE SET cou.id = randomUUID()
    
                WITH cou, cityName
                OPTIONAL MATCH (c:City)-[:{GraphRelations.Location.LocatedIn}]->(cou)
                WHERE c.name = cityName
    
                WITH cou, cityName, c
                FOREACH (ignore IN CASE WHEN c IS NULL THEN [1] ELSE [] END | 
                    CREATE (newC:City {{name: cityName, id: randomUUID()}})
                    CREATE (newC)-[:{GraphRelations.Location.LocatedIn}]->(cou)
                )
    
                WITH cou, cityName
                MATCH (city:City)-[:{GraphRelations.Location.LocatedIn}]->(cou)
                WHERE city.name = cityName
    
                RETURN city.name AS cityName, city.id AS cityId, 
                       cou.name AS countryName, cou.id AS countryId";




        public static readonly string CountCountryLocatedInQuery = $@"
                MATCH (c:Country {{name: apoc.text.capitalizeAll($name)}})
                OPTIONAL MATCH (c)<-[r:{GraphRelations.Location.LocatedIn}]-(ci:City)
                RETURN COUNT(r) AS relCount";


        public static readonly string DeleteCountryQuery = @"
                MATCH (c:Country {name: apoc.text.capitalizeAll($name)})
                DELETE c RETURN true";


        public static readonly string CountCityLocatedInQuery = $@"
                MATCH (c:City {{name: apoc.text.capitalizeAll($cityName)}})
                OPTIONAL MATCH (c)<-[:{GraphRelations.Location.LocatedIn}]-(u:User)
                MATCH (c)-[r:{GraphRelations.Location.LocatedIn}]->
                                (cou:Country {{name: apoc.text.capitalizeAll($countryName)}})
                RETURN c AS city, COUNT(u) AS userCount";

        public static readonly string DeleteCityQuery = $@"
                MATCH (c:City {{name: apoc.text.capitalizeAll($cityName)}})
                MATCH (c)-[r:{GraphRelations.Location.LocatedIn}]->
                                (cou:Country {{name: apoc.text.capitalizeAll($countryName)}})
                DELETE r, c
                RETURN true";


        public static readonly string GetCountryByNameQuery = @"
                MATCH (c:Country {name: apoc.text.capitalizeAll($name)}) 
                RETURN c.name AS name, c.id As id";


        public static readonly string GetCityByNameQuery = $@"
                MATCH (c:City {{name: apoc.text.capitalizeAll($cityName)}})
                MATCH (c)-[r:{GraphRelations.Location.LocatedIn}]->
                                (cou:Country {{name: apoc.text.capitalizeAll($countryName)}})
                RETURN c.name AS name, c.id As id, cou.name AS countryName, cou.id AS countryId";

        public static readonly string GetAllCountriesQuery = @"
                 MATCH (c:Country) RETURN c.name AS name, c.id As id";

        public static readonly string GetAllCitiesQuery = $@"
                MATCH (c:City)
                MATCH (c)-[r:{GraphRelations.Location.LocatedIn}]->(cou:Country)
                RETURN c.name AS name, c.id As id , cou.name AS countryName, cou.id AS countryId";


        public static readonly string UpdateCountryName = @"
                MATCH (c:Country {name: apoc.text.capitalizeAll($oldName)})
                WHERE NOT EXISTS {
                    MATCH (:Country {name: apoc.text.capitalizeAll($newName)})
                }
                SET c.name = apoc.text.capitalizeAll($newName)
                RETURN c.name AS name, c.id AS id";


        public static readonly string UpdateCityName = $@"
                MATCH (c:City {{name: apoc.text.capitalizeAll($oldName)}})
                MATCH (c)-[r:{GraphRelations.Location.LocatedIn}]->(cou:Country {{name: apoc.text.capitalizeAll($countryName)}})
                WHERE NOT EXISTS {{
                MATCH(:City {{name: apoc.text.capitalizeAll($newName)}})-[:{GraphRelations.Location.LocatedIn}]->
                                (:Country {{name: apoc.text.capitalizeAll($countryName)}})
                }}
                SET c.name = apoc.text.capitalizeAll($newName)
                RETURN c.name AS name, c.id AS id, cou.name AS countryName, cou.id AS countryId";


        public static readonly string AddUserLocationQuery = $@"
                WITH apoc.text.capitalizeAll($cityName) AS cityName,
                     apoc.text.capitalizeAll($countryName) AS countryName
                MATCH (u:User {{id: $id}})
                OPTIONAL MATCH (u)-[oldRel:{GraphRelations.Location.LocatedIn}]->(oldCity:City)
                DELETE oldRel

                WITH u, cityName, countryName
                MERGE (c:Country {{name: countryName}})
                ON CREATE SET c.id = randomUUID()

                WITH u, c, cityName
                OPTIONAL MATCH (ci:City {{name: cityName}})-[:{GraphRelations.Location.LocatedIn}]->(c)

                WITH u, c, cityName, ci
                FOREACH (ignore IN CASE WHEN ci IS NULL THEN [1] ELSE [] END |
                    CREATE (newCi:City {{name: cityName, id: randomUUID()}})
                    CREATE (newCi)-[:{GraphRelations.Location.LocatedIn}]->(c)
                )

                WITH u, c, cityName
                MATCH (ci:City {{name: cityName}})-[:{GraphRelations.Location.LocatedIn}]->(c)
                MERGE (u)-[:{GraphRelations.Location.LocatedIn}]->(ci)

                RETURN
                    c.name AS country, c.id AS countryId,
                    ci.name AS city, ci.id AS cityId";


        public static readonly string GetUserLocationQuery = $@"
                MATCH (u:User {{id: $id}})
                OPTIONAL MATCH (u)-[:{GraphRelations.Location.LocatedIn}]->(ci:City)
                OPTIONAL MATCH (ci)-[:{GraphRelations.Location.LocatedIn}]->(c:Country)
                RETURN 
                c.name AS country, c.id AS countryId,
                ci.name AS city, ci.id AS cityId";


        public static readonly string DeleteUserLocation = $@"
                MATCH (u:User {{id: $id}})-[r:{GraphRelations.Location.LocatedIn}]->(ci:City)
                DELETE r
                RETURN COUNT(r) AS relCount";
    }
}
