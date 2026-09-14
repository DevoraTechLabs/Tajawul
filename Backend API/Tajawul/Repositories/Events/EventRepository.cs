using Neo4j.Driver;
using Newtonsoft.Json;
using Tajawul.Helpers;
using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Events;
using Tajawul.Models.DTOs.Event;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Models.ViewModels.Event;
using Tajawul.Services;

namespace Tajawul.Repositories.Events
{
    public class EventRepository
    {
        private readonly Neo4jService _neo4jService;

        public EventRepository(Neo4jService neo4jService)
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

        public async Task<Event> CreateEventAsync(CreateEventDto eventDto, string destinationId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (d:Destination {{id: $destinationId}})
                CREATE (e:Event {{
                id: randomUUID(),
                organizerId: $destinationId,
                name: $name,
                description: $description,
                coverImage: $coverImage,
                images: $images,
                ticketPrice: $ticketPrice,
                maxTicketsNumber: $maxTicketsNumber,
                attendeesCount: $attendeesCount,
                interestedInCount: $interestedInCount,
                bookingUrl: $bookingUrl,
                location: $location,
                creationDate: datetime(),
                lastEditDate: datetime()}})
                WITH d, e
                MERGE (d)-[:{GraphRelations.Destination.Organized}]->(e)
                SET d.eventsCount = coalesce(d.eventsCount, 0) + 1

                RETURN e.id AS EventId, e.name AS Name, 
                e.description AS Description, e.coverImage AS CoverImage, 
                e.images AS Images, e.ticketPrice AS TicketPrice,
                e.maxTicketsNumber AS MaxTicketsNumber, 
                e.attendeesCount AS AttendeesCount,
                e.interestedInCount AS InterestedInCount, 
                e.bookingUrl AS BookingUrl,
                e.creationDate AS CreationDate, 
                e.lastEditDate AS LastEditDate, 
                d.id AS OrganizerId, e.location AS Location",
                new
                {
                    destinationId,
                    name = eventDto.Name,
                    description = eventDto.Description,
                    coverImage = eventDto.CoverImage,
                    images = eventDto.Images,
                    ticketPrice = eventDto.TicketPrice,
                    maxTicketsNumber = eventDto.MaxTicketsNumber,
                    bookingUrl = eventDto.BookingUrl,
                    location = JsonConvert.SerializeObject(eventDto.Location),
                    attendeesCount = 0,
                    interestedInCount = 0,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Event
                    {
                        EventId = record["EventId"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        Images = record["Images"].As<List<string>>(),
                        TicketPrice = record["TicketPrice"].As<double>(),
                        MaxTicketsNumber = record["MaxTicketsNumber"].As<int>(),
                        AttendeesCount = record["AttendeesCount"].As<int>(),
                        InterestedInCount = record["InterestedInCount"].As<int>(),
                        BookingUrl = record["BookingUrl"].As<string>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                            ? creationDate
                            : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var lastEditDate)
                            ? lastEditDate
                            : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid."),
                        OrganizerId = record["OrganizerId"].As<string>(),
                        Location = JsonConvert.DeserializeObject<List<EventLocation>>(record["Location"].As<string>() ?? "[]")
                    };
                }
            );
        }

