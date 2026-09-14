using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tajawul.Helpers.Filters.SearchBar;
using Tajawul.Interfaces.User.Interactions;
using Tajawul.Models.DTOs.user.UserInteractions;
using Tajawul.Models.ViewModels.user.UserInteractions;

namespace Tajawul.Controllers.User.Interactions
{
    [ApiController]
    [Route("api/user/")]
    //[EnableRateLimiting("fixed")]
    public class UserInteractionsController(ILogger<UserInteractionsController> logger, IUserInteractionsService userInteractionsService) : ControllerBase
    {
        private readonly ILogger<UserInteractionsController> logger = logger;
        private readonly IUserInteractionsService userInteractionsService = userInteractionsService;


        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("toggle-follow")]
        public async Task<ActionResult> ToggleFollowUserAsync(ToggleFollowDto toggleFollowDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }


            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            try
            {
                var result = await userInteractionsService.ToggleFollowUserAsync(toggleFollowDto.FollowedId, userId);
                if (result == null) return BadRequest(new { Message = "Failed to follow user" });
                return Ok(new { FollowersCount = result.FollowersCount, IsFollowing = result.IsFollowing, Message = "User toggled successfully" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error following user. User ID: {UserId}", userId);
                return StatusCode(500, new { message = "Failed to follow user" });
            }

        }

        [Authorize]
        [HttpGet("followers")]
        public async Task<IActionResult> GetFollowersAsync([FromQuery] UserFollowersFilter userFollowersFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }
            try
            {
                var followers = await userInteractionsService.GetFollowersAsync(userId, userFollowersFilter.PageNumber, userFollowersFilter.PageSize);
                if (followers == null) return BadRequest(new { Message = "Failed to get followers" });

                return Ok(followers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting followers of user {UserId}", userId);
                return StatusCode(500, new { Message = $"Error while getting followers" });
            }
        }

        [Authorize]
        [HttpGet("followings")]
        public async Task<IActionResult> GetFollowingsAsync([FromQuery] UserFollowersFilter userFollowersFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }
            try
            {
                var followers = await userInteractionsService.GetFollowingsAsync(userId, userFollowersFilter.PageNumber, userFollowersFilter.PageSize);
                if (followers == null) return BadRequest(new { Message = "Failed to get followings" });

                return Ok(followers);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while getting followings of user {UserId}", userId);
                return StatusCode(500, new { Message = $"Error while getting followings" });
            }
        }
    }
}
