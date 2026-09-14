using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tajawul.Interfaces.User.Interactions;

namespace Tajawul.Controllers.User.Interactions
{
    [ApiController]
    [Authorize]
    [Route("api/user/{tripId}")]
    //[EnableRateLimiting("fixed")]
    public class TripInteractionsController : ControllerBase
    {
        private readonly ITripInteractionsService _tripInteractionsService;

        public TripInteractionsController(ITripInteractionsService tripInteractionsService)
        {
            _tripInteractionsService = tripInteractionsService;
        }

        [Authorize]
        [HttpGet("tripStatus")]
        public async Task<IActionResult> GetUserStatus(string tripId)
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

                var status = await _tripInteractionsService.GetUserStatusAsync(tripId, userId);

                return Ok(status);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("wishTrip")]
        public async Task<IActionResult> WishTrip(string tripId)
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
                var wishesCount = await _tripInteractionsService.WishTripAsync(tripId, userId);

                return Ok(new { wishesCount = wishesCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("wishTrip")]
        public async Task<IActionResult> UnwishTrip(string tripId)
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
                var wishesCount = await _tripInteractionsService.UnwishTripAsync(tripId, userId);

                return Ok(new { wishesCount = wishesCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("favoriteTrip")]
        public async Task<IActionResult> FavoriteTrip(string tripId)
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
                var favoritesCount = await _tripInteractionsService.FavoriteTripAsync(tripId, userId);

                return Ok(new { favoritesCount = favoritesCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("favoriteTrip")]
        public async Task<IActionResult> UnfavoriteTrip(string tripId)
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
                var favoritesCount = await _tripInteractionsService.UnfavoriteTripAsync(tripId, userId);

                return Ok(new { favoritesCount = favoritesCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}