        public async Task<Event> UpdateEventAsync(UpdateEventDto eventDto, string destinationId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"
                MATCH (d:Destination {{id: $destinationId}})
                MATCH (e:Event {{id: $eventId}})
                MERGE (d)-[r:{GraphRelations.Destination.Edited}]->(e)
                SET e.name = $name, 
                    e.description = $description,
                    e.coverImage = $coverImage,
                    e.images = $images,
                    e.ticketPrice = $ticketPrice,
                    e.maxTicketsNumber = $maxTicketsNumber,
                    e.bookingUrl = $bookingUrl,
                    e.location = $location,
                    e.lastEditDate = datetime()

                RETURN e.id AS EventId, e.name AS Name, 
                e.description AS Description, e.coverImage AS CoverImage, 
                e.images AS Images, e.ticketPrice AS TicketPrice,
                e.maxTicketsNumber AS MaxTicketsNumber, 
                e.attendeesCount AS AttendeesCount,
                e.interestedInCount AS InterestedInCount, 
                e.bookingUrl AS BookingUrl,
                e.creationDate AS CreationDate, 
                e.lastEditDate AS LastEditDate, 
                d.id AS OrganizerId, e.location AS Location",
                new
                {
                    eventId = eventDto.EventId,
                    destinationId,
                    name = eventDto.Name,
                    description = eventDto.Description,
                    coverImage = eventDto.CoverImage,
                    images = eventDto.Images,
                    ticketPrice = eventDto.TicketPrice,
                    maxTicketsNumber = eventDto.MaxTicketsNumber,
                    bookingUrl = eventDto.BookingUrl,
                    location = JsonConvert.SerializeObject(eventDto.Location)
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    if (record == null)
                    {
                        throw new InvalidOperationException("Event not found");
                    }

                    return new Event
                    {
                        EventId = record["EventId"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        Images = record["Images"].As<List<string>>(),
                        TicketPrice = record["TicketPrice"].As<double>(),
                        MaxTicketsNumber = record["MaxTicketsNumber"].As<int>(),
                        AttendeesCount = record["AttendeesCount"].As<int>(),
                        InterestedInCount = record["InterestedInCount"].As<int>(),
                        BookingUrl = record["BookingUrl"].As<string>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                            ? creationDate
                            : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var lastEditDate)
                            ? lastEditDate
                            : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid."),
                        OrganizerId = record["OrganizerId"].As<string>(),
                        Location = JsonConvert.DeserializeObject<List<EventLocation>>(record["Location"].As<string>() ?? "[]")
                    };
                }
            );
        }

        public async Task<List<Event>> GetEventsAsync(EventFilter filter)
        {
            var query = new List<string>
            {
                "MATCH (e:Event)",
                $"MATCH (d:Destination)-[r:{GraphRelations.Destination.Organized}]->(e)",
                $"OPTIONAL MATCH (e)-[:{GraphRelations.Event.HadTag}]->(tag:Tag)",
                $"OPTIONAL MATCH (e)-[:{GraphRelations.Event.HadPriceRange}]->(priceRange:PriceRange)",
                $"OPTIONAL MATCH (e)-[:{GraphRelations.Event.HadStatus}]->(status:Status)",
                $"OPTIONAL MATCH (e)-[:{GraphRelations.Event.StartedOn}]->(startOn:DateTime)",
                $"OPTIONAL MATCH (e)-[:{GraphRelations.Event.EndedOn}]->(endOn:DateTime)",
                $"OPTIONAL MATCH (e)-[:{GraphRelations.Event.LocatedIn}]->(city:City)",
                $"OPTIONAL MATCH (city)-[:{GraphRelations.Event.LocatedIn}]->(country:Country)"
            };

            var withAliases = new HashSet<string> { "e", "d" , "status", "priceRange", "startOn",
                "endOn", "city", "country"};

            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(filter.EventId))
            {
                conditions.Add("e.id = $EventId");
                parameters["EventId"] = filter.EventId;
            }

            if (!string.IsNullOrEmpty(filter.Name))
            {
                conditions.Add("e.name CONTAINS $Name");
                parameters["Name"] = filter.Name;
            }

            if (!string.IsNullOrEmpty(filter.OrganizerId))
            {
                conditions.Add("e.organizerId = $OrganizerId");
                parameters["OrganizerId"] = filter.OrganizerId;
            }

            if (filter.TicketPrice.HasValue)
            {
                // Use a small epsilon for floating point comparison
                double epsilon = 0.001;
                conditions.Add("abs(e.ticketPrice - $TicketPrice) < $Epsilon");
                parameters["TicketPrice"] = filter.TicketPrice.Value;
                parameters["Epsilon"] = epsilon;
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

            if (filter.StartOn.HasValue)
            {
                conditions.Add("startOn.value = $StartOn");
                parameters["StartOn"] = filter.StartOn;
            }

            if (filter.EndOn.HasValue)
            {
                conditions.Add("endOn.value = $EndOn");
                parameters["EndOn"] = filter.EndOn;
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

            if (!string.IsNullOrEmpty(filter.Tag))
            {
                query.Add($"OPTIONAL MATCH (e)-[:{GraphRelations.Event.HadTag}]->(t:Tag)");
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
                "date" => "e.startOn",
                "price" => "e.ticketPrice",
                "attendees" => "e.attendeesCount",
                "interested" => "e.interestedInCount",
                "tickets" => "e.maxTicketsNumber",
                _ => "e.startOn"
            };

            string orderClause = filter.Ascending == true ? "ASC" : "DESC";

            //Pagination
            int skip = (filter.PageNumber - 1) * filter.PageSize;
            parameters["Skip"] = skip;
            parameters["Limit"] = filter.PageSize;
            query.Add($@"RETURN e.id AS EventId, 
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

                    return records.Select(record => new Event
                    {
                        EventId = record["EventId"].As<string>(),
                        Name = record["Name"].As<string>(),
                        Description = record["Description"].As<string>(),
                        OrganizerId = record["OrganizerId"].As<string>(),
                        CreationDate = DateTime.TryParse(record["CreationDate"].As<string>(), out var creationDate)
                            ? creationDate
                            : throw new InvalidOperationException("CreationDate is required and cannot be null or invalid."),
                        AttendeesCount = record["AttendeesCount"].As<int>(),
                        InterestedInCount = record["InterestedInCount"].As<int>(),
                        LastEditDate = DateTime.TryParse(record["LastEditDate"].As<string>(), out var lastEditDate)
                            ? lastEditDate
                            : throw new InvalidOperationException("LastEditDate is required and cannot be null or invalid."),
                        BookingUrl = record["BookingUrl"]?.As<string>(),
                        TicketPrice = record["TicketPrice"].As<double>(),
                        CoverImage = record["CoverImage"].As<string>(),
                        Images = record["Images"].As<List<string>>(),
                        MaxTicketsNumber = record["MaxTicketsNumber"].As<int>(),
                        PriceRange = record["PriceRange"]?.As<string>(),
                        Status = record["Status"]?.As<string>(),
                        Country = record["Country"]?.As<string>(),
                        City = record["City"]?.As<string>(),
                        StartOn = DateTime.TryParse(record["StartOn"]?.As<string>(), out var startOn)
                        ? startOn : (DateTime?)null,
                        EndOn = DateTime.TryParse(record["EndOn"]?.As<string>(), out var endOn)
                        ? endOn : (DateTime?)null,
                        Location = JsonConvert.DeserializeObject<List<EventLocation>>(record["Location"].As<string>() ?? "[]") ?? new List<EventLocation>()
                    }).ToList();

                }
                );
        }

        public async Task<List<EventUserDto>> GetEventUsersAsync(EventUsersFilter usersFilter, string eventId)
        {
            string? relation = null;

            switch (usersFilter.Relation)
            {
                case "attend":
                    relation = GraphRelations.User.Attended;
                    usersFilter.Relation = "attend";
                    break;
                case "interested":
                    relation = GraphRelations.User.InterestedIn;
                    usersFilter.Relation = "interested";
                    break;
                default:
                    relation = GraphRelations.User.Attended;
                    usersFilter.Relation = "attend";
                    break;
            }

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User)-[r:{relation}]->(e:Event {{id: $eventId}})
                RETURN u.id AS Id, 
                (u.firstName + ' ' + u.lastName) AS Name, 
                u.profileImage AS Image",

            new { eventId },

            async result =>
            {
                var records = await result.ToListAsync();

                return records.Select(record => new EventUserDto
                {
                    Id = record["Id"].As<string>(),
                    Name = record["Name"].As<string>(),
                    ProfileImage = record["Image"].As<string>(),
                }).ToList();
            }
                );
        }

        public async Task<bool> DeleteEventAsync(string organizerId, string eventId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (d:Destination {{id: $organizerId}})-[:{GraphRelations.Destination.Organized}]->(e:Event {{id: $eventId}})
                DETACH DELETE e
                ",
                new
                {
                    organizerId,
                    eventId
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

        public async Task<bool> UpdateEventCoverImageAsync(string imageUrl, string eventId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (e:Event {{id: $eventId}})
                SET e.coverImage = $coverImage, e.lastEditDate = datetime()
                
                WITH u, e

                MERGE (u)-[e2:{GraphRelations.User.Edited}]->(e)
                SET e2.date = datetime()
                ",
                new
                {
                    userId,
                    eventId,
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

        public async Task<int> GetImageCountForEventAsync(string eventId)
        {
            return await _neo4jService.ExecuteReadAsync(
            $@"
                MATCH (e:Event {{id: $eventId}})
                RETURN e.images AS Images
                ",
                new { eventId },
                async result =>
                {
                    var record = await result.SingleAsync();

                    var images = record["Images"].As<List<string>>();

                    return images.Count;
                }
            );
        }

        public async Task<bool> UpdateEventImagesAsync(List<string> imageUrls, string eventId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (e:Event {{id: $eventId}})
                SET e.images = $images, e.lastEditDate = datetime()
                
                WITH u, e

                MERGE (u)-[e2:{GraphRelations.User.Edited}]->(e)
                SET e2.date = datetime()
                ",
                new
                {
                    userId,
                    eventId,
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

        public async Task<bool> DeleteEventImagesAsync(List<string> imageUrls, string eventId, string userId)
        {
            var summary = await _neo4jService.ExecuteWriteAsync(
                $@"
                MATCH (u:User {{id: $userId}})
                MATCH (e:Event {{id: $eventId}})
                
                WITH u, e, [img IN e.images WHERE NOT img IN $images] AS updatedImages
                SET e.images = updatedImages
               
                WITH u, e

                MERGE (u)-[e2:{GraphRelations.User.Edited}]->(e)
                SET e2.date = datetime()
                ",
                new
                {
                    userId,
                    eventId,
                    images = imageUrls,
                }
            );

            return summary.Counters.PropertiesSet > 0;
        }
    }
}