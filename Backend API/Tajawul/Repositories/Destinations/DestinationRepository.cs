using Neo4j.Driver;
using NetTopologySuite.Operation.Relate;
using Newtonsoft.Json;
using Tajawul.Helpers;
using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs.Destination;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Services;

namespace Tajawul.Repositories.Destinations
{
    public class DestinationRepository
    {
        private readonly Neo4jService _neo4jService;
        
        public DestinationRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }


        public async Task<Destination> CreateDestinationAsync(CreateDestinationDto destinationDto, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                    CREATE (d:Destination {{
                    id: randomUUID(),
                    name: $name,
                    description: $description,
                    coverImage: $coverImage,
                    isVerified: $isVerified,
                    isOpen24Hours: $isOpen24Hours,
                    averageRating: $averageRating,
                    visitorsCount: $visitorsCount,
                    reviewsCount: $reviewsCount,
                    eventsCount: $eventsCount,
                    followersCount: $followersCount,
                    wishesCount: $wishesCount,
                    favoritesCount: $favoritesCount,
                    images: $images,
                    establishedAt: $establishedAt,
                    locations: $locations,
                    socialMediaLinks: $socialMediaLinks,
                    contactInfo: $contactInfo,
                    creationDate: datetime(),
                    lastEditDate: datetime()}})
                WITH u, d
                MERGE (u)-[:{GraphRelations.User.Created}]->(d)
                RETURN d.id AS DestinationId, d.name AS Name, d.description AS Description,
                d.isOpen24Hours AS IsOpen24Hours,
                d.averageRating AS AverageRating,
                d.visitorsCount AS VisitorsCount, d.reviewsCount AS ReviewsCount,
                d.eventsCount AS EventsCount, d.followersCount AS FollowersCount,
                d.favoritesCount AS FavoritesCount, d.wishesCount AS WishesCount,
                d.isVerified AS IsVerified,
                [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                d.images AS Images, d.coverImage AS CoverImage,
                d.contactInfo AS ContactInfo,
                d.establishedAt AS EstablishedAt,
                d.locations AS Locations,
                d.socialMediaLinks AS SocialMediaLinks,
                d.creationDate AS CreationDate, d.lastEditDate AS LastEditDate",
                new
                {
                    userId = userId,
                    name = destinationDto.Name,
                    description = destinationDto.Description,
                    isOpen24Hours = destinationDto.IsOpen24Hours,
                    coverImage = "",
                    images = new List<string>(),
                    averageRating = 0.0,
                    visitorsCount = 0,
                    reviewsCount = 0,
                    eventsCount = 0,
                    followersCount = 0,
                    favoritesCount = 0,
                    wishesCount = 0,
                    isVerified = false,
                    establishedAt = destinationDto.EstablishedAt,
                    locations = JsonConvert.SerializeObject(destinationDto.Locations),
                    socialMediaLinks = JsonConvert.SerializeObject(destinationDto.SocialMediaLinks),
                    contactInfo = JsonConvert.SerializeObject(destinationDto.ContactInfo)
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Destination
                    {
                        DestinationId = record["DestinationId"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                        IsOpen24Hours = record["IsOpen24Hours"].As<bool>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        AverageRating = record["AverageRating"].As<float>(),
                        VisitorsCount = record["VisitorsCount"].As<int>(),
                        ReviewsCount = record["ReviewsCount"].As<int>(),
                        EventsCount = record["EventsCount"].As<int>(),
                        FollowersCount = record["FollowersCount"].As<int>(),
                        FavoritesCount = record["FavoritesCount"].As<int>(),
                        WishesCount = record["WishesCount"].As<int>(),
                        IsVerified = record["IsVerified"].As<bool>(),
                        Creator = record["Creator"].As<List<string>>(),
                        Images = record["Images"].As<List<string>>(),
                        EstablishedAt = DateOnly.TryParse(record["EstablishedAt"].As<string>(), out var establishedDate)
                        ? establishedDate
                        : null,
                        Locations = JsonConvert.DeserializeObject<List<DestinationLocation>>(record["Locations"].As<string>() ?? "[]"),
                        ContactInfo = JsonConvert.DeserializeObject<List<ContactInfo>>(record["ContactInfo"].As<string>() ?? "[]"),
                        SocialMediaLinks = JsonConvert.DeserializeObject<List<SocialMediaLink>>(record["SocialMediaLinks"].As<string>() ?? "[]"),
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

        public async Task<List<Destination?>> GetDestinationsAsync(DestinationFilter filter)
        {
            var query = new List<string>
            {
                "MATCH (d:Destination)",
                $"MATCH (u:User)-[r:{GraphRelations.User.Created}]->(d)",
                $"OPTIONAL MATCH (reviewer:User)-[reviewed:{GraphRelations.User.Reviewed}]->(d)",
                $"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadType}]->(type:Type)",
                $"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadPriceRange}]->(priceRange:PriceRange)",
                $"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.OpenAt}]->(openTime:Time)",
                $"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.CloseAt}]->(closeTime:Time)",
                $"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.LocatedIn}]->(city:City)",
                $"OPTIONAL MATCH (city)-[:{GraphRelations.Destination.LocatedIn}]->(country:Country)"
            };

            var withAliases = new HashSet<string> { "d", "u" , "type", "priceRange", "openTime",
                "closeTime", "city", "country", "count(reviewed) AS ReviewsCount" };

            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(filter.DestinationId))
            {
                conditions.Add("d.id = $DestinationId");
                parameters["DestinationId"] = filter.DestinationId;
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                conditions.Add("d.name CONTAINS $Name");
                parameters["Name"] = filter.Name;
            }

            if (!string.IsNullOrEmpty(filter.Type))
            {
                conditions.Add("type.name = $Type");
                parameters["Type"] = filter.Type;
            }

            if (!string.IsNullOrEmpty(filter.PriceRange))
            {
                conditions.Add("priceRange.name = $PriceRange");
                parameters["PriceRange"] = filter.PriceRange;
            }

            if (filter.OpenTime.HasValue)
            {
                conditions.Add("openTime.value = $OpenTime");
                parameters["OpenTime"] = filter.OpenTime;
            }

            if (filter.CloseTime.HasValue)
            {
                conditions.Add("closeTime.value = $CloseTime");
                parameters["CloseTime"] = filter.CloseTime;
            }

            if (!string.IsNullOrEmpty(filter.City))
            {
                conditions.Add("city.name = $CityName");
                parameters["CityName"] = filter.City;
            }

            if (!string.IsNullOrEmpty(filter.Country))
            {
                conditions.Add("country.name = $CountryName");
                parameters["CountryName"] = filter.Country;
            }

            if (filter.AverageRating.HasValue)
            {
                conditions.Add("d.averageRating = $AverageRating");
                parameters["AverageRating"] = filter.AverageRating.Value;
            }

            if (filter.IsVerified.HasValue)
            {
                conditions.Add("d.isVerified = $IsVerified");
                parameters["IsVerified"] = filter.IsVerified.Value;
            }

            if (filter.IsOpen24Hours.HasValue)
            {
                conditions.Add("d.isOpen24Hours = $IsOpen24Hours");
                parameters["IsOpen24Hours"] = filter.IsOpen24Hours.Value;
            }

            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                conditions.Add("d.creator[0] = $CreatorId");
                parameters["CreatorId"] = filter.CreatorId;
            }

            if (filter.EstablishedAt.HasValue)
            {
                conditions.Add("d.establishedAt = $EstablishedAt");
                parameters["EstablishedAt"] = filter.EstablishedAt;
            }

            if (filter.CreationDate.HasValue)
            {
                conditions.Add("d.creationDate = $CreationDate");
                parameters["CreationDate"] = filter.CreationDate;
            }

            if (filter.LastEditDate.HasValue)
            {
                conditions.Add("d.lastEditDate = $LastEditDate");
                parameters["LastEditDate"] = filter.LastEditDate;
            }

            if (!string.IsNullOrEmpty(filter.Tag))
            {
                query.Add($"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadTag}]->(t:Tag)");
                withAliases.Add("t");
                conditions.Add("t.name = $Tag");
                parameters["Tag"] = filter.Tag;
            }

            if (!string.IsNullOrEmpty(filter.Activity))
            {
                query.Add($"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadActivity}]->(a:Activity)");
                withAliases.Add("a");
                conditions.Add("a.name = $Activity");
                parameters["Activity"] = filter.Activity;
            }

            if (!string.IsNullOrEmpty(filter.GroupSize))
            {
                query.Add($"OPTIONAL MATCH (d)-[:{GraphRelations.Destination.PreferedGroupSize}]->(g:GroupSize)");
                withAliases.Add("g");
                conditions.Add("g.name = $GroupSize");
                parameters["GroupSize"] = filter.GroupSize;
            }

            query.Add("WITH " + string.Join(", ", withAliases));

            if (conditions.Any())
            {
                query.Add("WHERE " + string.Join(" AND ", conditions));
            }

            //Sorting
            string sortField = filter.sortBy?.ToLower() switch
            {
                "averagerating" => "d.averageRating",
                "visitors" => "d.visitorsCount",
                "followers" => "d.followersCount",
                "wishes" => "d.wishesCount",
                "favorites" => "d.favoritesCount",
                "reviews" => "d.reviewsCount",
                "establishedDate" => "d.establishedAt",
                "creationDate" => "d.creationDate",
                "updateDate" => "d.lastEditDate",
                _ => "d.reviewsCount"
            };

            string orderClause = filter.Ascending == true ? "ASC" : "DESC";

            //Pagination
            int skip = (filter.PageNumber - 1) * filter.PageSize;
            parameters["Skip"] = skip;
            parameters["Limit"] = filter.PageSize;

            query.Add($@"RETURN d.id AS DestinationId, d.name AS Name, d.description AS Description,
                      d.isOpen24Hours AS IsOpen24Hours,
                      d.averageRating AS AverageRating, d.visitorsCount AS VisitorsCount,
                      ReviewsCount, d.eventsCount AS EventsCount,
                      d.followersCount AS FollowersCount, d.wishesCount AS WishesCount,
                      d.favoritesCount AS FavoritesCount, d.isVerified AS IsVerified,
                      [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                      d.images AS Images, d.coverImage AS CoverImage,
                      d.contactInfo AS ContactInfo, d.establishedAt AS EstablishedAt,
                      d.locations AS Locations, d.socialMediaLinks AS SocialMediaLinks,
                      d.creationDate AS CreationDate, d.lastEditDate AS LastEditDate,
                      type.name AS Type, priceRange.name AS PriceRange,
                      openTime.value AS OpenTime, closeTime.value AS CloseTime,
                      city.name AS City, country.name AS Country

                      ORDER BY {sortField} {orderClause} SKIP $Skip LIMIT $Limit");

            //for 

            return await _neo4jService.ExecuteReadAsync(
                string.Join(" ", query),
                parameters,
                async result =>
                {

                    var records = await result.ToListAsync();

                    //Check if records are empty or contain only nulls
                    if (records == null || !records.Any() || records.All(r => r.Values.Values.All(v => v == null)))
                    {
                        return [];
                    }

                    return records.Select(record => new Destination
                    {
                        DestinationId = record["DestinationId"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                        Type = record["Type"].As<string>(),
                        Country = record["Country"].As<string>(),
                        City = record["City"].As<string>(),
                        PriceRange = record["PriceRange"].As<string>(),
                        IsOpen24Hours = record["IsOpen24Hours"].As<bool>(),
                        OpenTime = TimeOnly.TryParse(record["OpenTime"].As<string>(), out var openTime)
                        ? openTime
                        : null,
                        CloseTime = TimeOnly.TryParse(record["CloseTime"].As<string>(), out var closeTime)
                        ? closeTime
                        : null,
                        CoverImage = record["CoverImage"].As<string>(),
                        AverageRating = record["AverageRating"].As<float>(),
                        VisitorsCount = record["VisitorsCount"].As<int>(),
                        ReviewsCount = record["ReviewsCount"].As<int>(),
                        EventsCount = record["EventsCount"].As<int>(),
                        FollowersCount = record["FollowersCount"].As<int>(),
                        FavoritesCount = record["FavoritesCount"].As<int>(),
                        WishesCount = record["WishesCount"].As<int>(),
                        IsVerified = record["IsVerified"].As<bool>(),
                        Creator = record["Creator"].As<List<string>>(),
                        Images = record["Images"].As<List<string>>(),
                        EstablishedAt = DateOnly.TryParse(record["EstablishedAt"].As<string>(), out var establishedDate)
                        ? establishedDate
                        : null,
                        Locations = JsonConvert.DeserializeObject<List<DestinationLocation>>(record["Locations"].As<string>()),
                        ContactInfo = JsonConvert.DeserializeObject<List<ContactInfo>>(record["ContactInfo"].As<string>()),
                        SocialMediaLinks = JsonConvert.DeserializeObject<List<SocialMediaLink>>(record["SocialMediaLinks"].As<string>()),
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

        public async Task<Destination> UpdateDestinationAsync(UpdateDestinationDto destinationDto, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})
                OPTIONAL MATCH (u)-[reviewed:{GraphRelations.User.Reviewed}]->(d)
                MERGE (u)-[r:{GraphRelations.User.Edited}]->(d)
                ON CREATE SET r.date = datetime()
                SET d.name = $name, d.description = $description,
                d.locations = $locations,
                d.socialMediaLinks = $socialMediaLinks, d.contactInfo = $contactInfo,
                d.isOpen24Hours = $isOpen24Hours,
                d.establishedAt = $establishedAt,
                d.lastEditDate = datetime()
                RETURN d.id AS DestinationId, d.name AS Name, d.description AS Description,
                d.averageRating AS AverageRating,
                d.visitorsCount AS VisitorsCount, count(reviewed) AS ReviewsCount,
                d.eventsCount AS EventsCount, d.followersCount AS FollowersCount,
                d.isVerified AS IsVerified, d.isOpen24Hours AS IsOpen24Hours,
                [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                d.coverImage AS CoverImage,
                d.images AS Images,
                d.establishedAt AS EstablishedAt,
                d.locations AS Locations,
                d.contactInfo AS ContactInfo,
                d.socialMediaLinks AS SocialMediaLinks,
                d.creationDate AS CreationDate, d.lastEditDate AS LastEditDate",
                new
                {
                    userId,
                    destinationId = destinationDto.DestinationId,
                    name = destinationDto.Name,
                    description = destinationDto.Description,
                    isOpen24Hours = destinationDto.IsOpen24Hours,
                    contactInfo = JsonConvert.SerializeObject(destinationDto.ContactInfo),
                    locations = JsonConvert.SerializeObject(destinationDto.Locations),
                    socialMediaLinks = JsonConvert.SerializeObject(destinationDto.SocialMediaLinks),
                    establishedAt = destinationDto.EstablishedAt
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    if (record == null)
                    {
                        throw new InvalidOperationException("Destination not found.");
                    }

                    return new Destination
                    {
                        DestinationId = record["DestinationId"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        IsOpen24Hours = record["IsOpen24Hours"].As<bool>(),
                        AverageRating = record["AverageRating"].As<float>(),
                        VisitorsCount = record["VisitorsCount"].As<int>(),
                        ReviewsCount = record["ReviewsCount"].As<int>(),
                        EventsCount = record["EventsCount"].As<int>(),
                        FollowersCount = record["FollowersCount"].As<int>(),
                        IsVerified = record["IsVerified"].As<bool>(),
                        Creator = record["Creator"].As<List<string>>(),
                        Images = record["Images"].As<List<string>>(),
                        EstablishedAt = DateOnly.TryParse(record["EstablishedAt"].As<string>(), out var establishedDate)
                        ? establishedDate
                        : null,
                        Locations = JsonConvert.DeserializeObject<List<DestinationLocation>>(record["Locations"].As<string>()),
                        ContactInfo = JsonConvert.DeserializeObject<List<ContactInfo>>(record["ContactInfo"].As<string>()),
                        SocialMediaLinks = JsonConvert.DeserializeObject<List<SocialMediaLink>>(record["SocialMediaLinks"].As<string>()),
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

        public async Task<bool> UpdateDestinationCoverImageAsync(string imageUrl, string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})
                SET d.coverImage = $coverImage, d.lastEditDate = datetime()
                
                WITH u, d

                MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                SET e.date = datetime()
                ",
                new
                {
                    userId,
                    destinationId,
                    coverImage = imageUrl,
                }
            );

            if (summary.Counters.PropertiesSet > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<int> GetImageCountForDestinationAsync(string destinationId)
        {
            return await _neo4jService.ExecuteReadAsync(
            $@"
                MATCH (d:Destination {{id: $destinationId}})
                RETURN d.images AS Images
                ",
                new { destinationId },
                async result =>
                {
                    var record = await result.SingleAsync();

                    var images = record["Images"].As<List<string>>();

                    return images.Count;
                }
            );
        }

        public async Task<bool> UpdateDestinationImagesAsync(List<string> imageUrls, string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})
                SET d.images = $images, d.lastEditDate = datetime()
                
                WITH u, d

                MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                SET e.date = datetime()
                ",
                new
                {
                    userId,
                    destinationId,
                    images = imageUrls,
                }
            );

            if (summary.Counters.PropertiesSet > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteDestinationImagesAsync(List<string> imageUrls, string destinationId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (d:Destination {{id: $destinationId}})
                
                WITH u, d, [img IN d.images WHERE NOT img IN $images] AS updatedImages
                SET d.images = updatedImages
               
                WITH u, d

                MERGE (u)-[e:{GraphRelations.User.Edited}]->(d)
                SET e.date = datetime()
                ",
                new
                {
                    userId,
                    destinationId,
                    images = imageUrls,
                }
            );

            return summary.Counters.PropertiesSet > 0;
        }

        public async Task<List<DestinationUserDto>> GetDestinationUsersAsync(DestinationUsersFilter usersFilter, string destinationId)
        {
            string? relation = null;

            switch (usersFilter.Relation)
            {
                case "contribute":
                    relation = GraphRelations.User.Edited;
                    usersFilter.Relation = "contribute";
                    break;
                case "wish":
                    relation = GraphRelations.User.Wished;
                    usersFilter.Relation = "wish";
                    break;
                case "visit":
                    relation = GraphRelations.User.Visited;
                    usersFilter.Relation = "visit";
                    break;
                case "follow":
                    relation = GraphRelations.User.Followed;
                    usersFilter.Relation = "follow";
                    break;
                case "favorite":
                    relation = GraphRelations.User.FavoritedDestination;
                    usersFilter.Relation = "favorite";
                    break;
                default:
                    relation = GraphRelations.User.Edited;
                    usersFilter.Relation = "contribute";
                    break;
            }

            return await _neo4jService.ExecuteReadAsync(
                $@"
                    MATCH (u:User)-[r:{relation}]->(d:Destination {{id: $destinationId}})
                    RETURN u.id AS Id, (u.firstName + u.lastName) AS Name, u.profileImage AS Image
                ",
                new { destinationId },
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
    }
}