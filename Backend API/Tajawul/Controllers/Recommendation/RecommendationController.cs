using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Tajawul.Controllers.User;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.Recommendation;

namespace Tajawul.Controllers.Recommendation
{

    [ApiController]
    [Authorize(Policy = "FullyRegisteredUser")]
    [Route("api/[controller]/")]
    public class RecommendationController(IRecommendationService recommendationService, ILogger<RecommendationController> logger) : ControllerBase
    {
        private readonly IRecommendationService _recommendationService = recommendationService;
        private readonly ILogger<RecommendationController> _logger = logger;

        [HttpGet("trips")]
        public async Task<IActionResult> GetRecommendedTrips([FromQuery] RecommendationFilter tripRecommendationFilter)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var trips = await _recommendationService.GetRecommendedTripsAsync(userId, tripRecommendationFilter);
                return Ok(trips);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting recommended events for User ID: {UserId}", userId);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("events")]
        public async Task<IActionResult> GetRecommendedEvents([FromQuery] RecommendationFilter tripRecommendationFilter)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var events = await _recommendationService.GetRecommendedEventsAsync(userId, tripRecommendationFilter);
                return Ok(events);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting recommended events for User ID: {UserId}", userId);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("destinations")]
        public async Task<IActionResult> GetRecommendedDestinations([FromQuery] RecommendationFilter tripRecommendationFilter)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var destinations = await _recommendationService.GetRecommendedDestinationsAsync(userId, tripRecommendationFilter);
                return Ok(destinations);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting recommended destinations for User ID: {UserId}", userId);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetSimilarUsers([FromQuery] RecommendationFilter tripRecommendationFilter)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var users = await _recommendationService.GetSimilarUsersAsync(userId, tripRecommendationFilter);
                return Ok(users);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting similar users for User ID: {UserId}", userId);
                return StatusCode(500, "An error occurred");
            }
        }
    }
}
