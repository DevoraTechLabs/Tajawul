using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tajawul.Helpers.Filters.SocialMedia;
using Tajawul.Interfaces.SocialMedia;
using Tajawul.Models.DTOs.Comment;
using Tajawul.Models.DTOs.SocialMedia.Posts;
using Tajawul.Services.SocialMedia;

namespace Tajawul.Controllers.SocialMedia.Posts
{
    [ApiController]
    [Route("api/[controller]/")]
    //[EnableRateLimiting("fixed")]
    public class PostsController : ControllerBase
    {
        private readonly IPostsService _postsService;
        

        public PostsController(IPostsService postsService)
        {
            _postsService = postsService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost]
        public async Task<IActionResult> CreatePost([FromForm] CreatePostDto createPostDto)
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
                var (post, failures) = await _postsService.CreatePostAsync(createPostDto, userId);

                var response = new
                {
                    Post = post,
                    Failures = failures
                };

                if (failures.Count > 0)
                {
                    return StatusCode(207, response);
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserPosts()
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
                var posts = await _postsService.GetUserPostsAsync(userId);

                var count = posts.Count;

                return Ok(new { count = count, posts = posts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("feed")]
        public async Task<IActionResult> GetUserFeed([FromQuery] PostsFilter postsFilter)
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
                var posts = await _postsService.GetUserFeedAsync(postsFilter);

                var count = posts.Count;

                return Ok(new { count = count, posts = posts });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpDelete("{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
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
                var result = await _postsService.DeleteUserPostAsync(postId, userId);

                if (result == true) return Ok(new { message = "Post deleted succefully." });
                else return NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("toggle-upvote")]
        public async Task<IActionResult> ToggleUpvoteComment(ToggleVotingPostDto upvotePostDto)
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
                var result = await _postsService.ToggleUpvotePostAsync(upvotePostDto, userId);
                if (result == null) return BadRequest(new { Message = "Failed to upvote post" });
                return Ok(new
                {
                    Message = "Post toggled successfully",
                    Upvotes = result.Upvotes,
                    Downvotes = result.Downvotes,
                    IsUpvotedByCurrentUser = result.IsUpvotedByCurrentUser,
                    IsDownvotedByCurrentUser = result.IsDownvotedByCurrentUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to upvote post" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("toggle-downvote")]
        public async Task<IActionResult> ToggleDownvoteComment(ToggleVotingPostDto upvotePostDto)
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
                var result = await _postsService.ToggleDownvotePostAsync(upvotePostDto, userId);
                if (result == null) return BadRequest(new { Message = "Failed to downvote post" });
                return Ok(new
                {
                    Message = "Post toggled successfully",
                    Upvotes = result.Upvotes,
                    Downvotes = result.Downvotes,
                    IsUpvotedByCurrentUser = result.IsUpvotedByCurrentUser,
                    IsDownvotedByCurrentUser = result.IsDownvotedByCurrentUser
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to downvote post" });
            }
        }

    }
}
