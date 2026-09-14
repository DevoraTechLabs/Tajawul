using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain.SocialMedia;
using Tajawul.Models.DTOs.Comment;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.SocialMedia.Comment;
using Tajawul.Models.ViewModels.user;
using Tajawul.Services;

namespace Tajawul.Repositories.SocialMedia;
public class CommentRepository(Neo4jService neo4jService)
{
    private readonly Neo4jService _neo4JService = neo4jService;

    public async Task<CommentModel?> AddCommentAsync(CreateCommentDto commentDto, string userId)
    {
        var now = DateTime.UtcNow;

        return await _neo4JService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})
                MATCH (p:Post {{id: $postId}})
                CREATE (c:Comment {{
                    id: randomUUID(),
                    content: $content,
                    createdAt: $createdAt,
                    upvoteCount: 0,
                    downvoteCount: 0,
                    repliesCount: 0
                }})
                CREATE (u)-[:{GraphRelations.User.Created}]->(c)
                CREATE (c)-[:{GraphRelations.Comment.CommentedOn}]->(p)
                SET p.commentsCount = p.commentsCount + 1
                RETURN 
                    c.id AS CommentId, 
                    c.content AS Content, 
                    c.upvoteCount AS UpvoteCount, 
                    c.downvoteCount AS DownvoteCount, 
                    c.repliesCount AS RepliesCount, 
                    c.createdAt AS CreatedAt, 
                    c.updatedAt AS UpdatedAt",
            new
            {
                userId = userId,
                postId = commentDto.PostId,
                content = commentDto.Content,
                createdAt = now
            },
            async result =>
            {
                var records = await result.ToListAsync();
                var record = records.SingleOrDefault();
                if (record == null) return null;
                return new CommentModel
                {
                    CommentId = record["CommentId"].As<string>(),
                    Content = record["Content"].As<string>(),
                    UpvoteCount = record["UpvoteCount"].As<int>(),
                    DownvoteCount = record["DownvoteCount"].As<int>(),
                    RepliesCount = record["RepliesCount"].As<int>(),
                    CreatedAt = record["CreatedAt"] is ZonedDateTime zdt ? zdt.UtcDateTime : null,
                    UpdatedAt = record["UpdatedAt"] is ZonedDateTime zdt2 ? zdt2.UtcDateTime : null
                };
            }
        );
    }

    public async Task<CommentModel?> UpdateCommentAsync(UpdateCommentDto commentDto, string userId)
    {
        var now = DateTime.UtcNow;

        return await _neo4JService.ExecuteReadAsync($@"
            MATCH (u:User {{id: $userId}})-
                        [:{GraphRelations.User.Created}]
                            ->(c:Comment {{id: $commentId}})
            SET c.content = $content, 
                c.updatedAt = $updatedAt
            RETURN 
                c.id AS CommentId, 
                c.content AS Content, 
                c.upvoteCount AS UpvoteCount, 
                c.downvoteCount AS DownvoteCount, 
                c.repliesCount AS RepliesCount, 
                c.createdAt AS CreatedAt, 
                c.updatedAt AS UpdatedAt",
        new
        {
            userId = userId,
            commentId = commentDto.CommentId,
            content = commentDto.Content,
            updatedAt = now
        },
        async result =>
        {
            var records = await result.ToListAsync();
            var record = records.SingleOrDefault();

            if (record == null) return null;

            return new CommentModel
            {
                CommentId = record["CommentId"].As<string>(),
                Content = record["Content"].As<string>(),
                UpvoteCount = record["UpvoteCount"].As<int>(),
                DownvoteCount = record["DownvoteCount"].As<int>(),
                RepliesCount = record["RepliesCount"].As<int>(),
                CreatedAt = record["CreatedAt"] is ZonedDateTime zdt ? zdt.UtcDateTime : null,
                UpdatedAt = record["UpdatedAt"] is ZonedDateTime zdt2 ? zdt2.UtcDateTime : null
            };
        }
    );
    }

    public async Task<CommentModel?> ReplyCommentAsync(ReplyCommentDto commentDto, string userId)
    {
        var now = DateTime.UtcNow;

        return await _neo4JService.ExecuteReadAsync($@"
            MATCH (u:User {{id: $userId}})
            MATCH (c:Comment {{id: $commentId}})
            CREATE (r:Comment {{
                id: randomUUID(),
                content: $content,
                createdAt: $createdAt,
                upvoteCount: 0,
                downvoteCount: 0,
                repliesCount: 0
            }})
            CREATE (u)-[:{GraphRelations.User.Created}]->(r)
            CREATE (r)-[:{GraphRelations.Comment.Replied}]->(c)
            SET c.repliesCount = c.repliesCount + 1
            RETURN 
                r.id AS CommentId, 
                r.content AS Content, 
                r.upvoteCount AS UpvoteCount, 
                r.downvoteCount AS DownvoteCount, 
                r.repliesCount AS RepliesCount, 
                r.createdAt AS CreatedAt, 
                r.updatedAt AS UpdatedAt",
                new
                {
                    userId = userId,
                    commentId = commentDto.CommentId,
                    content = commentDto.Content,
                    createdAt = now
                },
                async result =>
                {
                    var records = await result.ToListAsync();
                    var record = records.SingleOrDefault();

                    if (record == null) return null;
                    return new CommentModel
                    {
                        CommentId = record["CommentId"].As<string>(),
                        Content = record["Content"].As<string>(),
                        UpvoteCount = record["UpvoteCount"].As<int>(),
                        DownvoteCount = record["DownvoteCount"].As<int>(),
                        RepliesCount = record["RepliesCount"].As<int>(),
                        CreatedAt = record["CreatedAt"] is ZonedDateTime zdt ? zdt.UtcDateTime : null,
                        UpdatedAt = record["UpdatedAt"] is ZonedDateTime zdt2 ? zdt2.UtcDateTime : null
                    };
                });
    }

    public async Task<CommentModel?> DeleteCommentAsync(string commentId, string userId)
    {
        return await _neo4JService.ExecuteReadAsync(
            $@"
            // 1. Verify Ownership: Match the target comment 'c' AND the user who created it.
            // If this match fails, the query stops, nothing is deleted, and null is returned.
            MATCH (u:User {{id: $userId}})-[:{GraphRelations.User.Created}]->(c:Comment {{id: $commentId}})

            // 2. Find Parent of 'c' (Comment or Post) for counter decrement (Optional Matches)
            OPTIONAL MATCH (c)-[:{GraphRelations.Comment.Replied}]->(parentOfC_Comment:Comment)
            OPTIONAL MATCH (c)-[:{GraphRelations.Comment.CommentedOn}]->(parentOfC_Post:Post) // If comments can be directly on posts

            // 3. Store data of the target comment 'c' *before* any deletions.
            // This WITH clause projects 'c' and its parent info, and captures 'c's data.
            WITH c, u, parentOfC_Comment, parentOfC_Post, {{
                commentId: c.id,
                content: c.content,
                upvoteCount: c.upvoteCount,
                downvoteCount: c.downvoteCount,
                repliesCount: c.repliesCount, // Replies count *of c* before its deletion
                createdAt: c.createdAt,
                updatedAt: c.updatedAt
            }} AS deletedCommentData

            // 4. Find 'c' and ALL its descendant replies to be deleted.
            // (c)<-[:REPLIED*0..]-(node) will match 'c' itself (0 hops) and all its nested replies.
            // It's an OPTIONAL MATCH in case 'c' somehow has no :Comment label or relations after previous steps (shouldn't happen here).
            // More importantly, this ensures the query doesn't fail if there are no replies at all.
            OPTIONAL MATCH (c)<-[:{GraphRelations.Comment.Replied}*0..]-(nodeToDelete:Comment)
            // Collect all distinct nodes that need to be deleted (c and its entire reply tree).
            // Pass previously captured data and parent info through.
            WITH parentOfC_Comment, parentOfC_Post, deletedCommentData, collect(DISTINCT nodeToDelete) AS allNodesInSubgraphToDelete

            // 5. Decrement parent counters for 'c' (if 'c' had a parent)
            FOREACH (_ IN CASE WHEN parentOfC_Comment IS NOT NULL THEN [1] ELSE [] END |
                SET parentOfC_Comment.repliesCount = parentOfC_Comment.repliesCount - 1
            )
            FOREACH (_ IN CASE WHEN parentOfC_Post IS NOT NULL AND parentOfC_Post.commentsCount IS NOT NULL THEN [1] ELSE [] END |
                SET parentOfC_Post.commentsCount = parentOfC_Post.commentsCount - 1
            )

            // 6. Delete all collected nodes (c and its entire reply hierarchy) and their relationships.
            // Filter out potential nulls if 'c' wasn't found in the OPTIONAL MATCH (unlikely given the flow).
            FOREACH (node IN [n IN allNodesInSubgraphToDelete WHERE n IS NOT NULL] |
                DETACH DELETE node
            )

            // 7. Return the captured data of the original (root) deleted comment 'c'
            RETURN deletedCommentData
            ",
            new { commentId, userId },
            async result =>
            {
                var records = await result.ToListAsync();
                var record = records.SingleOrDefault();

                if (record == null) return null;

                var dataMap = record["deletedCommentData"].As<IDictionary<string, object>>();

                return new CommentModel
                {
                    CommentId = dataMap["commentId"].As<string>(),
                    Content = dataMap["content"]?.As<string>() ?? string.Empty,
                    UpvoteCount = dataMap["upvoteCount"].As<int>(),
                    DownvoteCount = dataMap["downvoteCount"].As<int>(),
                    RepliesCount = dataMap["repliesCount"].As<int>(),
                    CreatedAt = dataMap["createdAt"] is ZonedDateTime zdt ? zdt.UtcDateTime : null,
                    UpdatedAt = dataMap["updatedAt"] is ZonedDateTime zdt2 ? zdt2.UtcDateTime : null
                };
            }
        );
    }


    public async Task<CommentWithRepliesDto?> GetCommentWithRepliesAsync(
            string commentId,
            string currentUserId,
            int pageNumber,
            int pageSize)
    {
        int skip = (pageNumber - 1) * pageSize;

        return await _neo4JService.ExecuteReadAsync(
            $@"
            // 1. Match the root comment
            MATCH (rootComment:Comment {{id: $commentId}})
            OPTIONAL MATCH (rootAuthor:User)-[:{GraphRelations.User.Created}]->(rootComment)

            // 2. Current user and their voting status on the root comment
            OPTIONAL MATCH (currentUser:User {{id: $currentUserId}})
            OPTIONAL MATCH (currentUser)-[rUpRoot:{GraphRelations.Comment.Upvoted}]->(rootComment)
            OPTIONAL MATCH (currentUser)-[rDownRoot:{GraphRelations.Comment.Downvoted}]->(rootComment)

            // --- Collect root comment data ---
            WITH rootComment, rootAuthor, currentUser,
                 rUpRoot IS NOT NULL AS rootIsUpvoted,
                 rDownRoot IS NOT NULL AS rootIsDownvoted

            // --- Determine if pagination is needed based on rootComment.repliesCount ---
            // This parameter will be passed to subsequent parts of the query
            WITH rootComment, rootAuthor, currentUser, rootIsUpvoted, rootIsDownvoted,
                 (rootComment.repliesCount > $paginationThreshold) AS applyPagination

            // 3. Get all direct replies to the root comment
            OPTIONAL MATCH (reply:Comment)-[:{GraphRelations.Comment.Replied}]->(rootComment)
            OPTIONAL MATCH (replyAuthor:User)-[:{GraphRelations.User.Created}]->(reply)
            OPTIONAL MATCH (currentUser)-[rUpReply:{GraphRelations.Comment.Upvoted}]->(reply)
            OPTIONAL MATCH (currentUser)-[rDownReply:{GraphRelations.Comment.Downvoted}]->(reply)

            // --- Prepare reply data before collection ---
            WITH rootComment, rootAuthor, currentUser, rootIsUpvoted, rootIsDownvoted, applyPagination,
                 reply, replyAuthor,
                 rUpReply IS NOT NULL AS replyIsUpvoted,
                 rDownReply IS NOT NULL AS replyIsDownvoted
            ORDER BY reply.createdAt ASC // Consistent ordering

            // --- Collect all ordered replies with their details ---
            WITH rootComment, rootAuthor, currentUser, rootIsUpvoted, rootIsDownvoted, applyPagination,
                 COLLECT(CASE WHEN reply IS NOT NULL THEN {{
                     commentNode: reply,
                     authorNode: replyAuthor,
                     isUpvoted: replyIsUpvoted,
                     isDownvoted: replyIsDownvoted
                 }} ELSE null END) AS allReplyDetailsList

            // Filter out potential nulls if no replies
            WITH rootComment, rootAuthor, currentUser, rootIsUpvoted, rootIsDownvoted, applyPagination,
                 [r IN allReplyDetailsList WHERE r IS NOT NULL] AS filteredReplyDetailsList

            // 4. Conditionally apply pagination OR take all replies
            // If applyPagination is true, use SKIP and LIMIT. Otherwise, take the whole list.
            WITH rootComment, rootAuthor, currentUser, rootIsUpvoted, rootIsDownvoted, applyPagination,
                 filteredReplyDetailsList,
                 (CASE
                    WHEN applyPagination THEN filteredReplyDetailsList[$skipReplies .. $skipReplies + $limitReplies]
                    ELSE filteredReplyDetailsList // Take all if not paginating
                 END) AS finalPaginatedOrAllReplyDetailsList

            // 5. Return the structured result
            RETURN
                {{ // Root Comment
                    commentId: rootComment.id,
                    content: rootComment.content,
                    upvoteCount: rootComment.upvoteCount,
                    downvoteCount: rootComment.downvoteCount,
                    repliesCount: rootComment.repliesCount, // Count of replies *to this rootComment*
                    createdAt: rootComment.createdAt,
                    updatedAt: rootComment.updatedAt,
                    author: CASE WHEN rootAuthor IS NOT NULL THEN {{
                        id: rootAuthor.id,
                        firstName: rootAuthor.firstName,
                        lastName: rootAuthor.lastName,
                        username: rootAuthor.username,
                        profileImage: rootAuthor.profileImage // Added profileImage
                        // other rootAuthor properties
                    }} ELSE null END,
                    isUpvotedByCurrentUser: rootIsUpvoted,
                    isDownvotedByCurrentUser: rootIsDownvoted
                }} AS rootCommentData,
                // Paginated or All Replies
                [replyData IN finalPaginatedOrAllReplyDetailsList | {{
                    commentId: replyData.commentNode.id,
                    content: replyData.commentNode.content,
                    upvoteCount: replyData.commentNode.upvoteCount,
                    downvoteCount: replyData.commentNode.downvoteCount,
                    repliesCount: replyData.commentNode.repliesCount, // Count of replies *to this specific reply*
                    createdAt: replyData.commentNode.createdAt,
                    updatedAt: replyData.commentNode.updatedAt,
                    author: CASE WHEN replyData.authorNode IS NOT NULL THEN {{
                        id: replyData.authorNode.id,
                        firstName: replyData.authorNode.firstName,
                        lastName: replyData.authorNode.lastName,
                        username: replyData.authorNode.username,
                        profileImage: replyData.authorNode.profileImage // Added profileImage
                        // other replyAuthor properties
                    }} ELSE null END,
                    isUpvotedByCurrentUser: replyData.isUpvoted,
                    isDownvotedByCurrentUser: replyData.isDownvoted
                }}] AS repliesData,
                SIZE(filteredReplyDetailsList) AS totalRepliesCount, // Total actual replies
                applyPagination // To inform C# side if pagination was applied
            ",
            new
            {
                commentId,
                currentUserId,
                skipReplies = skip, // For pagination
                limitReplies = pageSize, // For pagination
                paginationThreshold = pageSize // Pass the threshold
            },
            async result =>
            {
                var records = await result.ToListAsync();
                var record = records.SingleOrDefault();
                if (record == null) return null;

                var rootCommentData = record["rootCommentData"].As<IDictionary<string, object>>();
                var repliesDataList = record["repliesData"].As<List<IDictionary<string, object>>>();
                var totalReplies = record["totalRepliesCount"].As<int>();
                bool paginationApplied = record["applyPagination"].As<bool>();

                var rootCommentDto = MapToCommentWithAuthorDto(rootCommentData);
                if (rootCommentDto == null) return null;

                var repliesDtoList = repliesDataList
                    .Select(replyMap => MapToCommentWithAuthorDto(replyMap))
                    .Where(dto => dto != null)
                    .Select(dto => dto!) // Non-null assertion after Where
                    .ToList();

                return new CommentWithRepliesDto
                {
                    RootComment = rootCommentDto,
                    Items = repliesDtoList,
                    PageNumber = paginationApplied ? pageNumber : 1,
                    PageSize = paginationApplied ? pageSize : totalReplies,
                    TotalCount = totalReplies
                };
            });
    }

    private CommentWithAuthorDto? MapToCommentWithAuthorDto(IDictionary<string, object>? dataMap)
    {
        if (dataMap == null || !dataMap.ContainsKey("commentId") || dataMap["commentId"] == null)
            return null;

        UserBasicInfoDto? authorDto = null;
        if (dataMap.TryGetValue("author", out var authorObj) && authorObj is IDictionary<string, object> authorMap)
        {
            authorDto = new UserBasicInfoDto
            {
                Id = authorMap["id"].As<string>(),
                FirstName = authorMap["firstName"]?.As<string>()!,
                LastName = authorMap["lastName"]?.As<string>()!,
                Username = authorMap["username"]?.As<string>()!,
                ProfileImage = authorMap["profileImage"]?.As<string>() // Can be null
            };
        }

        return new CommentWithAuthorDto
        {
            CommentId = dataMap["commentId"].As<string>(),
            Content = dataMap["content"]?.As<string>() ?? string.Empty,
            UpvoteCount = dataMap["upvoteCount"].As<int>(),
            DownvoteCount = dataMap["downvoteCount"].As<int>(),
            RepliesCount = dataMap["repliesCount"].As<int>(),
            CreatedAt = dataMap["createdAt"] is ZonedDateTime zdt ? zdt.UtcDateTime : null,
            UpdatedAt = dataMap["updatedAt"] is ZonedDateTime zdt2 ? zdt2.UtcDateTime : null,
            Author = authorDto,
            IsUpvotedByCurrentUser = dataMap.ContainsKey("isUpvotedByCurrentUser") && dataMap["isUpvotedByCurrentUser"].As<bool>(),
            IsDownvotedByCurrentUser = dataMap.ContainsKey("isDownvotedByCurrentUser") && dataMap["isDownvotedByCurrentUser"].As<bool>()
        };
    }

    public async Task<PaginatedResultDto<CommentWithAuthorDto>> GetCommentsForPostAsync(
        string postId,
        string currentUserId,
        int pageNumber,
        int pageSize)
    {

        int skip = (pageNumber - 1) * pageSize;

        // The Cypher query is modified to use postNode.commentsCount
        return await _neo4JService.ExecuteReadAsync(
            $@"
        // 1. Attempt to match the target post.
        // If postNode is not found, the query will return no rows.
        MATCH (postNode:Post {{id: $postId}})

        // 2. Get total comments count directly from the post node's attribute.
        // Use COALESCE to default to 0 if commentsCount is null or missing.
        // This totalCommentsCount will be returned with each comment row, or as a standalone
        // value if no comments are found for the current page.
        WITH postNode, COALESCE(postNode.commentsCount, 0) AS totalCommentsCount

        // 3. Get the paginated slice of direct comments for the post.
        // This OPTIONAL MATCH ensures that if there are no comments for the post,
        // or if the current page is out of bounds, the query still proceeds.
        // commentNode will be null in such cases.
        OPTIONAL MATCH (postNode)<-[:{GraphRelations.Comment.CommentedOn}]-(commentNode:Comment)
        WITH postNode, commentNode, totalCommentsCount // Pass total count along
        ORDER BY commentNode.createdAt ASC 
        SKIP $skipParam LIMIT $limitParam

        // 4. For each comment in the paginated set, get its author.
        // These are OPTIONAL so they don't filter out comments if author/user is missing.
        OPTIONAL MATCH (authorNode:User)-[:{GraphRelations.User.Created}]->(commentNode)

        // 5. For each comment, check if the current user has upvoted or downvoted it.
        // OPTIONAL MATCH for currentUser to handle cases where currentUserId might not match any user,
        // or if the user hasn't voted.
        OPTIONAL MATCH (currentUser:User {{id: $currentUserIdParam}})
        OPTIONAL MATCH (currentUser)-[rUp:{GraphRelations.Comment.Upvoted}]->(commentNode)
        OPTIONAL MATCH (currentUser)-[rDown:{GraphRelations.Comment.Downvoted}]->(commentNode)

        // 6. Return the total count and the details for each comment in the paginated set.
        // If commentNode is null (no comments on this page or no comments at all for the post
        // after the initial OPTIONAL MATCH), commentDetails will be null.
        // If postNode was not found initially, this RETURN statement is never reached for that post.
        RETURN
            totalCommentsCount,
            CASE WHEN commentNode IS NOT NULL THEN {{
                commentId: commentNode.id,
                content: commentNode.content,
                upvoteCount: commentNode.upvoteCount,
                downvoteCount: commentNode.downvoteCount,
                repliesCount: commentNode.repliesCount,
                createdAt: commentNode.createdAt,
                updatedAt: commentNode.updatedAt,
                author: CASE WHEN authorNode IS NOT NULL THEN {{
                    id: authorNode.id,
                    firstName: authorNode.firstName,
                    lastName: authorNode.lastName,
                    username: authorNode.username,
                    profileImage: authorNode.profileImage
                }} ELSE null END,
                isUpvotedByCurrentUser: rUp IS NOT NULL,
                isDownvotedByCurrentUser: rDown IS NOT NULL
            }} ELSE null END AS commentDetails
        ",
            new
            {
                postId = postId,
                currentUserIdParam = currentUserId,
                skipParam = skip,
                limitParam = pageSize
            },
            async resultCursor =>
            {
                var records = await resultCursor.ToListAsync();
                var commentItems = new List<CommentWithAuthorDto>();
                int totalCount = 0;

                if (records.Count != 0)
                {
                    totalCount = records.First()["totalCommentsCount"].As<int>();

                    foreach (var record in records)
                    {
                        if (record["commentDetails"] != null && record["commentDetails"] is IDictionary<string, object> commentMap)
                        {
                            var commentDto = MapToCommentWithAuthorDto(commentMap); // Your existing helper
                            if (commentDto != null)
                            {
                                commentItems.Add(commentDto);
                            }
                        }
                    }
                }
                return new PaginatedResultDto<CommentWithAuthorDto>
                {
                    Items = commentItems,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }
        ) ?? new PaginatedResultDto<CommentWithAuthorDto>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}







