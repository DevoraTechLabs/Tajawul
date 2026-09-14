using Neo4j.Driver;
using Neo4jClient.Cypher;
using System.Collections.Generic;
using Tajawul.Helpers;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Models.ViewModels.Trip;
using Tajawul.Services;

namespace Tajawul.Repositories.User.Interaction
{
    public class TripInteractionsRepository
    {
        private readonly Neo4jService _neo4jService;

        public TripInteractionsRepository(Neo4jService neo4jService)
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

        public async Task<UserTripStatus> GetUserStatusAsync(string tripId, string userId)
        {
            return new UserTripStatus
            {
                Clone = await IsRelationExist(GraphRelations.User.Cloned, userId, "User", tripId, "Trip"),
                Wish = await IsRelationExist(GraphRelations.User.Wished, userId, "User", tripId, "Trip"),
                Favorite = await IsRelationExist(GraphRelations.User.FavoritedTrip, userId, "User", tripId, "Trip")
            };
        }

        public async Task<int> WishTripAsync(string tripId, string userId)
        {

            string relation = GraphRelations.User.WishedTrip;

            bool exist = await IsRelationExist(relation, userId, "User", tripId, "Trip");

            if (exist)
                throw new Exception("Relation Already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}}) 
                MATCH (t:Trip {{id: $tripId}}) 
                WHERE NOT (u)-[:{relation}]->(t) 
                CREATE (u)-[w:{relation}]->(t)
                SET t.wishesCount = t.wishesCount + 1, w.date = datetime()
                RETURN t.wishesCount AS WishesCount",
                new
                {
                    tripId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["WishesCount"].As<int>();
                }
            );
        }

        public async Task<int> UnwishTripAsync(string tripId, string userId)
        {
            string relation = GraphRelations.User.WishedTrip;

            bool exist = await IsRelationExist(relation, userId, "User", tripId, "Trip");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[w:{relation}]->(t:Trip {{id: $tripId}}) 
                DELETE w 
                SET t.wishesCount = t.wishesCount - 1 
                RETURN t.wishesCount AS WishesCount",
                new
                {
                    tripId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["WishesCount"].As<int>();
                }
            );
        }

        public async Task<int> FavoriteTripAsync(string tripId, string userId)
        {
            string relation = GraphRelations.User.FavoritedTrip;

            bool exist = await IsRelationExist(relation, userId, "User", tripId, "Trip");

            if (exist)
                throw new Exception("Relation Already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (t:Trip {{id: $tripId}}) 
                WHERE NOT (u)-[:{relation}]->(t) 
                CREATE (u)-[fav:{relation}]->(t)
                SET t.favoritesCount = t.favoritesCount + 1, fav.date = datetime()
                RETURN t.favoritesCount AS FavoritesCount",
                new
                {
                    tripId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["FavoritesCount"].As<int>();
                }
            );
        }

        public async Task<int> UnfavoriteTripAsync(string tripId, string userId)
        {
            string relation = GraphRelations.User.FavoritedTrip;

            bool exist = await IsRelationExist(relation, userId, "User", tripId, "Trip");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[fav:{relation}]->(t:Trip {{id: $tripId}}) 
                DELETE fav 
                SET t.favoritesCount = t.favoritesCount - 1 
                RETURN t.favoritesCount AS FavoritesCount",
                new
                {
                    tripId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["FavoritesCount"].As<int>();
                }
            );
        }

    }
}