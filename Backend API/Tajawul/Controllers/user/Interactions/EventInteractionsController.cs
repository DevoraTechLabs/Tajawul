using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Tajawul.Interfaces.User.Interactions;

namespace Tajawul.Controllers.User.Interactions
{
    [ApiController]
    [Route("api/user/{eventId}")]
    //[EnableRateLimiting("fixed")]

    public class EventInteractionsController : ControllerBase
    {
        private readonly IEventInteractionsService _eventInteractionsService;

        public EventInteractionsController(IEventInteractionsService eventInteractionsService)
        {
            _eventInteractionsService = eventInteractionsService;
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("interested")]
        public async Task<IActionResult> InterestedInEvent(string eventId)
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
                var interestedInCount = await _eventInteractionsService.InterestedInEventAsync(eventId, userId);

                return Ok(new { interestedInCount = interestedInCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("interested")]
        public async Task<IActionResult> NotInterestedInEvent(string eventId)
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
                var interestedInCount = await _eventInteractionsService.NotInterestedInEventAsync(eventId, userId);

                return Ok(new { interestedInCount = interestedInCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("attend")]
        public async Task<IActionResult> AttendEvent(string eventId)
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
                var attendeesCount = await _eventInteractionsService.AttendEventAsync(eventId, userId);

                return Ok(new { attendeesCount = attendeesCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("attend")]
        public async Task<IActionResult> UnAttendEvent(string eventId)
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
                var attendeesCount = await _eventInteractionsService.UnAttendEventAsync(eventId, userId);

                return Ok(new { attendeesCount = attendeesCount });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
