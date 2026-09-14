using Microsoft.EntityFrameworkCore;
using Neo4j.Driver;
using Newtonsoft.Json;
using Tajawul.Helpers;
using Tajawul.Models.Domain.General;
using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs;
using Tajawul.Models.ViewModels.user;
using Tajawul.Services;

namespace Tajawul.Repositories.user.Profile
{
    public class UserProfileRepository
    {
        private readonly Neo4jService _neo4jService;

        public UserProfileRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }

        private async Task<bool> IsNodeExistAsync(string nodeLabel, string attribute, string attributeValue)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (n:{nodeLabel})
                WHERE n.{attribute} = $attributeValue
                RETURN COUNT(n) > 0 AS NodeExists
                ",
                new
                {
                    attributeValue
                },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["NodeExists"].As<bool>();
                }
            );
        }

        public async Task<bool> CreateUserNodeIfNotExistAsync(string userId)
        {
            bool exists = await IsNodeExistAsync("User", "id", userId);
            if (exists)
            {
                return true;
            }

            var summary = await _neo4jService.ExecuteWriteAsync(
                @"
                CREATE (u:User { id: $userId })
                SET u.username = $username,
                u.firstName = $firstName,
                u.lastName = $lastName,
                u.bio = $bio,
                u.nationality = $nationality,
                u.phoneNumber = $phoneNumber,
                u.birthDate = $birthDate,
                u.isTopTraveler = $isTopTraveler,
                u.profileImage = $profileImage,
                u.socialMediaLinks = $socialMediaLinks,
                u.createdDestinationCount = $createdDestinationCount,
                u.editedDestinationCount = $editedDestinationCount,
                u.wishedDestinationCount = $wishedDestinationCount,
                u.favoriteDestinationCount = $favoriteDestinationCount,
                u.visitedDestinationCount = $visitedDestinationCount,
                u.followedDestinationCount = $followedDestinationCount,
                u.createdTripCount = $createdTripCount,
                u.wishedTripCount = $wishedTripCount,
                u.favoriteTripCount = $favoriteTripCount,
                u.clonedTripCount = $clonedTripCount,
                u.followersCount = $followersCount,
                u.followingsCount = $followingsCount,
                u.postsCount = $postsCount,
                u.creationDate = datetime(),
                u.lastEditDate = datetime()
                ",
                new
                {
                    userId,
                    username = string.Empty,
                    firstName = string.Empty,
                    lastName = string.Empty,
                    bio = string.Empty,
                    nationality = string.Empty,
                    phoneNumber = string.Empty,
                    birthDate = string.Empty,
                    isTopTraveler = false,
                    profileImage = string.Empty,
                    socialMediaLinks = string.Empty,
                    createdDestinationCount = 0,
                    editedDestinationCount = 0,
                    wishedDestinationCount = 0,
                    favoriteDestinationCount = 0,
                    visitedDestinationCount = 0,
                    followedDestinationCount = 0,
                    createdTripCount = 0,
                    wishedTripCount = 0,
                    favoriteTripCount = 0,
                    clonedTripCount = 0,
                    postsCount = 0,
                    followersCount = 0,
                    followingsCount = 0,
                }
            );

            return summary.Counters.NodesCreated > 0;
        }

        public async Task<UserModel?> GetUserProfileAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HadGender}]->(gender:Gender)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HasMaritalStatus}]->(status:MaritalStatus)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.LocatedIn}]->(city:City)
                OPTIONAL MATCH (city)-[:{GraphRelations.User.LocatedIn}]->(country:Country)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.Speak}]->(language:SpokenLanguage)
                RETURN u.id AS UserId, u.username AS Username, u.firstName AS FirstName, u.lastName AS LastName,
                       u.phoneNumber AS PhoneNumber, u.isTopTraveler AS IsTopTraveler,
                       u.birthDate AS BirthDate, u.bio AS Bio, u.profileImage AS ProfileImage,
                       u.socialMediaLinks AS SocialMediaLinks,
                       u.nationality AS Nationality,
                       city.name AS City, country.name AS Country,
                       gender.name AS Gender, status.name AS MaritalStatus,
                       COLLECT(language.name) AS SpokenLanguages,
                       u.createdDestinationCount AS CreatedDestinationCount,
                       u.editedDestinationCount AS EditedDestinationCount,
                       u.wishedDestinationCount AS WishedDestinationCount,
                       u.favoriteDestinationCount AS FavoriteDestinationCount,
                       u.visitedDestinationCount AS VisitedDestinationCount,
                       u.followedDestinationCount AS FollowedDestinationCount,
                       u.followersCount AS FollowersCount,
                       u.followingsCount AS FollowingsCount,
                       u.createdTripCount AS CreatedTripCount,
                       u.wishedTripCount AS WishedTripCount,
                       u.favoriteTripCount AS FavoriteTripCount,
                       u.clonedTripCount AS ClonedTripCount,
                       u.postsCount AS PostsCount,
                       u.lastEditDate AS LastEditDate,
                       u.creationDate AS CreationDate",
            new { userId },
            async result =>
            {
                var record = await result.SingleAsync();

                if (record == null) return null;

                return new UserModel
                {
                    UserId = record["UserId"].As<string>(),
                    Username = record["Username"].As<string>(),
                    FirstName = record["FirstName"].As<string?>(),
                    LastName = record["LastName"].As<string?>(),
                    Bio = record["Bio"].As<string?>(),
                    Nationality = record["Nationality"].As<string?>(),
                    PhoneNumber = record["PhoneNumber"].As<string?>(),
                    Gender = record["Gender"].As<string?>(),
                    MaritalStatus = record["MaritalStatus"].As<string?>(),
                    BirthDate = DateOnly.TryParse(record["BirthDate"].As<string>(), out var birthDate)
                    ? birthDate
                    : null,
                    City = record["City"].As<string?>(),
                    Country = record["Country"].As<string?>(),
                    IsTopTraveler = record["IsTopTraveler"].As<bool>(),
                    ProfileImage = record["ProfileImage"].As<string?>(),
                    SpokenLanguages = record["SpokenLanguages"].As<List<string>>(),
                    SocialMediaLinks = JsonConvert.DeserializeObject<List<SocialMediaLink>>(record["SocialMediaLinks"].As<string>()),
                    CreatedDestinationCount = record["CreatedDestinationCount"].As<int>(),
                    EditedDestinationCount = record["EditedDestinationCount"].As<int>(),
                    WishedDestinationCount = record["WishedDestinationCount"].As<int>(),
                    FavoriteDestinationCount = record["FavoriteDestinationCount"].As<int>(),
                    VisitedDestinationCount = record["VisitedDestinationCount"].As<int>(),
                    FollowedDestinationCount = record["FollowedDestinationCount"].As<int>(),
                    FollowersCount = record["FollowersCount"].As<int>(),
                    FollowingsCount = record["FollowingsCount"].As<int>(),
                    CreatedTripCount = record["CreatedTripCount"].As<int>(),
                    WishedTripCount = record["WishedTripCount"].As<int>(),
                    FavoriteTripCount = record["FavoriteTripCount"].As<int>(),
                    ClonedTripCount = record["ClonedTripCount"].As<int>(),
                    PostsCount = record["PostsCount"].As<int>(),
                    CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                    ? creationDate
                    : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                    LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                    ? updateDate
                    : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid.")
                };
            });
        }

        public async Task<UserInterests> GetUserInterestsAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HadDestinationType}]->(type:Type)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HadGroupSize}]->(groupSize:GroupSize)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.PreferedActivity}]->(activity:Activity)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HadTag}]->(tag:Tag)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HadPriceRange}]->(priceRange:PriceRange)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.PreferedDuration}]->(duration:TripDuration)
                RETURN COLLECT(type.name) AS Types,
                       COLLECT(groupSize.name) AS GroupSizes,
                       COLLECT(activity.name) AS Activities,
                       COLLECT(tag.name) AS Tags,
                       COLLECT(priceRange.name) AS PriceRanges,
                       COLLECT(duration.name) AS Durations",
            new { userId },
            async result =>
            {
                var record = await result.SingleAsync();

                if (record == null) return null;

                return new UserInterests
                {
                    DestinationTypes = record["Types"].As<List<string>>(),
                    GroupSizes = record["GroupSizes"].As<List<string>>(),
                    Activities = record["Activities"].As<List<string>>(),
                    Tags = record["Tags"].As<List<string>>(),
                    PriceRanges = record["PriceRanges"].As<List<string>>(),
                    TripDurations = record["Durations"].As<List<string>>(),
                };
            });
        }

        public async Task<UserModel> UpdateUserProfileAsync(UpdateUserProfileDto userDto, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                SET u.username = $username,
                u.firstName = $firstName,
                u.lastName = $lastName,
                u.bio = $bio,
                u.nationality = $nationality,
                u.phoneNumber = $phoneNumber,
                u.birthDate = $birthDate,
                u.socialMediaLinks = $socialMediaLinks,
                u.lastEditDate = datetime()
                RETURN u.id AS UserId, u.username AS Username, u.firstName AS FirstName, u.lastName AS LastName,
                       u.phoneNumber AS PhoneNumber, u.isTopTraveler AS IsTopTraveler,
                       u.birthDate AS BirthDate, u.bio AS Bio, u.profileImage AS ProfileImage,
                       u.nationality AS Nationality,
                       u.socialMediaLinks AS SocialMediaLinks,
                       u.createdDestinationCount AS CreatedDestinationCount,
                       u.editedDestinationCount AS EditedDestinationCount,
                       u.wishedDestinationCount AS WishedDestinationCount,
                       u.favoriteDestinationCount AS FavoriteDestinationCount,
                       u.visitedDestinationCount AS VisitedDestinationCount,
                       u.followedDestinationCount AS FollowedDestinationCount,
                       u.createdTripCount AS CreatedTripCount,
                       u.wishedTripCount AS WishedTripCount,
                       u.favoriteTripCount AS FavoriteTripCount,
                       u.clonedTripCount AS ClonedTripCount,
                       u.postsCount AS PostsCount,
                       u.lastEditDate AS LastEditDate,
                       u.creationDate AS CreationDate",
                new
                {
                    userId,
                    username = userDto.Username,
                    firstName = userDto.FirstName,
                    lastName = userDto.LastName,
                    bio = userDto.Bio,
                    nationality = userDto.Nationality,
                    phoneNumber = userDto.PhoneNumber,
                    birthDate = userDto.BirthDate,
                    socialMediaLinks = JsonConvert.SerializeObject(userDto.SocialMediaLinks)
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    if (record == null) return null;

                    return new UserModel
                    {
                        UserId = record["UserId"].As<string>(),
                        Username = record["Username"].As<string>(),
                        FirstName = record["FirstName"].As<string?>(),
                        LastName = record["LastName"].As<string?>(),
                        Bio = record["Bio"].As<string?>(),
                        Nationality = record["Nationality"].As<string>(),
                        PhoneNumber = record["PhoneNumber"].As<string?>(),
                        BirthDate = DateOnly.TryParse(record["BirthDate"].As<string>(), out var birthDate)
                        ? birthDate
                        : null,
                        IsTopTraveler = record["IsTopTraveler"].As<bool>(),
                        ProfileImage = record["ProfileImage"].As<string?>(),
                        SocialMediaLinks = JsonConvert.DeserializeObject<List<SocialMediaLink>>(record["SocialMediaLinks"].As<string>()),
                        CreatedDestinationCount = record["CreatedDestinationCount"].As<int>(),
                        EditedDestinationCount = record["EditedDestinationCount"].As<int>(),
                        WishedDestinationCount = record["WishedDestinationCount"].As<int>(),
                        FavoriteDestinationCount = record["FavoriteDestinationCount"].As<int>(),
                        VisitedDestinationCount = record["VisitedDestinationCount"].As<int>(),
                        FollowedDestinationCount = record["FollowedDestinationCount"].As<int>(),
                        CreatedTripCount = record["CreatedTripCount"].As<int>(),
                        WishedTripCount = record["WishedTripCount"].As<int>(),
                        FavoriteTripCount = record["FavoriteTripCount"].As<int>(),
                        ClonedTripCount = record["ClonedTripCount"].As<int>(),
                        PostsCount = record["PostsCount"].As<int>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                        ? creationDate
                        : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                        ? updateDate
                        : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid.")
                    };
                }
            );
        }

        public async Task<bool> UpdateProfileImageAsync(string imageUrl, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                @"
                MATCH (u:User {id: $userId})
                SET u.profileImage = $image, u.lastEditDate = datetime()
                ",
                new
                {
                    userId = userId,
                    image = imageUrl,
                }
            );
            return summary.Counters.PropertiesSet > 0;
        }

        public async Task<int> AddUserGender(string userId, string gender)
        {
            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (n:User {{id: $userId}})
                OPTIONAL MATCH (g:Gender {{name: apoc.text.capitalizeAll($gender)}})
                OPTIONAL MATCH (n)-[:{GraphRelations.User.HadGender}]->(oldGender:Gender)
                WITH n, g, oldGender
                CALL apoc.do.when(
                    g IS NOT NULL,
                    'OPTIONAL MATCH (n)-[r:{GraphRelations.User.HadGender}]->(:Gender) DELETE r 
                    MERGE (n)-[r_new:{GraphRelations.User.HadGender}]->(g) 
                    RETURN r_new AS rel',
                    'RETURN null AS rel',
                    {{ n: n, g: g, oldGender: oldGender }}
                ) YIELD value
                RETURN count(value.rel) AS relationshipsCreated, value.gender AS gender",
                new { userId, gender },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["relationshipsCreated"].As<int>();
                }
            );
        }

        public async Task<List<string>> AddUserTags(string userId, List<string> tagNames)
        {

            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[K:{GraphRelations.User.HadTag}]->(c:Tag)
                DELETE K
                WITH  DISTINCT u, $tagNames AS tagNames
                UNWIND $tagNames AS tagName
                MERGE (c:Tag {{name: apoc.text.capitalizeAll(tagName)}})
                ON CREATE SET c.id = randomUUID()
                // Create the relationship only if it doesn't exist
                MERGE (u)-[:{GraphRelations.User.HadTag}]->(c)
                RETURN c.name AS name, c.id AS id;",
                new { userId, tagNames },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.Select(x => x["name"].As<string>()).ToList();
                }
                ) ?? [];
        }

        public async Task<List<string>> AddUserActivities(string userId, List<string> activities)
        {
            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[K:{GraphRelations.User.PreferedActivity}]->(c:Activity)
                DELETE K
                WITH DISTINCT u, $activities AS activities
                UNWIND activities AS activity
                MERGE (c:Activity {{name: apoc.text.capitalizeAll(activity)}})
                ON CREATE SET c.id = randomUUID()
                // Create the relationship only if it doesn't exist
                MERGE (u)-[r:{GraphRelations.User.PreferedActivity}]->(c)
                RETURN collect(c.name) AS activitiesList;",
        new { userId, activities },
        async result =>
        {
            var record = await result.SingleAsync();
            return (
                record["activitiesList"].As<List<string>>()
            );
        }
    ) ?? [];
        }
        
        public async Task<(int relationshipsCreated, List<string> priceRanges)> AddUserPriceRangesAsync(string userId, List<string> priceRanges)
        {
            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[K:{GraphRelations.User.HadPriceRange}]->(c:PriceRange)
                DELETE K
                WITH DISTINCT u, $priceRanges AS priceRanges
                UNWIND priceRanges AS priceRange
                MATCH (c:PriceRange {{name: apoc.text.capitalizeAll(priceRange)}})
                MERGE (u)-[r:{GraphRelations.User.HadPriceRange}]->(c)
                WITH u, collect(c.name) AS priceRangeNames, count(r) AS relationshipsCreated
                RETURN relationshipsCreated, priceRangeNames;",
                new { userId, priceRanges },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return (
                        record["relationshipsCreated"].As<int>(),
                        record["priceRangeNames"].As<List<string>>()
                    );
                }
            );
        }

        public async Task<(int relationshipsCreated, List<string> destinationTypes)> AddUserDestinationTypes(string userId, List<string> destinationTypes)
        {
            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[r:{GraphRelations.User.HadDestinationType}]->(:Type)
                DELETE r
                WITH u
                UNWIND $destinationTypes AS type
                OPTIONAL MATCH (m:Type {{name: apoc.text.capitalizeAll(type)}})
                WITH u, m, type
                CALL apoc.do.when(
                    m IS NOT NULL,
                    'MERGE (u)-[r:{GraphRelations.User.HadDestinationType}]->(m) RETURN m.name AS name, r AS rel',
                    'RETURN null AS name, null AS rel',
                    {{ u: u, m: m }}
                ) YIELD value
                WITH collect(value.name) AS names, count(value.rel) AS relationshipsCreated
                RETURN relationshipsCreated, names",
                new { userId, destinationTypes },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return (
                        record["relationshipsCreated"].As<int>(),
                        record["names"].As<List<string>>().Where(name => name != null).ToList()
                    );
                }
            );
        }

        public async Task<(int relationshipsCreated, List<string> groupSizes)> AddUserGroupSizes(string userId, List<string> groupSizes)
        {
            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[r:{GraphRelations.User.HadGroupSize}]->(:GroupSize)
                DELETE r
                WITH u
                UNWIND $groupSizes AS groupSize
                OPTIONAL MATCH (m:GroupSize {{name: apoc.text.capitalizeAll(groupSize)}})
                WITH u, m, groupSize
                CALL apoc.do.when(
                    m IS NOT NULL,
                    'MERGE (u)-[r:{GraphRelations.User.HadGroupSize}]->(m) RETURN m.name AS name, r AS rel',
                    'RETURN null AS name, null AS rel',
                    {{ u: u, m: m }}
                ) YIELD value
                WITH collect(value.name) AS names, count(value.rel) AS relationshipsCreated
                RETURN relationshipsCreated, names",
                new { userId, groupSizes },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return (
                        record["relationshipsCreated"].As<int>(),
                        record["names"].As<List<string>>().Where(name => name != null).ToList()
                    );
                }
            );
        }

        public async Task<(int relationshipsCreated, List<string> tripDurations)> AddUserTripDurationsAsync(string userId, List<string> tripDurations)
        {
            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[r:{GraphRelations.User.PreferedDuration}]->(:TripDuration)
                DELETE r
                WITH u
                UNWIND $tripDurations AS tripDuration
                OPTIONAL MATCH (m:TripDuration {{name: apoc.text.capitalizeAll(tripDuration)}})
                WITH u, m, tripDuration
                CALL apoc.do.when(
                    m IS NOT NULL,
                    'MERGE (u)-[r:{GraphRelations.User.PreferedDuration}]->(m) 
                    RETURN m.name AS name, r AS rel',
                    'RETURN null AS name, null AS rel',
                    {{ u: u, m: m }}
                ) YIELD value
                WITH collect(value.name) AS names, count(value.rel) AS relationshipsCreated
                RETURN relationshipsCreated, names",
                new { userId, tripDurations },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return (
                        record["relationshipsCreated"].As<int>(),
                        record["names"].As<List<string>>().Where(name => name != null).ToList()
                    );
                }
            );
        }

        public async Task<UserInfoDto> GetUserInfoAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                OPTIONAL MATCH (u)-[:{GraphRelations.User.LocatedIn}]->(city:City)
                OPTIONAL MATCH (city)-[:{GraphRelations.User.LocatedIn}]->(country:Country)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.Speak}]->(language:SpokenLanguage)
                OPTIONAL MATCH (u)-[:{GraphRelations.User.HadTag}]->(tag:Tag)
                RETURN u.id AS UserId, u.firstName AS FirstName, u.lastName AS LastName,
                       city.name AS City, country.name AS Country,
                       COLLECT(language.name) AS SpokenLanguages,
                       COLLECT(tag.name) AS Tags
                       ",
            new { userId },
            async result =>
            {
                var record = await result.SingleAsync();

                if (record == null) return null;

                return new UserInfoDto
                {
                    UserId = record["UserId"].As<string>(),
                    FirstName = record["FirstName"].As<string?>(),
                    LastName = record["LastName"].As<string?>(),
                    SpokenLanguages = record["SpokenLanguages"].As<List<string>>(),
                    Tags = record["Tags"].As<List<string>>(),
                };
            });

        }
    }
}