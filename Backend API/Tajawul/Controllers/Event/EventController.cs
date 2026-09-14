using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.Events;
using Tajawul.Interfaces.General;
using Tajawul.Mappers;
using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs.Destination;
using Tajawul.Models.DTOs.Event;
using Tajawul.Models.DTOs.UploadService;
using Tajawul.Services.Destinations;
using static Tajawul.Helpers.GraphRelations;

namespace Tajawul.Controllers.Event
{
    [ApiController]
    [Route("api/[controller]/")]
    //[EnableRateLimiting("fixed")]
    public class EventController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IPriceRangeService _priceRangeService;
        private readonly IStatusService _statusService;
        private readonly IEventDateRangeService _eventDateRangeService;
        private readonly IDLocationService _dLocationService;
        private readonly ITagService _tagService;

        public EventController(
        IEventService eventService,
        IPriceRangeService priceRangeService,
        IStatusService statusService,
        IEventDateRangeService eventDateRangeService,
        ITagService tagService,
        IDLocationService dLocationService)
        {
            _eventService = eventService;
            _priceRangeService = priceRangeService;
            _statusService = statusService;
            _eventDateRangeService = eventDateRangeService;
            _dLocationService = dLocationService;
            _tagService = tagService;
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto, [FromQuery] string destinationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(destinationId))
            {
                return BadRequest(new { message = "Destination ID is required." });
            }

            try
            {
                var (eventEntity, failures) = await _eventService.CreateEventAsync(createEventDto, destinationId);

                var response = new
                {
                    Event = eventEntity,
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

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut]
        public async Task<IActionResult> UpdateEventAsync(UpdateEventDto eventDto, string destinationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(destinationId))
            {
                return BadRequest(new { message = "Destination ID is required." });
            }

            try
            {
                var (eventEntity, failures) = await _eventService.UpdateEventAsync(eventDto, destinationId);

                var response = new
                {
                    Event = eventEntity,
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
                return StatusCode(500, new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetEvents([FromQuery] EventFilter eventFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var events = await _eventService.GetEventsAsync(eventFilter);

                var eventDtos = events.Select(e => e.ToEventDto()).ToList();

                return Ok(new
                {
                    count = eventDtos.Count, events = eventDtos
                });
            }

            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{eventId}/users")]
        public async Task<IActionResult> GetEventUsers([FromQuery] EventUsersFilter usersFilter, string eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var users = await _eventService.GetEventUsersAsync(usersFilter, eventId);
                var count = users.Count;

                return Ok(new { count, relation = usersFilter.Relation, users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "PlaceOwner")]
        [HttpDelete("{eventId}")]
        public async Task<IActionResult> DeleteEventAsync(string eventId, string destinationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrWhiteSpace(destinationId))
            {
                return BadRequest(new { message = "Destination ID is required." });
            }

            try
            {
                var isDeleted = await _eventService.DeleteEventAsync(eventId, destinationId);
                return Ok(new { deleted = isDeleted });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("{eventId}/tags")]
        public async Task<IActionResult> AssignEventTags(string eventId, [FromBody] EventTagsDto eventTags, string destinationId)
        {
            if (eventTags == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return BadRequest(new { message = "Event ID is required" });
            }

            try
            {
                var tagTasks = eventTags.Tags?.Select(tag =>
                    _tagService.AssignEventTagAsync(tag, eventId, destinationId))
                    ?? new List<Task<Tag?>>();

                var tags = await Task.WhenAll(tagTasks);

                if (!tags.Any(t => t != null))
                {
                    return BadRequest(new { message = "No valid tags were provided." });
                }

                return Ok(new { tags });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("{eventId}/tags")]
        public async Task<IActionResult> RemoveEventTags(string eventId, [FromBody] EventTagsDto eventTags, string destinationId)
        {
            if (eventTags == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(eventId))
            {
                return Unauthorized(new { message = "Event not found" });
            }

            try
            {

                var tagTasks = eventTags.Tags?.Select(tag =>
                    _tagService.DeleteEventTagAsync(tag, eventId, destinationId)) ?? new List<Task<bool>>();

                var tagResults = await Task.WhenAll(tagTasks);

                
                bool anyDeleted = tagResults.Any(r => r);

                if (anyDeleted)
                {
                    return Ok(new { message = "Tag removed successfully." });
                }
                else
                {
                    return NotFound(new { message = "No matching Tag found to delete." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{eventId}/tags")]
        public async Task<IActionResult> GetEventATags(string eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tagTask = _tagService.GetEventTagsAsync(eventId);

                await Task.WhenAll(tagTask);

                var tags = tagTask.Result;

                return Ok(new
                {
                    tags = new { count = tags?.Count() ?? 0, data = tags }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut("{eventId}/coverImage")]
        public async Task<IActionResult> UpdateEventCoverImage([FromForm] ImageUploadDto imageDto, string eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found" });
            }

            try
            {
                var url = await _eventService.UpdateEventCoverImageAsync(imageDto.ProfileImage, eventId, userId);

                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image update failed: {ex.Message}");
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut("{eventId}/images")]
        public async Task<IActionResult> UpdateEventImages([FromForm] ImagesUploadDto imageFiles, string eventId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "A list of image URLs must be provided." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found" });
            }

            try
            {

                var result = await _eventService.UpdateEventImagesAsync(imageFiles.Images, eventId, userId);

                if (result.Failed.Any())
                    return StatusCode(207, result);

                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image update failed: {ex.Message}");
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("{eventId}/images")]
        public async Task<IActionResult> DeleteEventImages(List<string> imageUrls, string eventId)
        {
            if (!ModelState.IsValid || imageUrls == null || !imageUrls.Any())
            {
                return BadRequest(new { message = "A list of image URLs must be provided." });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found" });
            }

            try
            {
                var result = await _eventService.DeleteEventImagesAsync(imageUrls, eventId, userId);

                if (result.Failed.Any())
                {
                    return StatusCode(207, new
                    {
                        message = "Some images could not be deleted.",
                        deleted = result.Success,
                        failed = result.Failed
                    });
                }

                return Ok(new
                {
                    message = "All images deleted successfully.",
                    deleted = result.Success
                });
            }
            catch (ArgumentException argEx)
            {
                return BadRequest(new { message = argEx.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { ex.Message });
            }
        }
    }
}
