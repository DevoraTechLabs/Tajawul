using Tajawul.Models.Domain;
using Tajawul.Models.DTOs.Review;
using Tajawul.Repositories;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.Reviews;

namespace Tajawul.Services.Reviews
{
    public class ReviewService : IReviewService
    {

        private readonly ReviewRepository _repository;

        public ReviewService(ReviewRepository repository)
        {
            _repository = repository;

        }

        public async Task<Review> CreateReviewAsync(CreateReviewDto reviewDto, string userId)
        {
            return await _repository.CreateReviewAsync(reviewDto, userId);
        }

        public async Task<bool> DeleteReviewAsync(string reviewId, string userId)
        {
            return await _repository.DeleteReviewAsync(reviewId, userId);
        }

        public async Task<List<Review>> GetReviewsAsync(ReviewsFilter reviewFilter)
        {
            return await _repository.GetReviewsAsync(reviewFilter);
        }

        public async Task<Review> UpdateReviewAsync(UpdateReviewDto ReviewDto, string userId)
        {
            return await _repository.UpdateReviewAsync(ReviewDto, userId);
        }
    }
}
