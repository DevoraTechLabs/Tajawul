
using Microsoft.AspNetCore.Mvc;
using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Helpers.Filters.SocialMedia;
using Tajawul.Models.Domain;
using Tajawul.Models.Domain.SocialMedia.Posts;
using Tajawul.Models.DTOs.SocialMedia.Posts;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Services;

namespace Tajawul.Repositories.SocialMedia.Posts
{
    public class PostRepository
    {
        private readonly Neo4jService _neo4jService;

        public PostRepository(Neo4jService neo4jService)
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

        public async Task<bool> IsNodeExist(string nodeId, string nodeLabel)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (n:{nodeLabel} {{id: $nodeId}})
                RETURN COUNT(n) > 0 AS NodeExists",
                new
                {
                    nodeId
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["NodeExists"].As<bool>();
                }
            );
        }

        public async Task<Post> CreatePostAsync(CreatePostDto postDto, string userId)
        {
            var post = await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                    CREATE (p:Post{{
                    id: randomUUID(),
                    content: $content,
                    images: $images,
                    upVotesCount: $upVotesCount,
                    downVotesCount: $downVotesCount,
                    sharesCount: $sharesCount,
                    commentsCount: $commentsCount,
                    creationDate: datetime(),
                    lastEditDate: datetime()}})

                WITH u, p

                MERGE (u)-[:{GraphRelations.User.Created}]->(p)

                RETURN p.id AS PostId, p.content AS Content, p.images AS Images,
                p.upVotesCount AS UpVotesCount, p.downVotesCount AS DownVotesCount,
                p.sharesCount AS SharesCount, p.commentsCount AS CommentsCount,
                [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                p.creationDate AS CreationDate, p.lastEditDate AS LastEditDate",
                new
                {
                    userId = userId,
                    content = postDto.Content,
                    images = new List<string>(),
                    upVotesCount = 0,
                    downVotesCount = 0,
                    sharesCount = 0,
                    commentsCount = 0,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Post
                    {
                        PostId = record["PostId"].As<string>(),
                        Content = record["Content"].As<string>(),
                        Images = record["Images"].As<List<string>>(),
                        UpVotesCount = record["UpVotesCount"].As<int>(),
                        DownVotesCount = record["DownVotesCount"].As<int>(),
                        SharesCount = record["SharesCount"].As<int>(),
                        CommentsCount = record["CommentsCount"].As<int>(),
                        Creator = record["Creator"].As<List<string>>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                        ? creationDate
                        : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                        ? updateDate
                        : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid.")
                    };
                });

            return post;
        }

        public async Task<bool> UpdatePostImagesAsync(List<string> imageUrls, string postId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (p:Post {{id: $postId}})
                SET p.images = $images, p.lastEditDate = datetime()
                ",
                new
                {
                    postId,
                    images = imageUrls
                }
            );

            return summary.Counters.PropertiesSet > 0;
        }

        public async Task<List<DestinationUserDto>> CreatePostEmbeddingsAsync(string postId, List<string> entitiesIds, string entityName)
        {
            string attribute = entityName == "Trip" ? "title" : "name";

            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (p:Post {{id: $postId}})
                UNWIND $entitiesIds AS nodeId
                MATCH (n:{entityName} {{id: nodeId}})
                MERGE (p)-[:{GraphRelations.Post.Contained}]->(n)
                RETURN n.id AS Id, n.{attribute} AS Name, n.coverImage AS Image
                ",
                new
                {
                    postId,
                    entitiesIds = entitiesIds
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new DestinationUserDto
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Image = record["Image"].As<string>(),

                    }).ToList();
                }
            );
        }
        
        public async Task<List<Post>?> GetUserPostsAsync(string userId)
        {

            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            return await _neo4jService.ExecuteReadAsync(

                $@"
                    MATCH (u:User {{id: $userId}})-[:{GraphRelations.User.Created}]->(p:Post)
                    MATCH (p)-[:{GraphRelations.Post.HasVisibility}]->(v:Visibility)
                    Optional MATCH (p)-[:{GraphRelations.Post.HadTag}]->(tag:Tag)

                    Optional MATCH (p)-[:{GraphRelations.Post.Contained}]->(d:Destination)
                    Optional MATCH (p)-[:{GraphRelations.Post.Contained}]->(t:Trip)
                    Optional MATCH (p)-[:{GraphRelations.Post.Contained}]->(e:Event)


                    RETURN p.id AS PostId, p.content AS Content, p.images AS Images,
                    p.upVotesCount AS UpVotesCount, p.downVotesCount AS DownVotesCount,
                    p.sharesCount AS SharesCount, p.commentsCount AS CommentsCount,
                    [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                    v.name AS Visibility, collect(DISTINCT tag.name) AS Tags,
                    collect(DISTINCT CASE WHEN d IS NOT NULL THEN {{id: d.id, name: d.name, image: d.coverImage }} END) AS Destinations,
                    collect(DISTINCT CASE WHEN t IS NOT NULL THEN {{id: t.id, title: t.title, image: t.coverImage }} END) AS Trips,
                    collect(DISTINCT CASE WHEN e IS NOT NULL THEN {{id: e.id, name: e.name, image: e.coverImage }} END) AS Events,
                    p.creationDate AS CreationDate, p.lastEditDate AS LastEditDate
                ",
                new { userId },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Post
                    {
                        PostId = record["PostId"].As<string>(),
                        Content = record["Content"].As<string>(),
                        Visibility = record["Visibility"].As<string>(),
                        Images = record["Images"].As<List<string>>(),
                        Destinations = record["Destinations"]
                            .As<List<IDictionary<string, object>>>()
                            .Select(d => new DestinationUserDto
                            {
                                Id = d["id"].As<string>(),
                                Name = d["name"].As<string>(),
                                Image = d["image"].As<string>()
                            }).ToList(),
                        Trips = record["Trips"]
                            .As<List<IDictionary<string, object>>>()
                            .Select(d => new DestinationUserDto
                            {
                                Id = d["id"].As<string>(),
                                Name = d["title"].As<string>(),
                                Image = d["image"].As<string>()
                            }).ToList(),
                        Events = record["Events"]
                            .As<List<IDictionary<string, object>>>()
                            .Select(d => new DestinationUserDto
                            {
                                Id = d["id"].As<string>(),
                                Name = d["name"].As<string>(),
                                Image = d["image"].As<string>()
                            }).ToList(),
                        Tags = record["Tags"].As<List<string>>(),
                        UpVotesCount = record["UpVotesCount"].As<int>(),
                        DownVotesCount = record["DownVotesCount"].As<int>(),
                        SharesCount = record["SharesCount"].As<int>(),
                        CommentsCount = record["CommentsCount"].As<int>(),
                        Creator = record["Creator"].As<List<string>>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                        ? creationDate
                        : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                        ? updateDate
                        : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }

        public async Task<List<Post>?> GetUserFeedAsync(PostsFilter postsFilter)
        {

            return await _neo4jService.ExecuteReadAsync(

                $@"
                    MATCH (p:Post)-[:{GraphRelations.Post.HasVisibility}]->(v:Visibility)
                    WHERE v.name = 'Public'

                    MATCH (u:User)-[:{GraphRelations.User.Created}]->(p)

                    Optional MATCH (p)-[:{GraphRelations.Post.HadTag}]->(tag:Tag)

                    Optional MATCH (p)-[:{GraphRelations.Post.Contained}]->(d:Destination)
                    Optional MATCH (p)-[:{GraphRelations.Post.Contained}]->(t:Trip)
                    Optional MATCH (p)-[:{GraphRelations.Post.Contained}]->(e:Event)


                    RETURN p.id AS PostId, p.content AS Content, p.images AS Images,
                    p.upVotesCount AS UpVotesCount, p.downVotesCount AS DownVotesCount,
                    p.sharesCount AS SharesCount, p.commentsCount AS CommentsCount,
                    [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                    v.name AS Visibility, collect(DISTINCT tag.name) AS Tags,
                    collect(DISTINCT CASE WHEN d IS NOT NULL THEN {{id: d.id, name: d.name, image: d.coverImage }} END) AS Destinations,
                    collect(DISTINCT CASE WHEN t IS NOT NULL THEN {{id: t.id, title: t.title, image: t.coverImage }} END) AS Trips,
                    collect(DISTINCT CASE WHEN e IS NOT NULL THEN {{id: e.id, name: e.name, image: e.coverImage }} END) AS Events,
                    p.creationDate AS CreationDate, p.lastEditDate AS LastEditDate
                    SKIP $skip LIMIT $limit
                ",
                new {
                    skip = (postsFilter.PageNumber - 1) * postsFilter.PageSize,
                    limit = postsFilter.PageSize
                },
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Post
                    {
                        PostId = record["PostId"].As<string>(),
                        Content = record["Content"].As<string>(),
                        Visibility = record["Visibility"].As<string>(),
                        Images = record["Images"].As<List<string>>(),
                        Destinations = record["Destinations"]
                            .As<List<IDictionary<string, object>>>()
                            .Select(d => new DestinationUserDto
                            {
                                Id = d["id"].As<string>(),
                                Name = d["name"].As<string>(),
                                Image = d["image"].As<string>()
                            }).ToList(),
                        Trips = record["Trips"]
                            .As<List<IDictionary<string, object>>>()
                            .Select(d => new DestinationUserDto
                            {
                                Id = d["id"].As<string>(),
                                Name = d["title"].As<string>(),
                                Image = d["image"].As<string>()
                            }).ToList(),
                        Events = record["Events"]
                            .As<List<IDictionary<string, object>>>()
                            .Select(d => new DestinationUserDto
                            {
                                Id = d["id"].As<string>(),
                                Name = d["name"].As<string>(),
                                Image = d["image"].As<string>()
                            }).ToList(),
                        Tags = record["Tags"].As<List<string>>(),
                        UpVotesCount = record["UpVotesCount"].As<int>(),
                        DownVotesCount = record["DownVotesCount"].As<int>(),
                        SharesCount = record["SharesCount"].As<int>(),
                        CommentsCount = record["CommentsCount"].As<int>(),
                        Creator = record["Creator"].As<List<string>>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                        ? creationDate
                        : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                        ? updateDate
                        : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid.")
                    }).ToList();
                }
            );
        }
    
        public async Task<bool> DeletePostAsync(string postId, string userId)
        {

            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            var postExist = await IsNodeExist(postId, "Post");
            if (!postExist)
                throw new Exception("Post not found");

            var owned = await IsRelationExist(GraphRelations.User.Created, userId, "User", postId, "Post");

            if (!owned)
                throw new Exception("User not authorized to delete this post");

            var summary = await _neo4jService.ExecuteWriteAsync(
                    @$"MATCH (p:Post {{id: $postId}})
                       DETACH DELETE p",
                    new
                    {
                        postId
                    }
                );

            int nodessDeleted = summary.Counters.NodesDeleted;
            return nodessDeleted > 0;
        }
    }
}
