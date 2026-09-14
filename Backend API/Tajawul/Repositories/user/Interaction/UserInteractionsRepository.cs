using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.user;
using Tajawul.Models.ViewModels.user.UserInteractions;
using Tajawul.Services;

namespace Tajawul.Repositories.User.Interaction
{
    public class UserInteractionsRepository(Neo4jService neo4jService)
    {
        private readonly Neo4jService _neo4JService = neo4jService;


        public async Task<FollowToggleResult?> ToggleFollowUserAsync(string followedId, string userId)
        {
            return await _neo4JService.ExecuteReadAsync(
                $@"
                // 1. Find both users. If either is not found, the query stops and returns nothing.
                MATCH (follower:User {{id: $userId}})      // The user performing the action
                MATCH (followed:User {{id: $followedId}}) // The user being followed/unfollowed

                // 2. Check if the FOLLOWS relationship already exists
                OPTIONAL MATCH (follower)-[r:{GraphRelations.User.FollowedUser}]->(followed)

                // 3. Determine if we need to create (r is null) or delete (r is not null)
                WITH follower, followed, r, (r IS NULL) AS shouldCreate

                // 4. Use apoc.do.when for conditional execution
                CALL apoc.do.when(
                    shouldCreate,
                    // Query to execute if shouldCreate is true (CREATE relationship - follow)
                    'CREATE (follower)-[:{GraphRelations.User.FollowedUser} {{createdAt: datetime()}}]->(followed)
                     // Increment counts, ensuring they exist first (coalesce)
                     SET follower.followingsCount = coalesce(follower.followingsCount, 0) + 1
                     SET followed.followersCount = coalesce(followed.followersCount, 0) + 1
                     RETURN true AS isFollowingNow', // Indicate the new state is 'following'

                    // Query to execute if shouldCreate is false (DELETE relationship - unfollow)
                    'DELETE r
                     // Decrement counts, ensuring they dont go below 0
                     SET follower.followingsCount = CASE WHEN coalesce(follower.followingsCount, 0) > 0 THEN follower.followingsCount - 1 ELSE 0 END
                     SET followed.followersCount = CASE WHEN coalesce(followed.followersCount, 0) > 0 THEN followed.followersCount - 1 ELSE 0 END
                     RETURN false AS isFollowingNow', // Indicate the new state is 'not following'
                    {{follower: follower, followed: followed, r: r}} // Params for inner queries
                ) YIELD value 

                // 5. Return the final state
                // The follower/followed nodes have been updated by the conditional query
                RETURN
                    followed.followersCount AS finalFollowersCount, // Get updated count from the followed user
                    value.isFollowingNow AS isFollowing             // Get the boolean result from the APOC call
                ",
                new
                {
                    userId,     // Parameter for follower user ID
                    followedId  // Parameter for followed user ID
                },
                async resultCursor =>
                {
                    // Use SingleOrDefault because if users are found, one result row is expected.
                    // If either user is not found, the initial MATCH fails, and no rows are returned.
                    var records = await resultCursor.ToListAsync();
                    var record = records.SingleOrDefault();
                    if (record == null)
                    {
                        return null; // Indicate that one or both users were not found
                    }

                    return new FollowToggleResult
                    {
                        // Use coalesce on the C# side as well for safety, though the query ensures counts exist
                        FollowersCount = record["finalFollowersCount"]?.As<int?>() ?? 0,
                        IsFollowing = record["isFollowing"].As<bool>()
                    };
                }
            );
        }


