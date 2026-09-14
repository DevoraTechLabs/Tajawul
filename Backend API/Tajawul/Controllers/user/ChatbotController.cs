using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Tajawul.Interfaces.User.ChatBot;
using Tajawul.Models.DTOs.user.Chatbot;

namespace Tajawul.Controllers.User
{
    [ApiController]
    [Authorize]
    [Route("api/Chats")]
    public class ChatbotController(
        IChatbotService chatbotService,
        ILogger<ChatbotController> logger) : ControllerBase
    {
        private readonly IChatbotService _chatbotService = chatbotService;
        private readonly ILogger<ChatbotController> _logger = logger;


        [HttpPost]
        public async Task<IActionResult> CreateChat([FromBody] CreateChatbotChatDto createChatbotChatDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Get the user's ID from the claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //return Ok(userId);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID not found");
            }
            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                // Create a new chat with the authenticated user's ID
                var newChatDto = await _chatbotService.CreateChatAsync(createChatbotChatDto.Prompt, token, userId);
                if (newChatDto == null) return BadRequest(new { Message = "Failed to create chat" });
                return Ok(newChatDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating chat. User ID: {UserId}", userId);
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpGet("{chatId}")]
        public async Task<IActionResult> GetChat([FromRoute] string chatId)
        {

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not found");

            // Validate chatId
            if (!MongoDB.Bson.ObjectId.TryParse(chatId, out _))
            {
                return NotFound(new { Message = "Chat not found" });
            }

            try
            {
                var chatDto = await _chatbotService.GetChatAsync(chatId, userId);

                if (chatDto == null) return NotFound(new { Message = "Chat not found" });

                return Ok(chatDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting chat Id {chatId}. User ID: {userId}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpPost("prompt")]
        public async Task<IActionResult> SendPrompt(SendPromptDto sendPromptDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not found");

            try
            {
                var token = HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");
                var sentMessageDto = await _chatbotService.SendPromptAsync(sendPromptDto.ChatId, userId, sendPromptDto.Prompt, token);

                if (sentMessageDto == null) return BadRequest();

                return Ok(sentMessageDto);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending prompt to chat Id {sendPromptDto.ChatId}. User ID: {userId}");
                return StatusCode(500, "An error occurred");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteChat(DeleteChatbotChatDto deleteChatbotChatDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not found");

            try
            {
                var isDeleted = await _chatbotService.DeleteChatAsync(deleteChatbotChatDto.ChatId, userId);
                if (!isDeleted) return NotFound();

                return Ok(new { Message = "Chat deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting chat Id {deleteChatbotChatDto.ChatId}. User ID: {userId}");
                return StatusCode(500, "An error occurred");
            }

        }

        [HttpGet]
        public async Task<IActionResult> GetAllUserChats()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId)) return Unauthorized("User not found");

            try
            {
                var chatDtos = await _chatbotService.GetAllUserChatsAsync(userId);
                return Ok(chatDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting all chats for user. User ID: {userId}");
                return StatusCode(500, "An error occurred");
            }
        }

    }
}
