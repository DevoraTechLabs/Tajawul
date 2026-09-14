using Neo4j.Driver;
using NetTopologySuite.Operation.Relate;
using Tajawul.Helpers;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Services;

namespace Tajawul.Repositories.User.Interaction
{
    public class DestinationInteractionsRepository
    {

        private readonly Neo4jService _neo4jService;

        public DestinationInteractionsRepository(Neo4jService neo4jService)
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


        public async Task<UserDestinationStatus> GetUserStatusAsync(string destinationId, string userId)
        {
 
            return new UserDestinationStatus
            {
                Follow = await IsRelationExist(GraphRelations.User.Followed, userId, "User", destinationId, "Destination"),
                Visit = await IsRelationExist(GraphRelations.User.Visited, userId, "User", destinationId, "Destination"),
                Wish = await IsRelationExist(GraphRelations.User.Wished, userId, "User", destinationId, "Destination"),
                Favorite = await IsRelationExist(GraphRelations.User.FavoritedDestination, userId, "User", destinationId, "Destination")
            };
        }

        public async Task<int> FollowDestinationAsync(string destinationId, string userId)
        {
            string relation = GraphRelations.User.Followed;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (exist)
                throw new Exception("Relation already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}}) 
                WHERE NOT (u)-[:{relation}]->(d) 
                CREATE (u)-[f:{relation}]->(d)
                SET d.followersCount = d.followersCount + 1, f.date = datetime()
                RETURN d.followersCount AS FollowersCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["FollowersCount"].As<int>();
                }
            );
        }

        public async Task<int> UnfollowDestinationAsync(string destinationId, string userId)
        {

            string relation = GraphRelations.User.Followed;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[f:{GraphRelations.User.Followed}]->(d:Destination {{id: $destinationId}}) 
                DELETE f 
                SET d.followersCount = d.followersCount - 1 
                RETURN d.followersCount AS FollowersCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["FollowersCount"].As<int>();
                }
            );
        }

        public async Task<int> VisitDestinationAsync(string destinationId, string userId)
        {

            string relation = GraphRelations.User.Visited;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (exist)
                throw new Exception("Relation Already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}}) 
                WHERE NOT (u)-[:{relation}]->(d) 
                CREATE (u)-[v:{relation}]->(d)
                SET d.visitorsCount = d.visitorsCount + 1, v.date = datetime()
                RETURN d.visitorsCount AS VisitorsCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["VisitorsCount"].As<int>();
                }
            );
        }

        public async Task<int> UnVisitDestinationAsync(string destinationId, string userId)
        {

            string relation = GraphRelations.User.Visited;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[v:{GraphRelations.User.Visited}]->(d:Destination {{id: $destinationId}}) 
                DELETE v 
                SET d.visitorsCount = d.visitorsCount - 1 
                RETURN d.visitorsCount AS VisitorsCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["VisitorsCount"].As<int>();
                }
            );
        }

        public async Task<int> WishDestinationAsync(string destinationId, string userId)
        {

            string relation = GraphRelations.User.Wished;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (exist)
                throw new Exception("Relation Already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}}) 
                MATCH (d:Destination {{id: $destinationId}}) 
                WHERE NOT (u)-[:{relation}]->(d) 
                CREATE (u)-[w:{relation}]->(d)
                SET d.wishesCount = d.wishesCount + 1, w.date = datetime()
                RETURN d.wishesCount AS WishesCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["WishesCount"].As<int>();
                }
            );
        }

        public async Task<int> UnwishDestinationAsync(string destinationId, string userId)
        {

            string relation = GraphRelations.User.Wished;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[w:{relation}]->(d:Destination {{id: $destinationId}}) 
                DELETE w 
                SET d.wishesCount = d.wishesCount - 1 
                RETURN d.wishesCount AS WishesCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["WishesCount"].As<int>();
                }
            );
        }

        public async Task<int> FavoriteDestinationAsync(string destinationId, string userId)
        {

            string relation = GraphRelations.User.FavoritedDestination;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (exist)
                throw new Exception("Relation Already exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}}) 
                WHERE NOT (u)-[:{relation}]->(d) 
                CREATE (u)-[fav:{relation}]->(d)
                SET d.favoritesCount = d.favoritesCount + 1, fav.date = datetime()
                RETURN d.favoritesCount AS FavoritesCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["FavoritesCount"].As<int>();
                }
            );
        }

        public async Task<int> UnfavoriteDestinationAsync(string destinationId, string userId)
        {

            string relation = GraphRelations.User.FavoritedDestination;

            bool exist = await IsRelationExist(relation, userId, "User", destinationId, "Destination");

            if (!exist)
                throw new Exception("Relation not exist.");

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})-[fav:{relation}]->(d:Destination {{id: $destinationId}}) 
                DELETE fav 
                SET d.favoritesCount = d.favoritesCount - 1 
                RETURN d.favoritesCount AS FavoritesCount",
                new
                {
                    destinationId,
                    userId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["FavoritesCount"].As<int>();
                }
            );
        }

        public async Task UpdateAllDestinationCountersAsync()
        {
            await _neo4jService.ExecuteWriteAsync(
                $@"MATCH (d:Destination)
                OPTIONAL MATCH (User)-[f:{GraphRelations.User.Followed}]->(d)
                OPTIONAL MATCH (User)-[v:{GraphRelations.User.Visited}]->(d)
                OPTIONAL MATCH (User)-[w:{GraphRelations.User.Wished}]->(d)
                OPTIONAL MATCH (User)-[fav:{GraphRelations.User.FavoritedDestination}]->(d)
                OPTIONAL MATCH (User)-[review:{GraphRelations.User.Reviewed}]->(d)
                WITH d, COUNT(f) AS FollowersCount, COUNT(v) AS VisitorsCount,
                        COUNT(w) AS WishesCount, COUNT(fav) AS FavoritesCount,
                        COUNT(review) AS ReviewsCount
                SET d.followersCount = FollowersCount,
                    d.visitorsCount = VisitorsCount,
                    d.wishesCount = WishesCount,
                    d.favoritesCount = FavoritesCount,
                    d.reviewsCount = ReviewsCount",
                new { }
            );
        }
    }
}
