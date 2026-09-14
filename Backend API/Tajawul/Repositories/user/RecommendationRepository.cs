using Neo4j.Driver;
using Newtonsoft.Json;
using Tajawul.Helpers;
using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.Domain.Events;
using Tajawul.Models.Domain.General;
using Tajawul.Models.Domain.Trips;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.user;
using Tajawul.Services;

namespace Tajawul.Repositories.user
{

    public class RecommendationRepository(Neo4jService neo4jService)
    {
        private readonly Neo4jService _neo4jService = neo4jService;

        public async Task<PaginatedResultDto<Trip>> GetRecommendedTripsAsync(string userId , RecommendationFilter filter)
        {
            var skip = (filter.PageNumber - 1) * filter.PageSize;

            var query = $@"
            MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Recommended}]->(t:Trip)
            OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadStatus}]->(status:Status)
            OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadVisibility}]->(visibility:Visibility)
            OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadPriceRange}]->(priceRange:PriceRange)
            OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadDuration}]->(tripDuration:TripDuration)
            RETURN 
                t.id AS TripId, 
                t.title AS Title, 
                t.description AS Description,
                t.coverImage AS CoverImage, 
                t.sameCountry AS SameCountry,
                t.wishedCount AS WishedCount, 
                t.favoriteCount AS FavoriteCount,
                t.cloneCount AS CloneCount, 
                t.destinationCount AS DestinationCount,
                t.creationDate AS CreationDate, 
                t.lastEditDate AS LastEditDate,
                [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                priceRange.name AS PriceRange,
                status.name AS Status,
                visibility.name AS Visibility,
                tripDuration.name AS TripDuration
            ORDER BY r.score DESC, r.rank DESC
            SKIP $skip
            LIMIT $limit";

            var result = await _neo4jService.ExecuteReadAsync(
                query,
                new {
                    userId,
                    skip,
                    limit = filter.PageSize },
                async records =>
                {
                    var trips = new List<Trip>();
                    await records.ForEachAsync(record =>
                    {
                        trips.Add(new Trip
                        {
                            TripId = record["TripId"]?.As<string>() ?? string.Empty,
                            Title = record["Title"]?.As<string>() ?? string.Empty,
                            Description = record["Description"]?.As<string>() ?? string.Empty,
                            CoverImage = record["CoverImage"]?.As<string>() ?? string.Empty,
                            SameCountry = record["SameCountry"]?.As<bool>() ?? false,
                            WishesCount = record["WishedCount"]?.As<int>() ?? 0,
                            FavoritesCount = record["FavoriteCount"]?.As<int>() ?? 0,
                            ClonesCount = record["CloneCount"]?.As<int>() ?? 0,
                            DestinationsCount = record["DestinationCount"]?.As<int>() ?? 0,
                            CreationDate = DateTime.Parse(record["CreationDate"].As<string>()),
                            LastEditDate = DateTime.Parse(record["LastEditDate"].As<string>()),
                            Creator = record["Creator"]?.As<List<string>>() ?? new(),
                            PriceRange = record["PriceRange"]?.As<string>(),
                            Status = record["Status"]?.As<string>(),
                            Visibility = record["Visibility"]?.As<string>(),
                            TripDuration = record["TripDuration"]?.As<string>()
                        });
                    });

                    return trips;
                }
            );

            return new PaginatedResultDto<Trip>
            {
                Items = result ?? new(),
                TotalCount = await GetTotalRecommendedTripCountAsync(userId), 
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<PaginatedResultDto<Event>> GetRecommendedEventsAsync(string userId, RecommendationFilter filter)
        {
            var skip = (filter.PageNumber - 1) * filter.PageSize;

            var query = $@"
    MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Recommended}]->(e:Event)
    MATCH (d:Destination)-[:{GraphRelations.Destination.Organized}]->(e)
    OPTIONAL MATCH (e)-[:{GraphRelations.Event.HadStatus}]->(status:Status)
    OPTIONAL MATCH (e)-[:{GraphRelations.Event.HadPriceRange}]->(priceRange:PriceRange)
    OPTIONAL MATCH (e)-[:{GraphRelations.Event.StartedOn}]->(startOn:DateTime)
    OPTIONAL MATCH (e)-[:{GraphRelations.Event.EndedOn}]->(endOn:DateTime)
    OPTIONAL MATCH (e)-[:{GraphRelations.Event.LocatedIn}]->(city:City)
    OPTIONAL MATCH (city)-[:{GraphRelations.Event.LocatedIn}]->(country:Country)

    RETURN 
        e.id AS EventId,
        e.name AS Name,
        e.description AS Description,
        d.id AS OrganizerId,
        e.creationDate AS CreationDate,
        e.attendeesCount AS AttendeesCount,
        e.interestedInCount AS InterestedInCount,
        e.lastEditDate AS LastEditDate,
        e.bookingUrl AS BookingUrl,
        e.ticketPrice AS TicketPrice,
        e.coverImage AS CoverImage,
        e.images AS Images,
        e.maxTicketsNumber AS MaxTicketsNumber,
        priceRange.name AS PriceRange,
        status.name AS Status,
        country.name AS Country,
        city.name AS City,
        startOn.value AS StartOn,
        endOn.value AS EndOn,
        e.location AS Location
    ORDER BY r.score DESC, r.rank DESC
    SKIP $skip
    LIMIT $limit";

            var result = await _neo4jService.ExecuteReadAsync(
                query,
                new { userId, skip, limit = filter.PageSize },
                async records =>
                {
                    var events = new List<Event>();
                    await records.ForEachAsync(record =>
                    {
                        var creationDateStr = record["CreationDate"]?.As<string>();
                        var lastEditDateStr = record["LastEditDate"]?.As<string>();
                        var startOnStr = record["StartOn"]?.As<string>();
                        var endOnStr = record["EndOn"]?.As<string>();

                        var creationDate = string.IsNullOrEmpty(creationDateStr)
                            ? throw new InvalidOperationException("CreationDate is required.")
                            : DateTime.Parse(creationDateStr);

                        var lastEditDate = string.IsNullOrEmpty(lastEditDateStr)
                            ? throw new InvalidOperationException("LastEditDate is required.")
                            : DateTime.Parse(lastEditDateStr);

                        events.Add(new Event
                        {
                            EventId = record["EventId"]?.As<string>() ?? string.Empty,
                            Name = record["Name"]?.As<string>() ?? "Untitled",
                            Description = record["Description"]?.As<string>(),
                            OrganizerId = record["OrganizerId"]?.As<string>() ?? string.Empty,
                            CreationDate = creationDate,
                            AttendeesCount = record["AttendeesCount"]?.As<int>() ?? 0,
                            InterestedInCount = record["InterestedInCount"]?.As<int>() ?? 0,
                            LastEditDate = lastEditDate,
                            BookingUrl = record["BookingUrl"]?.As<string>(),
                            TicketPrice = record["TicketPrice"]?.As<double>() ?? 0.0,
                            CoverImage = record["CoverImage"]?.As<string>() ?? string.Empty,
                            Images = record["Images"]?.As<List<string>>() ?? new(),
                            MaxTicketsNumber = record["MaxTicketsNumber"]?.As<int>() ?? 0,
                            PriceRange = record["PriceRange"]?.As<string>(),
                            Status = record["Status"]?.As<string>(),
                            Country = record["Country"]?.As<string>(),
                            City = record["City"]?.As<string>(),
                            StartOn = string.IsNullOrEmpty(startOnStr) ? (DateTime?)null : DateTime.Parse(startOnStr),
                            EndOn = string.IsNullOrEmpty(endOnStr) ? (DateTime?)null : DateTime.Parse(endOnStr),
                            Location = JsonConvert.DeserializeObject<List<EventLocation>>(record["Location"]?.As<string>() ?? "[]") ?? new()
                        });
                    });

                    return events;
                }
            );

            return new PaginatedResultDto<Event>
            {
                Items = result ?? new(),
                TotalCount = await GetTotalRecommendedEventCountAsync(userId),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<PaginatedResultDto<Destination>> GetRecommendedDestinationsAsync(string userId, RecommendationFilter filter)
        {
            var skip = (filter.PageNumber - 1) * filter.PageSize;

            var query = $@"
            MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Recommended}]->(d:Destination)
            OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadType}]->(type:Type)
            OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadPriceRange}]->(priceRange:PriceRange)
            OPTIONAL MATCH (d)-[:{GraphRelations.Destination.OpenAt}]->(openTime:Time)
            OPTIONAL MATCH (d)-[:{GraphRelations.Destination.CloseAt}]->(closeTime:Time)
            OPTIONAL MATCH (d)-[:{GraphRelations.Destination.LocatedIn}]->(city:City)
            OPTIONAL MATCH (city)-[:{GraphRelations.Destination.LocatedIn}]->(country:Country)

            RETURN 
                d.id AS DestinationId,
                d.name AS Name,
                d.description AS Description,
                d.isOpen24Hours AS IsOpen24Hours,
                d.averageRating AS AverageRating,
                d.visitorsCount AS VisitorsCount,
                d.reviewsCount AS ReviewsCount,
                d.eventsCount AS EventsCount,
                d.followersCount AS FollowersCount,
                d.wishesCount AS WishesCount,
                d.favoritesCount AS FavoritesCount,
                d.isVerified AS IsVerified,
                [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                d.images AS Images,
                d.coverImage AS CoverImage,
                d.contactInfo AS ContactInfo,
                d.establishedAt AS EstablishedAt,
                d.locations AS Locations,
                d.socialMediaLinks AS SocialMediaLinks,
                d.creationDate AS CreationDate,
                d.lastEditDate AS LastEditDate,
                type.name AS Type,
                priceRange.name AS PriceRange,
                openTime.value AS OpenTime,
                closeTime.value AS CloseTime,
                city.name AS City,
                country.name AS Country
            ORDER BY r.score DESC, r.rank DESC
            SKIP $skip
            LIMIT $limit";

            var result = await _neo4jService.ExecuteReadAsync(
                query,
                new { userId, skip, limit = filter.PageSize },
                async records =>
        {
            var destinations = new List<Destination>();
            await records.ForEachAsync(record =>
            {
                destinations.Add(new Destination
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
                }

                    );
            });

            return destinations;
            });
            return new PaginatedResultDto<Destination>
            {
                Items = result ?? new(),
                TotalCount = await GeTotalRecommendedDestinationCountAsync(userId),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<PaginatedResultDto<RecommendedUserDto>> GetSimilarUsersAsync(string userId, RecommendationFilter filter)
        {
            var skip = (filter.PageNumber - 1) * filter.PageSize;

            var query = $@"
            MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Similar}]->(user:User)
            OPTIONAL MATCH (user)-[:{GraphRelations.Destination.LocatedIn}]->(city:City)
            OPTIONAL MATCH (city)-[:{GraphRelations.Destination.LocatedIn}]->(country:Country)
            RETURN 
                user.id AS UserId,
                user.firstName AS FirstName,
                user.lastName AS LastName,
                user.profileImage AS ProfileImage,
                city.name AS CityName,
                country.name AS CountryName,
                user.createdDestinationCount AS CreatedDestinationCount,
                user.editedDestinationCount AS EditedDestinationCount,
                user.visitedDestinationCount AS VisitedDestinationCount,
                user.followedDestinationCount AS FollowedDestinationCount,
                user.createdTripCount AS CreatedTripCount,
                user.clonedTripCount AS ClonedTripCount,
                user.postsCount AS PostsCount
                ORDER BY r.rank DESC
                SKIP $skip
                LIMIT $limit";


            var result = await _neo4jService.ExecuteReadAsync(
                query,
                new { userId, skip, limit = filter.PageSize },
                async records =>
                {
                    var users = new List<RecommendedUserDto>();
                    await records.ForEachAsync(record =>
                    {
                        users.Add(new RecommendedUserDto
                        {
                            UserId = record["UserId"].As<string>(),
                            FirstName = record["FirstName"].As<string?>(),
                            LastName = record["LastName"].As<string?>(),
                            ProfileImage = record["ProfileImage"].As<string?>(),
                            CityName = record["CityName"].As<string?>(),
                            CountryName = record["CountryName"].As<string?>(),
                            CreatedDestinationCount = record["CreatedDestinationCount"].As<int>(),
                            EditedDestinationCount = record["EditedDestinationCount"].As<int>(),
                            VisitedDestinationCount = record["VisitedDestinationCount"].As<int>(),
                            FollowedDestinationCount = record["FollowedDestinationCount"].As<int>(),
                            CreatedTripCount = record["CreatedTripCount"].As<int>(),
                            ClonedTripCount = record["ClonedTripCount"].As<int>(),
                            PostsCount = record["PostsCount"].As<int>()
                        });
                    });
                    return users;
                }
            );

            return new PaginatedResultDto<RecommendedUserDto>
            {
                Items = result ?? new(),
                TotalCount = await GetTotalRecommendedUserCountAsync(userId),
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };

        }

        private async Task<int> GetTotalRecommendedUserCountAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync($@"
                MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Similar}]->(user:User)
                RETURN COUNT(user) AS TotalUsers",
                new { userId },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["TotalUsers"].As<int>();
                }
            );
        }

        private async Task<int> GetTotalRecommendedTripCountAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
            MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Recommended}]->(t:Trip)
            RETURN COUNT(t) AS TotalTrips",
                new { userId },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["TotalTrips"].As<int>();
                }
            );
        }
        private async Task<int> GetTotalRecommendedEventCountAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
            MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Recommended}]->(t:Event)
            RETURN COUNT(t) AS TotalEvents",
                new { userId },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["TotalEvents"].As<int>();
                }
            );
        }

        private async Task<int> GeTotalRecommendedDestinationCountAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
            MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Recommended}]->(d:Destination)
            RETURN COUNT(d) AS TotalDestinations",
                new { userId },
                async result =>
                {
                    var record = await result.SingleAsync();
                    return record["TotalDestinations"].As<int>();
                }
            );
        }
    }
}