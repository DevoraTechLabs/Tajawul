using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain;
using Tajawul.Models.DTOs.Review;
using Tajawul.Services;

namespace Tajawul.Repositories
{
    public class ReviewRepository
    {
        private readonly Neo4jService _neo4jService;

        public ReviewRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;

        }

        public async Task<List<Review>> GetReviewsAsync(ReviewsFilter filter)
        {

            var withAliases = new HashSet<string> { "d", "u" , "r" };

            var query = new List<string>
            {
                $"MATCH (u:User)-[r:{GraphRelations.User.Reviewed}]->(d:Destination)"
            };

            var conditions = new List<string>();
            var parameters = new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(filter.ReviewId))
            {
                conditions.Add("r.id = $ReviewId");
                parameters["ReviewId"] = filter.ReviewId;
            }

            if (!string.IsNullOrEmpty(filter.DestinationId))
            {
                conditions.Add("d.id = $DestinationId");
                parameters["DestinationId"] = filter.DestinationId;
            }

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                conditions.Add("u.id = $UserId");
                parameters["UserId"] = filter.UserId;
            }

            if (filter.Rate.HasValue)
            {
                conditions.Add("r.rate = $Rate");
                parameters["Rate"] = filter.Rate.Value;
            }

            query.Add("WITH " + string.Join(", ", withAliases));

            if (conditions.Any())
            {
                query.Add("WHERE " + string.Join(" AND ", conditions));
            }

            // Sorting
            string sortField = filter.sortBy?.ToLower() switch
            {
                "rate" => "r.rate",
                _ => "r.date"
            };

            string orderClause = filter.Ascending == true ? "ASC" : "DESC";

            // Pagination
            int skip = (filter.PageNumber - 1) * filter.PageSize;
            parameters["Skip"] = skip;
            parameters["Limit"] = filter.PageSize;

            query.Add($"RETURN u.id AS UserId, d.id AS DestinationId, r.id AS ReviewId, r.content AS Comment, r.rate AS Rate, r.date AS Date, [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator " +
                      $"ORDER BY {sortField} {orderClause} SKIP $Skip LIMIT $Limit");

            
            return await _neo4jService.ExecuteReadAsync(
                string.Join(" ", query),
                parameters,
                async result =>
                {
                    var records = await result.ToListAsync();

                    return records.Select(record => new Review
                    {
                        ReviewId = record["ReviewId"].As<string>(),
                        UserId = record["UserId"].As<string>(),
                        Creator = record["Creator"].As<List<string>>(),
                        DestinationId = record["DestinationId"].As<string>(),
                        Comment = record["Comment"].As<string>(),
                        Rate = record["Rate"].As<float>(),
                        Date = DateTime.Parse(record["Date"].As<string>())
                    }).ToList();
                }
            );
        }

        public async Task<Review> CreateReviewAsync(CreateReviewDto reviewDto, string userId)
        {

            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $id}}), (d:Destination {{id: $destinationId}})
                OPTIONAL MATCH (u)-[existingReview:{GraphRelations.User.Reviewed}]->(d)
                WITH u, d, existingReview 
                WHERE existingReview IS NULL
                CREATE (u)-[r:REVIEWED {{id: randomUUID(), content: $content, rate: $rate, date: datetime()}}]->(d)
                RETURN d.id AS DestinationId, r.id AS ReviewId, r.content AS Comment, r.rate AS Rate, r.date AS Date, [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator",
                new
                {
                    id = userId,
                    destinationId = reviewDto.DestinationId,
                    content = reviewDto.Comment,
                    rate = reviewDto.Rate,
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    return new Review
                    {
                        ReviewId = record["ReviewId"].As<string>(),
                        DestinationId = record["DestinationId"].As<string>(),
                        UserId = userId,
                        Creator = record["Creator"].As<List<string>>(),
                        Comment = record["Comment"].As<string>(),
                        Rate = record["Rate"].As<float>(),
                        Date = DateTime.Parse(record["Date"].As<string>())
                    };
                }
            );
        }

        public async Task<Review> UpdateReviewAsync(UpdateReviewDto reviewDto, string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                $@"MATCH (u:User {{id: $id}})-[r:{GraphRelations.User.Reviewed} {{id: $reviewId}}]->(d:Destination)
                SET r.content = $content, r.rate = $rate, r.date = datetime()
                RETURN d.id AS DestinationId, r.id AS ReviewId, r.content AS Comment, r.rate AS Rate, r.date AS Date, [u.id, u.firstName + ' ' + u.lastName, u.profileImage] AS Creator",
                new
                {
                    id = userId,
                    reviewId = reviewDto.ReviewId,
                    content = reviewDto.Comment,
                    rate = reviewDto.Rate
                },
                async result =>
                {
                    var record = await result.SingleAsync();

                    if (record == null)
                    {
                        throw new InvalidOperationException("Review not found or user is not authorized to update this review.");
                    }

                    return new Review
                    {
                        ReviewId = record["ReviewId"].As<string>(),
                        DestinationId = record["DestinationId"].As<string>(),
                        UserId = userId,
                        Creator = record["Creator"].As<List<string>>(),
                        Comment = record["Comment"].As<string>(),
                        Rate = record["Rate"].As<float>(),
                        Date = DateTime.Parse(record["Date"].As<string>())
                    };
                }
            );
        }

        public async Task<bool> DeleteReviewAsync(string reviewId, string userId)
        {

            var summary = await _neo4jService.ExecuteWriteAsync(
                @$"MATCH (u:User {{id: $userId}})-[r:{GraphRelations.User.Reviewed} {{id: $reviewId}}]->(d:Destination)
                DELETE r",
                new
                {
                    userId,
                    reviewId
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

        public async Task UpdateAverageRatingsAsync()
        {
            string relation = GraphRelations.User.Reviewed;

            await _neo4jService.ExecuteWriteAsync(
                $@"MATCH (d:Destination)<-[r:{relation}]-(User)
                WITH d, round(AVG(r.rate) * 100) / 100 AS averageRating
                SET d.averageRating = averageRating",
                new { }
            );
        }

    }
}
