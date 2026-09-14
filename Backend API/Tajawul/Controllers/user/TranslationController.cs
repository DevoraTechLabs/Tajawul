using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.User.Translation;
using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.DTOs.user.Translation;

namespace Tajawul.Controllers.User
{
    [ApiController]
    [Route("api/[controller]")]
    public class TranslationController(
        ITranslationService translationService,
        ILogger<TranslationController> logger) : ControllerBase
    {
        private readonly ITranslationService _translationService = translationService;
        private readonly ILogger<TranslationController> _logger = logger;

        private string? GetUserId() => User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        [HttpPost]
        [Authorize(Policy = "FullyRegisteredUser")]
        public async Task<IActionResult> AddTranslation([FromBody] TranslationItemDto translationItemDto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Unauthorized" });

            try
            {
                var translatedText = await _translationService.AddTranslationItemAsync(translationItemDto, userId);

                if (translatedText == null) return BadRequest(new { Message = "Failed to translate text" });

                return CreatedAtAction(nameof(GetUserHistory), new { }, translatedText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding translation item. User ID: {UserId}", userId);
                return Problem("An internal error occurred. Please try again later.");
            }
        }

        [HttpGet]
        [Authorize(Policy = "FullyRegisteredUser")]
        public async Task<IActionResult> GetUserHistory([FromQuery] TranslationFilter filter)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Unauthorized" });

            try
            {
                var history = await _translationService.GetHistoryAsync(userId, filter);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user history. User ID: {UserId}", userId);
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpGet("{translationItemId}")]
        [Authorize(Policy = "FullyRegisteredUser")]
        public async Task<IActionResult> GetTranslationItem([FromRoute] string translationItemId)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Unauthorized" });

            try
            {
                var translationItem = await _translationService.GetTranslationItemAsync(translationItemId, userId);

                return translationItem == null ? NotFound(new { Message = "Translation item not found" }) : Ok(translationItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting translation item. User ID: {UserId}", userId);
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpGet("favorites")]
        [Authorize(Policy = "FullyRegisteredUser")]
        public async Task<IActionResult> GetUserFavorites([FromQuery] TranslationFilter filter)
        {

            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Unauthorized" });

            try
            {
                var favorites = await _translationService.GetFavoritesAsync(userId, filter);
                return Ok(favorites);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user favorites. User ID: {UserId}", userId);
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpPatch("toggle-favorite")]
        [Authorize(Policy = "FullyRegisteredUser")]
        public async Task<IActionResult> MarkTranslationAsFavorite([FromBody] MarkAsFavoriteDto markAsFavoriteDto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Unauthorized" });

            try
            {
                var result = await _translationService.MarkAsFavoriteAsync(markAsFavoriteDto.TranslationItemId, userId);
                if (!result.Success) return BadRequest(new { Message = "Failed to Mark" });
                return Ok(new { Message = "Translation marked successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking translation . User ID: {UserId}", userId);
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpDelete]
        [Authorize(Policy = "FullyRegisteredUser")]
        public async Task<IActionResult> ClearUserHistory()
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Unauthorized" });

            try
            {
                var result = await _translationService.ClearTranslationHistoryAsync(userId);

                if (result == false) return BadRequest(new { Message = "Failed to clear history" });
                else if (result == null) return Ok(new { Message = "History already cleared" });
                return Ok(new { Message = "History cleared successfully" });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing user history. User ID: {UserId}", userId);
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpDelete("translation-item")]
        [Authorize(Policy = "FullyRegisteredUser")]
        public async Task<IActionResult> DeleteTranslationItem([FromBody] DeleteTranslationItemDto deleteTranslationItemDto)
        {
            var userId = GetUserId();
            if (string.IsNullOrEmpty(userId)) return Unauthorized(new { Message = "Unauthorized" });

            try
            {
                var result = await _translationService.DeleteTranslationItemAsync(deleteTranslationItemDto.TranslationItemId, userId);
                return result ? Ok(new { Message = "Translation deleted successfully" }) : NotFound(new { Message = "Translation item not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting translation. User ID: {UserId}", userId);
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }

        [HttpPost("translateText")]
        public async Task<IActionResult> TranslateText([FromBody] TranslationItemDto translateTextDto)
        {
            try
            {
                var result = await _translationService.TranslateTextAsync(translateTextDto);
                return result == null ? BadRequest(new { Message = "Translation failed" }) : Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal error occurred. Please try again later.");
            }
        }
    }
}