        public async Task<PaginatedResultDto<UserBasicInfoDto>?> GetFollowersAsync(
            string userId,
            int pageNumber,
            int pageSize)
        {

            int skip = (pageNumber - 1) * pageSize;

            // Use ExecuteReadAsync as we are only reading data
            return await _neo4JService.ExecuteReadAsync(
                $@"
                // 1. Match the target user whose followers we want.
                MATCH (targetUser:User {{id: $userId}})

                // 2. Get the total count of followers directly from the target user's property.
                // Use COALESCE for safety if the property might be missing/null.
                WITH targetUser, COALESCE(targetUser.followersCount, 0) AS totalCount

                // 3. Find the followers pointing to the target user (paginated).
                // OPTIONAL MATCH ensures we still get the totalCount even if this page is empty.
                OPTIONAL MATCH (followerUser:User)-[:{GraphRelations.User.FollowedUser}]->(targetUser)
                WITH targetUser, totalCount, followerUser
                ORDER BY followerUser.username // Or followerUser.createdAt, etc. Define an order.
                SKIP $skipParam LIMIT $limitParam

                // 4. Collect and return the data.
                RETURN
                    totalCount,
                    // Only build the map if a followerUser was found on this page
                    CASE WHEN followerUser IS NOT NULL THEN {{
                        userProfile: {{
                            userId: followerUser.id,
                            firstName: followerUser.firstName,
                            lastName: followerUser.lastName,
                            username: followerUser.username,
                            profileImage: followerUser.profileImage
                        }}
                    }} ELSE null END AS followerData
                ",
                new
                {
                    userId,
                    skipParam = skip,
                    limitParam = pageSize
                },
                async result =>
                {
                    var records = await result.ToListAsync();
                    var items = new List<UserBasicInfoDto>();
                    int totalCount = 0;

                    if (records.Count == 0) return null;

                    // totalCount is the same for all records from this query structure
                    totalCount = records.First()["totalCount"].As<int>();

                    foreach (var record in records)
                    {
                        // 'record["followerData"]' gives an object, which should be a map or null
                        var followerDataMap = record["followerData"].As<IDictionary<string, object>>();

                        if (followerDataMap != null)
                        {
                            // The 'followerDataMap' IS the structure: { userProfile: { ... } }
                            var userProfileMap = followerDataMap["userProfile"].As<IDictionary<string, object>>();

                            if (userProfileMap != null)
                            {
                                items.Add(new UserBasicInfoDto
                                {
                                    Id = userProfileMap["userId"].As<string>()!,
                                    FirstName = userProfileMap["firstName"]?.As<string>()!,
                                    LastName = userProfileMap["lastName"]?.As<string>()!,
                                    Username = userProfileMap["username"]?.As<string>()!,
                                    ProfileImage = userProfileMap["profileImage"]?.As<string>()
                                });
                            }
                        }
                    }

                    return new PaginatedResultDto<UserBasicInfoDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            );
        }

        public async Task<PaginatedResultDto<UserBasicInfoDto>?> GetFollowingsAsync(
         string userId,
         int pageNumber,
         int pageSize)
        {

            int skip = (pageNumber - 1) * pageSize;

            // Use ExecuteReadAsync as we are only reading data
            return await _neo4JService.ExecuteReadAsync(
                $@"
                // 1. Match the target user whose followers we want.
                MATCH (targetUser:User {{id: $userId}})

                // 2. Get the total count of followers directly from the target user's property.
                // Use COALESCE for safety if the property might be missing/null.
                WITH targetUser, COALESCE(targetUser.followingsCount, 0) AS totalCount

                // 3. Find the followers pointing to the target user (paginated).
                // OPTIONAL MATCH ensures we still get the totalCount even if this page is empty.
                OPTIONAL MATCH (followerUser:User)<-[:{GraphRelations.User.FollowedUser}]-(targetUser)
                WITH targetUser, totalCount, followerUser
                ORDER BY followerUser.username // Or followerUser.createdAt, etc. Define an order.
                SKIP $skipParam LIMIT $limitParam

                // 4. Collect and return the data.
                RETURN
                    totalCount,
                    // Only build the map if a followerUser was found on this page
                    CASE WHEN followerUser IS NOT NULL THEN {{
                        userProfile: {{
                            userId: followerUser.id,
                            firstName: followerUser.firstName,
                            lastName: followerUser.lastName,
                            username: followerUser.username,
                            profileImage: followerUser.profileImage
                        }}
                    }} ELSE null END AS followerData
                ",
                new
                {
                    userId,
                    skipParam = skip,
                    limitParam = pageSize
                },
                async result =>
                {
                    var records = await result.ToListAsync();
                    var items = new List<UserBasicInfoDto>();
                    int totalCount = 0;

                    if (records.Count == 0) return null;

                    // totalCount is the same for all records from this query structure
                    totalCount = records.First()["totalCount"].As<int>();

                    foreach (var record in records)
                    {
                        // 'record["followerData"]' gives an object, which should be a map or null
                        var followerDataMap = record["followerData"].As<IDictionary<string, object>>();

                        if (followerDataMap != null)
                        {
                            // The 'followerDataMap' IS the structure: { userProfile: { ... } }
                            var userProfileMap = followerDataMap["userProfile"].As<IDictionary<string, object>>();

                            if (userProfileMap != null)
                            {
                                items.Add(new UserBasicInfoDto
                                {
                                    Id = userProfileMap["userId"].As<string>()!,
                                    FirstName = userProfileMap["firstName"]?.As<string>()!,
                                    LastName = userProfileMap["lastName"]?.As<string>()!,
                                    Username = userProfileMap["username"]?.As<string>()!,
                                    ProfileImage = userProfileMap["profileImage"]?.As<string>()
                                });
                            }
                        }
                    }

                    return new PaginatedResultDto<UserBasicInfoDto>
                    {
                        Items = items,
                        TotalCount = totalCount,
                        PageNumber = pageNumber,
                        PageSize = pageSize
                    };
                }
            );
        }


    }
}
