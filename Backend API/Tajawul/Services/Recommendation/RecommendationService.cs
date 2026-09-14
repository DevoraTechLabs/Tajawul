using Tajawul.Models.Domain;
using Tajawul.Models.DTOs.Review;
using Tajawul.Repositories;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.Reviews;
using Tajawul.Models.Domain.Trips;
using Tajawul.Repositories.user;
using Tajawul.Interfaces.Recommendation;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.Domain.Events;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.ViewModels.user;

namespace Tajawul.Services.Recommendation
{
    public class RecommendationService(RecommendationRepository repository) : IRecommendationService
    {
        private readonly RecommendationRepository _recommendationRepository = repository;
       public async Task<PaginatedResultDto<Trip>>  GetRecommendedTripsAsync(string userId, RecommendationFilter filter)
        {
            return await _recommendationRepository.GetRecommendedTripsAsync(userId, filter);
        }

        public async Task<PaginatedResultDto<Event>> GetRecommendedEventsAsync(string userId, RecommendationFilter filter)
        {
            return await _recommendationRepository.GetRecommendedEventsAsync(userId, filter);
        }

        public async Task<PaginatedResultDto<Destination>> GetRecommendedDestinationsAsync(string userId, RecommendationFilter filter)
        {
            return await _recommendationRepository.GetRecommendedDestinationsAsync(userId, filter);
        }

        public async Task<PaginatedResultDto<RecommendedUserDto>> GetSimilarUsersAsync(string userId, RecommendationFilter filter)
        {
            return await _recommendationRepository.GetSimilarUsersAsync(userId, filter);
        }
    }
}
