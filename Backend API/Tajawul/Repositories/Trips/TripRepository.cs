using Neo4j.Driver;
using Newtonsoft.Json;
using Tajawul.Helpers;
using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.Domain.Trips;
using Tajawul.Models.DTOs.Trip;
using Tajawul.Models.ViewModels.Trip;
using Tajawul.Services;

namespace Tajawul.Repositories.Trips
{
    public class TripRepository
    {
        private readonly Neo4jService _neo4jService;

        public TripRepository(Neo4jService neo4jService)
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

        public async Task<Trip> CreateTripAsync(CreateTripDto tripDto, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $userId}})
                CREATE (t:Trip {{
                    id: randomUUID(),
                    title: $title,
                    description: $description,
                    coverImage: $coverImage,
                    sameCountry: $sameCountry,
                    wishesCount: $wishesCount,
                    favoritesCount: $favoritesCount,
                    clonesCount: $clonesCount,
                    destinationCount: $destinationCount,
                    creationDate: datetime(),
                    lastEditDate: datetime()
                }})
                WITH u, t
                MERGE (u)-[:{GraphRelations.User.CreatedTrip}]->(t)

                RETURN t.id AS TripId, t.title AS Title, t.description AS Description,
                       t.coverImage AS CoverImage,
                       t.creationDate AS CreationDate, 
                       t.lastEditDate AS LastEditDate, t.sameCountry AS SameCountry,
                       t.wishesCount AS WishesCount,
                       t.favoritesCount AS FavoritesCount, 
                       t.clonesCount AS ClonesCount, 
                       t.destinationCount AS DestinationCount,
                       [u.id, u.firstName + ' ' + u.lastName, u.username, COALESCE(u.profileImage, '')] AS Creator",
                new
                {
                    userId,
                    title = tripDto.Title,
                    description = tripDto.Description,
                    coverImage = string.Empty,
                    sameCountry = tripDto.SameCountry,
                    wishesCount = 0,
                    favoritesCount = 0,
                    clonesCount = 0,
                    destinationCount = 0
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Trip
                    {
                        TripId = record["TripId"].As<string>(),
                        Title = record["Title"].As<string>(),
                        Description = record["Description"].As<string>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        SameCountry = record["SameCountry"].As<bool>(),
                        WishesCount = record["WishesCount"].As<int>(),
                        FavoritesCount = record["FavoritesCount"].As<int>(),
                        ClonesCount = record["ClonesCount"].As<int>(),
                        DestinationsCount = record["DestinationCount"].As<int>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                            ? creationDate
                            : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                            ? updateDate
                            : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid."),
                        Creator = record["Creator"].As<List<string>>()
                    };
                }
            );
        }

        public async Task<Trip> UpdateTripAsync(UpdateTripDto tripDto, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User {{id: $userId}})-[:{GraphRelations.User.CreatedTrip}|:{GraphRelations.User.Cloned}]->(t:Trip {{id: $tripId}})
                SET t.title = $title, 
                    t.description = $description,
                    t.lastEditDate = datetime()

                MERGE (u)-[r:{GraphRelations.User.Edited}]->(t)
                ON CREATE SET r.date = datetime()

                RETURN 
                    t.id AS TripId, 
                    t.title AS Title, 
                    t.description AS Description,
                    t.coverImage AS CoverImage, 
                    t.sameCountry AS SameCountry,
                    t.destinations AS Destinations,
                    t.wishesCount AS WishesCount, 
                    t.favoritesCount AS FavoritesCount,
                    t.clonesCount AS ClonesCount, 
                    t.destinationCount AS DestinationCount,
                    t.creationDate AS CreationDate, 
                    t.lastEditDate AS LastEditDate,
                    [u.id, u.firstName + ' ' + u.lastName, u.username, COALESCE(u.profileImage, '')] AS Creator",
                new
                {
                    userId,
                    tripId = tripDto.TripId,
                    title = tripDto.Title,
                    description = tripDto.Description,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    if (record == null)
                    {
                        throw new InvalidOperationException("Trip not found.");
                    }

                    return new Trip
                    {
                        TripId = record["TripId"].As<string>(),
                        Title = record["Title"].As<string>(),
                        Description = record["Description"].As<string>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        SameCountry = record["SameCountry"].As<bool>(),
                        WishesCount = record["WishesCount"].As<int>(),
                        FavoritesCount = record["FavoritesCount"].As<int>(),
                        ClonesCount = record["ClonesCount"].As<int>(),
                        DestinationsCount = record["DestinationCount"].As<int>(),
                        //Destinations = JsonConvert.DeserializeObject<List<TripDestination>>(record["Destinations"].As<string>() ?? "[]"),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                            ? creationDate
                            : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                            ? updateDate
                            : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid."),
                        Creator = record["Creator"].As<List<string>>()
                    };
                }
            );
        }

        public async Task<bool> DeleteTripAsync(string userId, string tripId)
        {
            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            var tripExist = await IsNodeExist(tripId, "Trip");
            if (!tripExist)
                throw new Exception("Trip not found");

            var owned = await IsRelationExist(GraphRelations.User.CreatedTrip, userId, "User", tripId, "Trip");
            var cloned = await IsRelationExist(GraphRelations.User.Cloned, userId, "User", tripId, "Trip");

            if (!owned && !cloned)
                throw new Exception("User not authorized to edit this trip");

            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})-[:{GraphRelations.User.CreatedTrip}|{GraphRelations.User.Cloned}]->(t:Trip {{id: $tripId}})
                DETACH DELETE t
                ",
                new
                {
                    userId,
                    tripId
                }
            );

            int relationshipsDeleted = summary.Counters.RelationshipsDeleted;
            if (relationshipsDeleted > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> AssignDestinationAsync(string userId, string tripId, string destinationId, int day)
        {

            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            var destinationExist = await IsNodeExist(destinationId, "Destination");
            if (!destinationExist)
                throw new Exception("Destination not found");

            var tripExist = await IsNodeExist(tripId, "Trip");
            if (!tripExist)
                throw new Exception("Trip not found");

            var owned = await IsRelationExist(GraphRelations.User.CreatedTrip, userId, "User", tripId, "Trip");
            var cloned = await IsRelationExist(GraphRelations.User.Cloned, userId, "User", tripId, "Trip");

            if (!owned && !cloned)
                throw new Exception("User not authorized to edit this trip");

            var alreadyIncluded = await IsRelationExist(GraphRelations.Trip.Included, tripId, "Trip", destinationId, "Destination");

            if (alreadyIncluded)
                throw new Exception("Destination already included");

            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (t:Trip {{id: $tripId}})
                MATCH (d:Destination {{id: $destinationId}})

                MERGE (t)-[rel:{GraphRelations.Trip.Included}]->(d)
                ON CREATE SET 
                    rel.day = $day,
                    t.destinationCount = coalesce(t.destinationCount, 0) + 1
                ON MATCH SET 
                    rel.day = $day

                SET t.lastEditDate = datetime()
                ",
                new
                {
                    day,
                    tripId,
                    destinationId
                }
            );

            int relationshipsDeleted = summary.Counters.RelationshipsCreated;
            if (relationshipsDeleted > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> RemoveDestinationFromTripAsync(string userId, string tripId, string destinationId)
        {
            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            var destinationExist = await IsNodeExist(destinationId, "Destination");
            if (!destinationExist)
                throw new Exception("Destination not found");

            var tripExist = await IsNodeExist(tripId, "Trip");
            if (!tripExist)
                throw new Exception("Trip not found");

            var owned = await IsRelationExist(GraphRelations.User.CreatedTrip, userId, "User", tripId, "Trip");
            var cloned = await IsRelationExist(GraphRelations.User.Cloned, userId, "User", tripId, "Trip");

            if (!owned && !cloned)
                throw new Exception("User not authorized to edit this trip");

            var alreadyIncluded = await IsRelationExist(GraphRelations.Trip.Included, tripId, "Trip", destinationId, "Destination");

            if (!alreadyIncluded)
                throw new Exception("Destination not included");

            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (t:Trip {{id: $tripId}})-[rel:{GraphRelations.Trip.Included}]->(d:Destination {{id: $destinationId}})
        
                DELETE rel

                SET t.destinationCount = t.destinationCount - 1
                SET t.lastEditDate = datetime()",
                new
                {
                    tripId,
                    destinationId
                }
                );

            int relationshipsDeleted = summary.Counters.RelationshipsDeleted;
            if (relationshipsDeleted > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<List<TripDestination>> GetTripDestinationsAsync(string tripId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (t:Trip {{id: $tripId}})-[rel:{GraphRelations.Trip.Included}]->(d:Destination)
                OPTIONAL MATCH (reviewer:User)-[reviewed:{GraphRelations.User.Reviewed}]->(d)
                OPTIONAL MATCH (d)-[:{GraphRelations.Destination.LocatedIn}]->(city:City)
                OPTIONAL MATCH (city)-[:{GraphRelations.Destination.LocatedIn}]->(country:Country)
                OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadType}]->(type:Type)
                WITH t, d, city, rel, country, type, reviewed
                OPTIONAL MATCH (d)-[:{GraphRelations.Destination.HadPriceRange}]->(priceRange:PriceRange)
                OPTIONAL MATCH (d)-[:{GraphRelations.Destination.OpenAt}]->(openTime:Time)
                OPTIONAL MATCH (d)-[:{GraphRelations.Destination.CloseAt}]->(closeTime:Time)
                RETURN  
                rel.day AS Day,  
                d.id AS DestinationId,
                d.name AS Name,
                d.isOpen24Hours AS IsOpen24Hours,
                COALESCE(type.name, '') AS Type,
                d.coverImage AS CoverImage,
                d.images AS Images,
                d.locations AS Locations,
                COALESCE(city.name, '') AS City,
                COALESCE(country.name, '') AS Country,
                COALESCE(openTime.value, '') AS OpenTime,
                COALESCE(closeTime.value, '') AS CloseTime,
                COALESCE(priceRange.name, '') AS PriceRange,
                COALESCE(d.averageRating, 0) AS AverageRating,
                COALESCE(count(reviewed), 0) AS ReviewsCount",

                new { tripId },

                async result =>
                {
                    var tripDestinations = new List<TripDestination>();

                    await foreach (var record in result)
                    {
                        tripDestinations.Add(new TripDestination
                        {
                            DestinationId = record["DestinationId"].As<string>(),
                            Day = record["Day"].As<int>(),
                            Name = record["Name"].As<string>(),
                            Type = record["Type"].As<string>(),
                            CoverImage = record["CoverImage"].As<string>(),
                            Images = record["Images"].As<List<string>>(),
                            City = record["City"].As<string>(),
                            Country = record["Country"].As<string>(),
                            Locations = JsonConvert.DeserializeObject<List<DestinationLocation>>(record["Locations"].As<string>()),
                            IsOpen24Hours = record["IsOpen24Hours"].As<bool>(),
                            OpenTime = record["OpenTime"].As<string>(),
                            CloseTime = record["CloseTime"].As<string>(),
                            PriceRange = record["PriceRange"].As<string>(),
                            AverageRating = record["AverageRating"].As<float>(),
                            ReviewsCount = record["ReviewsCount"].As<int>()
                        });
                    }
                    return tripDestinations;
                }
            );
        }

        public async Task<List<Trip?>> GetTripsAsync(TripFilter filter)
        {
            var query = new List<string>            
            {
                "MATCH (t:Trip)",
                $"MATCH (u:User)-[r:{GraphRelations.User.CreatedTrip}]->(t)",
                $"OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadStatus}]->(status:Status)",
                $"OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadVisibility}]->(visibility:Visibility)",
                $"OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadPriceRange}]->(priceRange:PriceRange)",
                $"OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadDuration}]->(tripDuration:TripDuration)",
            };

            var withAliases = new HashSet<string> { "t", "u" , "status", "priceRange", "visibility",
                "tripDuration"};

            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(filter.TripId))
            {
                conditions.Add("t.id = $TripId");
                parameters["TripId"] = filter.TripId;
            }

            if (!string.IsNullOrEmpty(filter.Title))
            {
                conditions.Add("t.title CONTAINS $Title");
                parameters["Title"] = filter.Title;
            }

            if (!string.IsNullOrEmpty(filter.CreatorId))
            {
                conditions.Add("t.creator[0] = $CreatorId");
                parameters["CreatorId"] = filter.CreatorId;
            }

            if (!string.IsNullOrEmpty(filter.PriceRange))
            {
                conditions.Add("priceRange.name = $PriceRange");
                parameters["PriceRange"] = filter.PriceRange;
            }

            if (!string.IsNullOrEmpty(filter.Status))
            {
                conditions.Add("status.name = $Status");
                parameters["Status"] = filter.Status;
            }

            if (!string.IsNullOrEmpty(filter.Visibility))
            {
                conditions.Add("visibility.name = $Visibility");
                parameters["Visibility"] = filter.Visibility;
            }

            if (!string.IsNullOrEmpty(filter.TripDuration))
            {
                conditions.Add("tripDuration.name = $TripDuration");
                parameters["TripDuration"] = filter.TripDuration;
            }

            if (!string.IsNullOrEmpty(filter.Tag))
            {
                query.Add($"OPTIONAL MATCH (t)-[:{GraphRelations.Trip.HadTag}]->(t:Tag)");
                withAliases.Add("t");
                conditions.Add("t.name = $Tag");
                parameters["Tag"] = filter.Tag;
            }

            query.Add("WITH " + string.Join(", ", withAliases));

            if (conditions.Any())
            {
                query.Add("WHERE " + string.Join(" AND ", conditions));
            }

            string sortField = filter.sortBy?.ToLower() switch
            {
                "date" => "t.creationDate",
                "title" => "t.title",
                "wishes" => "t.wishesCount",
                "favorites" => "t.favoritesCount",
                "clones" => "t.clonesCount",
                "destinations" => "t.destinationCount",
                _ => "t.creationDate"
            };

            string orderClause = filter.Ascending == true ? "ASC" : "DESC";

            int skip = (filter.PageNumber - 1) * filter.PageSize;
            parameters["Skip"] = skip;
            parameters["Limit"] = filter.PageSize;

            query.Add($@"RETURN 
                        t.id AS TripId, t.title AS Title, t.description AS Description,
                        t.coverImage AS CoverImage, t.sameCountry AS SameCountry,
                        t.wishedCount AS WishedCount, t.favoriteCount AS FavoriteCount,
                        t.cloneCount AS CloneCount, t.destinationCount AS DestinationCount,
                        t.creationDate AS CreationDate, t.lastEditDate AS LastEditDate,
                        [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator,
                        priceRange.name AS PriceRange,
                        status.name AS Status,
                        visibility.name AS Visibility,
                        tripDuration.name AS TripDuration
            ORDER BY {sortField} {orderClause} SKIP $Skip LIMIT $Limit");

            return await _neo4jService.ExecuteReadAsync(
                string.Join(" ", query),
                parameters,
                async result =>
                {

                    var records = await result.ToListAsync();

                    if (records == null || !records.Any() || records.All(r => r.Values.Values.All(v => v == null)))
                    {
                        return [];
                    }

                    return records.Select(record => new Trip
                    {
                        TripId = record["TripId"].As<string>(),
                        Title = record["Title"].As<string>(),
                        Description = record["Description"].As<string>(),
                        PriceRange = record["PriceRange"].As<string>(),
                        Status = record["Status"].As<string>(),
                        Visibility = record["Visibility"].As<string>(),
                        TripDuration = record["TripDuration"].As<string>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        SameCountry = record["SameCountry"].As<bool?>() ?? false,
                        WishesCount = record["WishedCount"].As<int?>() ?? 0,
                        FavoritesCount = record["FavoriteCount"].As<int?>() ?? 0,
                        ClonesCount = record["CloneCount"].As<int?>() ?? 0,
                        DestinationsCount = record["DestinationCount"].As<int?>() ?? 0,
                        CreationDate = DateTime.Parse(record["CreationDate"].As<string>()),
                        LastEditDate = DateTime.Parse(record["LastEditDate"].As<string>()),
                        Creator = record["Creator"].As<List<string>>(),
                    }).ToList();
                }
            );
        }

        public async Task<List<TripUserDto>> GetTripUsersAsync(TripUserFilter usersFilter, string tripId)
        {
            string? relation = null;

            switch (usersFilter.Relation)
            {
                case "created":
                    relation = GraphRelations.User.CreatedTrip;
                    usersFilter.Relation = "created";
                    break;
                case "wish":
                    relation = GraphRelations.User.WishedTrip;
                    usersFilter.Relation = "wish";
                    break;
                case "favorite":
                    relation = GraphRelations.User.FavoritedTrip;
                    usersFilter.Relation = "favorite";
                    break;
                case "clone":
                    relation = GraphRelations.User.Cloned;
                    usersFilter.Relation = "clone";
                    break;
                default:
                    relation = GraphRelations.User.CreatedTrip;
                    usersFilter.Relation = "created";
                    break;
            }

            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (u:User)-[r:{relation}]->(t:Trip {{id: $tripId}})
                RETURN u.id AS Id, (u.firstName + u.lastName) AS Name, u.profileImage AS Image",

                new { tripId },

                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new TripUserDto
                    {
                        Id = record["Id"].As<string>(),
                        Name = record["Name"].As<string>(),
                        ProfileImage = record["Image"].As<string>(),
                    }).ToList();
                }
            );
        }

        public async Task<bool> UpdateTripCoverImageAsync(string imageUrl, string tripId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (t:Trip {{id: $tripId}})
                SET t.coverImage = $coverImage, t.lastEditDate = datetime()
                
                WITH u, t

                MERGE (u)-[e:{GraphRelations.User.Edited}]->(t)
                SET e.date = datetime()
                ",
                new
                {
                    userId,
                    tripId,
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

        public async Task<Trip> CloneAsync(string tripId, string userId)
        {
            var userExist = await IsNodeExist(userId, "User");
            if (!userExist)
                throw new Exception("User not found");

            var tripExist = await IsNodeExist(tripId, "Trip");
            if (!tripExist)
                throw new Exception("Trip not found");

            return await _neo4jService.ExecuteReadAsync(
                $@"
                
                MATCH (u:User {{id: $userId}})
                MATCH (original:Trip {{id: $tripId}})
                
                CALL {{
                  WITH original
                  CREATE (copy:Trip)
                  SET copy += original, copy.id = randomUUID(),
                      copy.creationDate = datetime(), copy.lastEditDate = datetime(),
                      copy.clonesCount = 0, copy.favoritesCount = 0, copy.wishesCount = 0
                  RETURN copy
                }}

                WITH original, copy, u
                MATCH (original)-[r]->(target)
                WHERE NOT TYPE(r) IN ['{GraphRelations.Trip.HadStatus}', '{GraphRelations.Trip.HadVisibility}']
                CALL {{
                  WITH copy, r, target
                  CALL apoc.create.relationship(copy, type(r), properties(r), target) YIELD rel
                  RETURN rel
                }}


                WITH DISTINCT original, copy, u
                MERGE (u)-[r1:{GraphRelations.User.Created}]->(copy)
                ON CREATE SET r1.date = datetime()

                MERGE (copy)-[r2:{GraphRelations.User.Cloned}]->(original)
                ON CREATE SET r2.date = datetime(), original.clonesCount = original.clonesCount + 1

                RETURN copy.id AS TripId, copy.title AS Title, copy.description AS Description,
                       copy.coverImage AS CoverImage,
                       copy.creationDate AS CreationDate, 
                       copy.lastEditDate AS LastEditDate, copy.sameCountry AS SameCountry,
                       copy.wishesCount AS WishesCount,
                       copy.favoritesCount AS FavoritesCount, 
                       copy.clonesCount AS ClonesCount, 
                       copy.destinationCount AS DestinationCount,
                       [u.id, u.firstName + ' ' + u.lastName, u.username, COALESCE(u.profileImage, '')] AS Creator",
                new
                {
                    userId,
                    tripId
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Trip
                    {
                        TripId = record["TripId"].As<string>(),
                        Title = record["Title"].As<string>(),
                        Description = record["Description"].As<string>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        SameCountry = record["SameCountry"].As<bool>(),
                        WishesCount = record["WishesCount"].As<int>(),
                        FavoritesCount = record["FavoritesCount"].As<int>(),
                        ClonesCount = record["ClonesCount"].As<int>(),
                        DestinationsCount = record["DestinationCount"].As<int>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                            ? creationDate
                            : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var updateDate)
                            ? updateDate
                            : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid."),
                        Creator = record["Creator"].As<List<string>>()
                    };
                }
            );
        }

    }
}