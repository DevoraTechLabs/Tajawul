using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Neo4j.Driver;
using Tajawul.Helpers.Filters.SearchBar;
using Tajawul.Models.ViewModels.SearchBar;
using Tajawul.Services;

namespace Tajawul.Repositories
{
    public class SearchBarRepository
    {
        private readonly Neo4jService _neo4jService;

        public SearchBarRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        public async Task<List<UserSearchResultDto>> SearchUsersAsync(string query, int limit = 5)
        {
            var cypherQuery = $@"
                CALL db.index.fulltext.queryNodes('userSearch', $query) YIELD node, score
                WITH node, score, 
                    node.topTraveler AS topTraveler, 
                    node.createdDestinationCount AS createdDestinationCount, 
                    node.visitedDestinationCount AS visitedDestinationCount, 
                    node.editedDestinationCount AS editedDestinationCount, 
                    node.PostsCount AS PostsCount
                RETURN node.id AS id, 
                    node.firstName AS firstName, 
                    node.lastName AS lastName, 
                    node.bio AS bio, 
                    COALESCE(node.isTopTraveler, false) AS isTopTraveler, 
                    COALESCE(createdDestinationCount, 0) AS createdDestinationCount, 
                    COALESCE(visitedDestinationCount, 0) AS visitedDestinationCount, 
                    COALESCE(editedDestinationCount, 0) AS editedDestinationCount, 
                    COALESCE(PostsCount, 0) AS PostsCount, 
                    score
                ORDER BY score DESC, 
                        COALESCE(node.isTopTraveler, false) DESC, 
                        COALESCE(createdDestinationCount, 0) DESC, 
                        COALESCE(visitedDestinationCount, 0) DESC, 
                        COALESCE(editedDestinationCount, 0) DESC, 
                        COALESCE(PostsCount, 0) DESC
                LIMIT {limit}";
            return await _neo4jService.ExecuteReadAsync(cypherQuery,
                new { query },
                async result =>
                {
                    var records = await result.ToListAsync();
                    return records.Select(r => new UserSearchResultDto
                    {
                        Id = r["id"].As<string>(),
                        FirstName = r["firstName"].As<string>(),
                        LastName = r["lastName"].As<string>(),
                        IsTopTraveler = r["isTopTraveler"]?.As<bool>()
                    }).ToList();
                }) ?? [];
        }

