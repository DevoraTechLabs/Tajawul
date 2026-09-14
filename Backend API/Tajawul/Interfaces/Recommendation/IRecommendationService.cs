using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.Domain.Events;
using Tajawul.Models.Domain.Trips;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.user;

namespace Tajawul.Interfaces.Recommendation
{
    public interface IRecommendationService
    {
        Task<PaginatedResultDto<Trip>> GetRecommendedTripsAsync(string userId, RecommendationFilter filter);

        Task<PaginatedResultDto<Event>> GetRecommendedEventsAsync(string userId, RecommendationFilter filter);
        Task<PaginatedResultDto<Destination>> GetRecommendedDestinationsAsync(string userId, RecommendationFilter filter);
        Task<PaginatedResultDto<RecommendedUserDto>> GetSimilarUsersAsync(string userId, RecommendationFilter filter);

    }
}
