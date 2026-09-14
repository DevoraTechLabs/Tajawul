using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.Reviews;
using Tajawul.Mappers;
using Tajawul.Models.DTOs.Review;

namespace Tajawul.Controllers.Review
{
    [ApiController]
    [Route("api/[controller]/")]
    //[EnableRateLimiting("fixed")]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost]
        public async Task<IActionResult> CreateReview(CreateReviewDto createReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var review = await _reviewService.CreateReviewAsync(createReviewDto, userId);

                return Ok(new { review = review.ToReviewDto() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDestinationReviews([FromQuery] ReviewsFilter reviewFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var reviews = await _reviewService.GetReviewsAsync(reviewFilter);

                var reviewDtos = reviews.Select(r => r.ToReviewDto()).ToList();

                return Ok(new { reviews = reviewDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut]
        public async Task<IActionResult> UpdateReview(UpdateReviewDto updateReviewDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var review = await _reviewService.UpdateReviewAsync(updateReviewDto, userId);

                return Ok(new { review = review.ToReviewDto() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{reviewId}")]
        public async Task<IActionResult> DeleteReview(string reviewId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var result = await _reviewService.DeleteReviewAsync(reviewId, userId);

                if (result == true) return Ok(new { message = "Review deleted succefully." });
                else return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

    }
}
