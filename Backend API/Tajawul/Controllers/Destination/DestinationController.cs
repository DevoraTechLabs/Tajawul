using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.Destinations;
using Tajawul.Interfaces.General;
using Tajawul.Mappers;
using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs.Destination;
using Tajawul.Models.DTOs.UploadService;

namespace Tajawul.Controllers.Destination
{
    [ApiController]
    [Route("api/[controller]/")]
    //[EnableRateLimiting("fixed")]
    public class DestinationController : ControllerBase
    {
        private readonly IDestinationService _destinationService;
        private readonly IActivityService _activityService;
        private readonly IGroupSizeService _groupSizeService;
        private readonly IPriceRangeService _priceRangeService;
        private readonly ITagService _tagService;
        private readonly ITypeService _typeService;
        private readonly IOpenCloseService _openCloseService;
        private readonly IDLocationService _dLocationService;

        public DestinationController(IDestinationService destinationService, IActivityService activityService,
            IGroupSizeService groupSizeService, IPriceRangeService priceRangeService,
            ITagService tagService, ITypeService typeService, IOpenCloseService openCloseService,
            IDLocationService dLocationService)
        {
            _destinationService = destinationService;
            _activityService = activityService;
            _groupSizeService = groupSizeService;
            _priceRangeService = priceRangeService;
            _tagService = tagService;
            _typeService = typeService;
            _openCloseService = openCloseService;
            _dLocationService = dLocationService;

        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost]
        public async Task<IActionResult> CreateDestination(CreateDestinationDto createDestinationDto)
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
                var (destination, failures) = await _destinationService.CreateDestinationAsync(createDestinationDto, userId);

                var response = new
                {
                    Destination = destination.ToDestinationDto(),
                    Failures = failures
                };

                if (failures.Count > 0)
                {
                    return StatusCode(207, response); // 207: Multi-Status (indicates partial success)
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetDestinations([FromQuery] DestinationFilter destinationFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var destinations = await _destinationService.GetDestinationsAsync(destinationFilter);

                var destinationsDtos = destinations.Select(d => d.ToDestinationDto()).ToList();

                var count  = destinationsDtos.Count;

                return Ok(new { count = count, destinations = destinationsDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut]
        public async Task<IActionResult> UpdateDestination(UpdateDestinationDto updateDestinationDto)
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
                var (destination, failures) = await _destinationService.UpdateDestinationAsync(updateDestinationDto, userId);

                var response = new
                {
                    Destination = destination.ToDestinationDto(),
                    Failures = failures
                };

                if (failures.Count > 0)
                {
                    return StatusCode(207, response); // 207: Multi-Status (indicates partial success)
                }

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("{destinationId}/attributes")]
        public async Task<IActionResult> Add(string destinationId, [FromBody] DestinationAttributesDto destinationAttributes)
        {
            if (destinationAttributes == null || !ModelState.IsValid)
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
                var activityTasks = destinationAttributes.Activities?.Select(activity =>
                    _activityService.AssignActivityAsync(activity, destinationId, userId)) ?? new List<Task<Activity?>>();

                var groupSizeTasks = destinationAttributes.GroupSizes?.Select(groupSize =>
                    _groupSizeService.AssignGroupSizeAsync(groupSize, destinationId, userId)) ?? new List<Task<GroupSize?>>();

                var tagTasks = destinationAttributes.Tags?.Select(tag =>
                    _tagService.AssignDestinationTagAsync(tag, destinationId, userId)) ?? new List<Task<Tag?>>();

                // Run tasks concurrently
                var activities = await Task.WhenAll(activityTasks);
                var groupSizes = await Task.WhenAll(groupSizeTasks);
                var tags = await Task.WhenAll(tagTasks);

                // Ensure at least one attribute was assigned
                if (!activities.Any(a => a != null) && !groupSizes.Any(g => g != null) && !tags.Any(t => t != null))
                {
                    return BadRequest(new { message = "No valid attributes were provided." });
                }

                return Ok(new { activities, groupSizes, tags });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("{destinationId}/attributes")]
        public async Task<IActionResult> Remove(string destinationId, [FromBody] DestinationAttributesDto destinationAttributes)
        {
            if (destinationAttributes == null || !ModelState.IsValid)
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
                // Create deletion tasks for each attribute type
                var activityTasks = destinationAttributes.Activities?.Select(activity =>
                    _activityService.DeleteDestinationActivityAsync(activity, destinationId, userId)) ?? new List<Task<bool>>();

                var groupSizeTasks = destinationAttributes.GroupSizes?.Select(groupSize =>
                    _groupSizeService.RemoveDestinationGroupSizeAsync(groupSize, destinationId, userId)) ?? new List<Task<bool>>();

                var tagTasks = destinationAttributes.Tags?.Select(tag =>
                    _tagService.DeleteDestinationTagAsync(tag, destinationId, userId)) ?? new List<Task<bool>>();

                // Execute tasks in parallel
                var activityResults = await Task.WhenAll(activityTasks);
                var groupSizeResults = await Task.WhenAll(groupSizeTasks);
                var tagResults = await Task.WhenAll(tagTasks);

                // Check if any deletions were successful
                bool anyDeleted = activityResults.Any(r => r) || groupSizeResults.Any(r => r) || tagResults.Any(r => r);

                if (anyDeleted)
                {
                    return Ok(new { message = "Attributes removed successfully." });
                }
                else
                {
                    return NotFound(new { message = "No matching attributes found to delete." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{destinationId}/attributes")]
        public async Task<IActionResult> GetDestinationAttributes(string destinationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Run all tasks in parallel
                var activityTask = _activityService.GetDestinationActivitiesAsync(destinationId);
                var groupSizeTask = _groupSizeService.GetDestinationGroupSizesAsync(destinationId);
                var tagTask = _tagService.GetDestinationTagsAsync(destinationId);

                await Task.WhenAll(activityTask, groupSizeTask, tagTask);

                var activities = activityTask.Result;
                var groupSizes = groupSizeTask.Result;
                var tags = tagTask.Result;

                return Ok(new
                {
                    activities = new { count = activities?.Count() ?? 0, data = activities },
                    groupSizes = new { count = groupSizes?.Count() ?? 0, data = groupSizes },
                    tags = new { count = tags?.Count() ?? 0, data = tags }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{destinationId}/users")]
        public async Task<IActionResult> GetDestinationUsers([FromQuery] DestinationUsersFilter usersFilter, string destinationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {

                var users = await _destinationService.GetDestinationUsersAsync(usersFilter, destinationId);
                var count = users.Count;
                

                return Ok(new { count, usersFilter.Relation, users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut("{destinationId}/coverImage")]
        public async Task<IActionResult> UpdateDestinationCoverImage([FromForm] ImageUploadDto imageDto, string destinationId)
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
                var url = await _destinationService.UpdateDestinationCoverImageAsync(imageDto.ProfileImage, destinationId, userId);

                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image update failed: {ex.Message}");
            }
        }
        
        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut("{destinationId}/images")]
        public async Task<IActionResult> UpdateDestinationImages([FromForm] ImagesUploadDto imageFiles, string destinationId)
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

                var result = await _destinationService.UpdateDestinationImagesAsync(imageFiles.Images, destinationId, userId);

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
        [HttpDelete("{destinationId}/images")]
        public async Task<IActionResult> DeleteDestinationImages(List<string> imageUrls, string destinationId)
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
                var result = await _destinationService.DeleteDestinationImagesAsync(imageUrls, destinationId, userId);

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