        public async Task<List<DestinationSearchResultDto>> SearchDestinationsAsync(string query, SearchBarDestinationFilter? filters, int limit = 5)
        {
            var parameters = new Dictionary<string, object>();
            var cypher = new List<string>();

            bool useFullTextSearch = !string.IsNullOrWhiteSpace(query);

            if (useFullTextSearch)
            {
                cypher.Add("CALL db.index.fulltext.queryNodes('destinationSearch', $query) YIELD node, score");
                parameters["query"] = query;
            }
            else
            {
                cypher.Add("MATCH (node:Destination)");
                cypher.Add("WITH node, 0 AS score");
            }

            if (filters?.Type != null) { cypher.Add("MATCH (node)-[:HAD_TYPE]->(:Type {name: $type})"); parameters["type"] = filters.Type; }
            if (filters?.PriceRange != null) { cypher.Add("MATCH (node)-[:HAD_PRICE_RANGE]->(:PriceRange {name: $priceRange})"); parameters["priceRange"] = filters.PriceRange; }
            if (filters?.City != null) { cypher.Add("MATCH (node)-[:LOCATED_IN]->(:City {name: $city})"); parameters["city"] = filters.City; }
            if (filters?.Country != null) { cypher.Add("MATCH (node)-[:LOCATED_IN]->(:City)-[:LOCATED_IN]->(:Country {name: $country})"); parameters["country"] = filters.Country; }
            if (filters?.GroupSize != null) { cypher.Add("MATCH (node)-[:PREFERED_GROUP_SIZE]->(:GroupSize {name: $groupSize})"); parameters["groupSize"] = filters.GroupSize; }
            if (filters?.Tags != null && filters.Tags.Any()) { cypher.Add("MATCH (node)-[:HAD_TAG]->(tag:Tag)"); cypher.Add("WHERE tag.name IN $tags"); parameters["tags"] = filters.Tags; }
            if (filters?.Activities != null && filters.Activities.Any()) { cypher.Add("MATCH (node)-[:HAD_ACTIVITY]->(activity:Activity)"); cypher.Add("WHERE activity.name IN $activities"); parameters["activities"] = filters.Activities; }

            cypher.Add($@"
                WITH node, score,
                    node.isVerified AS isVerified,
                    node.visitorsCount AS visitorsCount,
                    node.followersCount AS followersCount,
                    node.averageRating AS averageRating,
                    node.favoritesCount AS favoritesCount,
                    node.wishesCount AS wishesCount,
                    node.eventsCount AS eventsCount,
                    node.reviewsCount AS reviewsCount
                RETURN DISTINCT node.id AS id,
                    node.name AS name,
                    COALESCE(isVerified, false) AS isVerified,
                    COALESCE(visitorsCount, 0) AS visitorsCount,
                    COALESCE(followersCount, 0) AS followersCount,
                    COALESCE(averageRating, 0.0) AS averageRating,
                    COALESCE(favoritesCount, 0) AS favoritesCount,
                    COALESCE(wishesCount, 0) AS wishesCount,
                    COALESCE(eventsCount, 0) AS eventsCount,
                    COALESCE(reviewsCount, 0) AS reviewsCount,
                    score
                ORDER BY score DESC,
                    COALESCE(isVerified, false) DESC,
                    COALESCE(visitorsCount, 0) DESC,
                    COALESCE(followersCount, 0) DESC,
                    COALESCE(averageRating, 0.0) DESC,
                    COALESCE(favoritesCount, 0) DESC,
                    COALESCE(wishesCount, 0) DESC,
                    COALESCE(eventsCount, 0) DESC,
                    COALESCE(reviewsCount, 0) DESC
                LIMIT {limit}");

            return await _neo4jService.ExecuteReadAsync(string.Join("\n", cypher), parameters, async result =>
            {
                var records = await result.ToListAsync();
                return records.Select(r => new DestinationSearchResultDto
                {
                    Id = r["id"].As<string>(),
                    Name = r["name"].As<string>(),
                    IsVerified = r["isVerified"]?.As<bool?>(),
                    VisitorsCount = r["visitorsCount"].As<int>(),
                    FollowersCount = r["followersCount"].As<int>(),
                    AverageRating = r["averageRating"].As<double>()
                }).ToList();
            }) ?? [];
        }

        public async Task<List<TripSearchResultDto>> SearchTripsAsync(string query, SearchBarTripFilter? filters, int limit = 5)
        {
            var parameters = new Dictionary<string, object>();
            var cypher = new List<string>();

            bool useFullTextSearch = !string.IsNullOrWhiteSpace(query);

            if (useFullTextSearch)
            {
                cypher.Add("CALL db.index.fulltext.queryNodes('tripSearch', $query) YIELD node, score");
                parameters["query"] = query;
            }
            else
            {
                cypher.Add("MATCH (node:Trip)");
                cypher.Add("WITH node, 0 AS score");
            }

            cypher.Add("MATCH (node)-[:HAD_VISIBILITY]->(:Visibility {name: 'Public'})");
            if (filters?.TripDuration != null) { cypher.Add("MATCH (node)-[:HAD_DURATION]->(:TripDuration {name: $duration})"); parameters["duration"] = filters.TripDuration; }
            if (filters?.PriceRange != null) { cypher.Add("MATCH (node)-[:HAD_PRICE_RANGE]->(:PriceRange {name: $priceRange})"); parameters["priceRange"] = filters.PriceRange; }
            if (filters?.Status != null) { cypher.Add("MATCH (node)-[:CURRENT_STATUS]->(:Status {name: $status})"); parameters["status"] = filters.Status; }
            if (filters?.Tags != null && filters.Tags.Any()) { cypher.Add("MATCH (node)-[:HAD_TAG]->(tag:Tag)"); cypher.Add("WHERE tag.name IN $tags"); parameters["tags"] = filters.Tags; }

           cypher.Add($@"
                WITH node, score,
                    node.cloneCount AS cloneCount,
                    node.favoriteCount AS favoriteCount,
                    node.wishedCount AS wishedCount
                RETURN DISTINCT node.id AS id,
                    node.title AS title,
                    COALESCE(node.description, '') AS description,
                    COALESCE(cloneCount, 0) AS cloneCount,
                    COALESCE(favoriteCount, 0) AS favoriteCount,
                    COALESCE(wishedCount, 0) AS wishedCount,
                    score
                ORDER BY score DESC,
                    COALESCE(cloneCount, 0) DESC,
                    COALESCE(favoriteCount, 0) DESC,
                    COALESCE(wishedCount, 0) DESC
                LIMIT {limit}");

            return await _neo4jService.ExecuteReadAsync(string.Join("\n", cypher), parameters, async result =>
            {
                var records = await result.ToListAsync();
                return records.Select(r => new TripSearchResultDto
                {
                    Id = r["id"].As<string>(),
                    Title = r["title"].As<string>(),
                    Description = r["description"].As<string>(),
                    CloneCount = r["cloneCount"].As<int>(),
                    FavoriteCount = r["favoriteCount"].As<int>(),
                    WishedCount = r["wishedCount"].As<int>()
                }).ToList();
            }) ?? [];
        }

        public async Task<List<EventSearchResultDto>> SearchEventsAsync(string query, SearchBarEventFilter? filters, int limit = 5)
        {
            var parameters = new Dictionary<string, object>();
            var cypher = new List<string>();

            bool useFullTextSearch = !string.IsNullOrWhiteSpace(query);

            if (useFullTextSearch)
            {
                cypher.Add("CALL db.index.fulltext.queryNodes('eventSearch', $query) YIELD node, score");
                parameters["query"] = query;
            }
            else
            {
                cypher.Add("MATCH (node:Event)");
                cypher.Add("WITH node, 0 AS score");
            }

            if (filters?.PriceRange != null) { cypher.Add("MATCH (node)-[:HAD_PRICE_RANGE]->(:PriceRange {name: $priceRange})"); parameters["priceRange"] = filters.PriceRange; }
            if (filters?.City != null) { cypher.Add("MATCH (node)-[:LOCATED_IN]->(:City {name: $city})"); parameters["city"] = filters.City; }
            if (filters?.Country != null) { cypher.Add("MATCH (node)-[:LOCATED_IN]->(:City)-[:LOCATED_IN]->(:Country {name: $country})"); parameters["country"] = filters.Country; }
            if (filters?.Tags != null && filters.Tags.Any()) { cypher.Add("MATCH (node)-[:HAD_TAG]->(tag:Tag)"); cypher.Add("WHERE tag.name IN $tags"); parameters["tags"] = filters.Tags; }
            if (filters?.Status != null) { cypher.Add("MATCH (node)-[:CURRENT_STATUS]->(:Status {name: $status})"); parameters["status"] = filters.Status; }
            cypher.Add($@"
                WITH node, score,
                    node.attendeesCount AS attendeesCount
                RETURN DISTINCT node.id AS id,
                    node.name AS name,
                    COALESCE(node.description, '') AS description,
                    COALESCE(attendeesCount, 0) AS attendeesCount,
                    score
                ORDER BY score DESC,
                    COALESCE(attendeesCount, 0) DESC
                LIMIT {limit}");
            return await _neo4jService.ExecuteReadAsync(string.Join("\n", cypher), parameters, async result =>
            {
                var records = await result.ToListAsync();
                return records.Select(r => new EventSearchResultDto
                {
                    Id = r["id"].As<string>(),
                    Name = r["name"].As<string>(),
                    Description = r["description"].As<string>(),
                    AttendeesCount = r["attendeesCount"]?.As<int?>()
                }).ToList();
            }) ?? [];
        }
    }
}