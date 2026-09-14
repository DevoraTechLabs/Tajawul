using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tajawul.Helpers.Filters.SocialMedia;
using Tajawul.Interfaces.SocialMedia;
using Tajawul.Models.DTOs.Comment;

namespace Tajawul.Controllers.SocialMedia
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController(ICommentService commentService, ILogger<CommentController> logger) : ControllerBase
    {
        private readonly ICommentService _commentService = commentService;
        private readonly ILogger<CommentController> _logger = logger;

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }


        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost]
        public async Task<IActionResult> CreateComment(CreateCommentDto createCommentDto)
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
                var (comment, errors) = await _commentService.AddCommentAsync(createCommentDto, userId);
                if (comment == null)
                {
                    return BadRequest(new { Message = errors });
                }

                return CreatedAtAction(nameof(CreateComment), new { }, comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding comment. User ID: {UserId}", userId);
                return StatusCode(500, new { Message = "Failed to add comment" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut]
        public async Task<IActionResult> UpdateComment(UpdateCommentDto updateCommentDto)
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
                var (comment, errors) = await _commentService.UpdateCommentAsync(updateCommentDto, userId);
                if (comment == null)
                {
                    return BadRequest(new { Message = errors });
                }

                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating comment. User ID: {UserId}", userId);
                return StatusCode(500, new { Message = "Failed to update comment" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("reply")]
        public async Task<IActionResult> ReplyComment(ReplyCommentDto replyCommentDto)
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
                var (comment, errors) = await _commentService.ReplyCommentAsync(replyCommentDto, userId);
                if (comment == null)
                {
                    return BadRequest(new { Message = errors });
                }

                return CreatedAtAction(nameof(ReplyComment), new { }, comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error replying comment. User ID: {UserId}", userId);
                return StatusCode(500, new { Message = "Failed to reply comment" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete]
        public async Task<IActionResult> DeleteComment(DeleteCommentDto commentDto)
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
                var result = await _commentService.DeleteCommentAsync(commentDto, userId);
                if (result == false) return BadRequest(new { Message = "Failed to delete comment" });
                return Ok(new { Message = "Comment deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting comment. User ID: {UserId}", userId);
                return StatusCode(500, new { Message = "Failed to delete comment" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("toggle-upvote")]
        public async Task<IActionResult> ToggleUpvoteComment(ToggleVotingCommentDto upvoteCommentDto)
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
                var result = await _commentService.ToggleUpvoteCommentAsync(upvoteCommentDto, userId);
                if (result == null) return BadRequest(new { Message = "Failed to upvote comment" });
                return Ok(new
                {
                    Message = "Comment toggled successfully",
                    Upvotes = result.Upvotes,
                    Downvotes = result.Downvotes,
                    IsUpvotedByCurrentUser = result.IsUpvotedByCurrentUser,
                    IsDownvotedByCurrentUser = result.IsDownvotedByCurrentUser
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error upvoting comment. User ID: {UserId}", userId);
                return StatusCode(500, new { message = "Failed to upvote comment" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("toggle-downvote")]
        public async Task<IActionResult> ToggleDownvoteComment(ToggleVotingCommentDto upvoteCommentDto)
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
                var result = await _commentService.ToggleDownvoteCommentAsync(upvoteCommentDto, userId);
                if (result == null) return BadRequest(new { Message = "Failed to downvote comment" });
                return Ok(new
                {
                    Message = "Comment toggled successfully",
                    Upvotes = result.Upvotes,
                    Downvotes = result.Downvotes,
                    IsUpvotedByCurrentUser = result.IsUpvotedByCurrentUser,
                    IsDownvotedByCurrentUser = result.IsDownvotedByCurrentUser
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downvoting comment. User ID: {UserId}", userId);
                return StatusCode(500, new { message = "Failed to downvote comment" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpGet("replies")]
        public async Task<IActionResult> GetReplies([FromQuery] CommentRepliesFilter commentRepliesFilter)
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
                var replies = await _commentService.GetCommentWithRepliesAsync(commentRepliesFilter.CommentId, userId, commentRepliesFilter.PageNumber, commentRepliesFilter.PageSize);
                if (replies == null) return BadRequest(new { Message = "Failed to get replies" });

                return Ok(replies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting replies of comment {CommentId}. requested by user {UserId}", commentRepliesFilter.CommentId, userId);
                return StatusCode(500, new { Message = $"Error while getting replies" });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpGet("comments")]
        public async Task<IActionResult> GetComments([FromQuery] CommentsFilter commentFilter)
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
                var comments = await _commentService.GetPostCommentsAsync(commentFilter.PostId, userId, commentFilter.PageNumber, commentFilter.PageSize);
                if (comments == null) return BadRequest(new { Message = "Failed to get comments" });

                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting comments of post {PostId}. requested by user {UserId}", commentFilter.PostId, userId);
                return StatusCode(500, new { Message = $"Error while getting comments" });
            }
        }
    }
}
