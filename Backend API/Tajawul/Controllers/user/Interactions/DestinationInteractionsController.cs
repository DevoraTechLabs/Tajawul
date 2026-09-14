using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tajawul.Interfaces.User.Interactions;

namespace Tajawul.Controllers.User.Interactions
{
    [ApiController]
    [Authorize(Policy = "FullyRegisteredUser")]
    [Route("api/user/{destinationId}")]
    //[EnableRateLimiting("fixed")]
    public class DestinationInteractionsController : ControllerBase
    {

        private readonly IDestinationInteractionsService _destinationInteractionsService;

        public DestinationInteractionsController(IDestinationInteractionsService destinationInteractionsService)
        {
            _destinationInteractionsService = destinationInteractionsService;
        }

        [Authorize]
        [HttpGet("destinationStatus")]
        public async Task<IActionResult> GetUserStatus(string destinationId)
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

                var status = await _destinationInteractionsService.GetUserStatusAsync(destinationId, userId);

                return Ok(status);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("follow")]
        public async Task<IActionResult> FollowDestination(string destinationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try { 

                var followersCount = await _destinationInteractionsService.FollowDestinationAsync(destinationId, userId);
            
                return Ok(new { followersCount = followersCount });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("follow")]
        public async Task<IActionResult> UnFollowDestination(string destinationId)
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
                var followersCount = await _destinationInteractionsService.UnfollowDestinationAsync(destinationId, userId);

                return Ok(new { follwersCount = followersCount });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("visit")]
        public async Task<IActionResult> VisitDestination(string destinationId)
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
                var visitorsCount = await _destinationInteractionsService.VisitDestinationAsync(destinationId, userId);

                return Ok(new { visitorsCount = visitorsCount });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("visit")]
        public async Task<IActionResult> UnVisitDestination(string destinationId)
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
                var visitorsCount = await _destinationInteractionsService.UnVisitDestinationAsync(destinationId, userId);

                return Ok(new { visitorsCount = visitorsCount });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("wish")]
        public async Task<IActionResult> WishDestination(string destinationId)
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
                var wishesCount = await _destinationInteractionsService.WishDestinationAsync(destinationId, userId);

                return Ok(new { wishesCount = wishesCount });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("wish")]
        public async Task<IActionResult> UnwishDestination(string destinationId)
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
                var wishesCount = await _destinationInteractionsService.UnwishDestinationAsync(destinationId, userId);

                return Ok(new { wishesCount = wishesCount });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("favorite")]
        public async Task<IActionResult> FavoriteDestination(string destinationId)
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
                var favoritesCount = await _destinationInteractionsService.FavoriteDestinationAsync(destinationId, userId);

                return Ok(new { favoritesCount = favoritesCount });

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("favorite")]
        public async Task<IActionResult> UnfavoriteDestination(string destinationId)
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
                var favoritesCount = await _destinationInteractionsService.UnfavoriteDestinationAsync(destinationId, userId);

                return Ok(new { favoritesCount = favoritesCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
