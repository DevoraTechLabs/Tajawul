using Tajawul.Models.Domain;
using Tajawul.Models.ViewModels.Review;

namespace Tajawul.Mappers
{
    public static class ReviewMapper
    {
        public static ReviewDto ToReviewDto(this Review review)
        {
            return new ReviewDto
            {
                ReviewId = review.ReviewId,
                UserId = review.UserId,
                Creator = review.Creator,
                DestinationId = review.DestinationId,
                Rate = review.Rate,
                Comment = review.Comment,
                Date = review.Date
            };
        }
    }
}
