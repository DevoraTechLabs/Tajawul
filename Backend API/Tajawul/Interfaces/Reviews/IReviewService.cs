using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain;
using Tajawul.Models.DTOs.Review;

namespace Tajawul.Interfaces.Reviews
{
    public interface IReviewService
    {
        Task<List<Review>> GetReviewsAsync(ReviewsFilter reviewFilter);
        Task<Review> CreateReviewAsync(CreateReviewDto reviewDto, string userId);
        Task<Review> UpdateReviewAsync(UpdateReviewDto ReviewDto, string userId);
        Task<bool> DeleteReviewAsync(string reviewId, string userId);
    }
}
