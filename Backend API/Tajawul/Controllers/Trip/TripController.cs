using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using Tajawul.Helpers.Filters;
using Tajawul.Interfaces.Trips;
using Tajawul.Mappers;
using Tajawul.Models.DTOs.Trip;
using Tajawul.Models.DTOs.UploadService;
using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.General;

namespace Tajawul.Controllers.Trip
{

    [ApiController]
    [Route("api/[controller]/")]
    //[EnableRateLimiting("fixed")]
    public class TripController : ControllerBase
    {
        private readonly ITripService _tripService;
        private readonly ITagService _tagService;
        private readonly IStatusService _statusService;
        private readonly IPriceRangeService _priceRangeService;
        private readonly ITripDurationService _tripDurationService;
        private readonly IVisibilityService _visibilityService;


        public TripController(ITripService tripService,
            ITagService tagService, IStatusService statusService,
            IPriceRangeService priceRangeService, ITripDurationService tripDurationService,
            IVisibilityService visibilityService)
        {
            _tripService = tripService;
            _tagService = tagService;
            _statusService = statusService;
            _priceRangeService = priceRangeService;
            _tripDurationService = tripDurationService;
            _visibilityService = visibilityService;
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripDto createTripDto)
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
                var (trip, failures) = await _tripService.CreateTripAsync(createTripDto, userId);

                var response = new
                {
                    Trip = trip.ToTripDto(), 
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
        public async Task<IActionResult> UpdateTrip([FromBody] UpdateTripDto updateTripDto)
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
                var (trip, failures) = await _tripService.UpdateTripAsync(updateTripDto, userId);

                var response = new
                {
                    Trip = trip.ToTripDto(), 
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
        [HttpDelete("{tripId}")]
        public async Task<IActionResult> DeleteTrip(string tripId)
        {
            if (string.IsNullOrEmpty(tripId))
            {
                return BadRequest(new { message = "Trip ID is required" });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { message = "User not found" });

            try
            {
                var success = await _tripService.DeleteTripAsync(userId, tripId);

                if (!success) 
                {
                    return NotFound(new { message = "Trip not found or no permission" });
                }

                return NoContent(); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("{tripId}/destination/{destinationId}")]
        public async Task<IActionResult> AssignDestinationToTrip(string tripId, string destinationId, [FromBody] TripDestinationDto tripDestinationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ModelState.IsValid || tripDestinationDto.Day <= 0)
            {
                return BadRequest();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found" });
            }
            try
            {
                var tripDestinations = await _tripService.AssignDestinationToTripAsync(userId, tripId, destinationId, tripDestinationDto);

                //var destinationsDtos = tripDestinations.Select(d => d.ToTripDestinationDto()).ToList();

                var count = tripDestinations.Count;

                return Ok(new { count = count, destinations = tripDestinations });
            }
            catch (Exception ex)
            {
                // Log error message and return appropriate status codes
                string errorMessage = ex.Message.ToLower();

                if (errorMessage.Contains("trip") && errorMessage.Contains("not found"))
                {
                    return NotFound(new { message = "Trip not found." });
                }
                if (errorMessage.Contains("destination") && errorMessage.Contains("not found"))
                {
                    return NotFound(new { message = "Destination not found." });
                }
                if (errorMessage.Contains("conflict"))
                {
                    return Conflict(new { message = "Another destination is already assigned to this day." });
                }

                // Return detailed error for debugging
                return StatusCode(500, new
                {
                    message = ex.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpDelete("{tripId}/destination/{destinationId}")]
        public async Task<IActionResult> RemoveDestinationFromTrip(string tripId, string destinationId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized(new { message = "User not found" });
            }

            try
            {
                bool removed = await _tripService.RemoveDestinationFromTripAsync(userId, tripId, destinationId);

                if (!removed)
                {
                    return NotFound(new { message = "Destination not found in this trip or unauthorized action." });
                }

                return Ok(new { message = "Destination removed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{tripId}/destinations")]
        public async Task<IActionResult> GetTripDestinations(string tripId)
        {
            var destinations = await _tripService.GetTripDestinationsAsync(tripId);

            if (destinations == null || !destinations.Any())
            {
                return NotFound(new { message = "No destinations found for this trip." });
            }

            return Ok(destinations);
        }

        [HttpGet]
        public async Task<IActionResult> GetTripsAsync([FromQuery] TripFilter tripFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var trips = await _tripService.GetTripsAsync(tripFilter);

                var tripDtos = trips.Select(t => t.ToTripDto()).ToList();

                var count = tripDtos.Count;

                return Ok(new { count = count, trips = tripDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("tripHub")]
        public async Task<IActionResult> GetTripHubAsync([FromQuery] TripHubFilter tripHubFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var tripFilter = tripHubFilter.ToTripFilter();

                tripFilter.Visibility = "TripHub";

                var trips = await _tripService.GetTripsAsync(tripFilter);

                var tripDtos = trips.Select(t => t.ToTripDto()).ToList();

                var count = tripDtos.Count;

                return Ok(new { count = count, trips = tripDtos });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPost("Clone")]
        public async Task<IActionResult> CloneTrip([FromBody] CloneTripDto tripDto)
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
                var (trip, failures) = await _tripService.CloneTripAsync(tripDto.TripId, userId);

                var response = new
                {
                    Trip = trip.ToTripDto(),
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
        [HttpPost("{tripId}/tags")]
        public async Task<IActionResult> AssignTripTags(string tripId, [FromBody] TripTagsDto tripTags)
        {
            if (tripTags == null || !ModelState.IsValid)
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
                var tagTasks = tripTags.Tags?.Select(tag =>
                    _tagService.AssignTripTagAsync(tag, tripId, userId))
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
        [HttpDelete("{tripId}/tags")]
        public async Task<IActionResult> RemoveTripTags(string tripId, [FromBody] TripTagsDto tripTags)
        {
            if (tripTags == null || !ModelState.IsValid)
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
                var tagTasks = tripTags.Tags?.Select(tag =>
                    _tagService.DeleteTripTagAsync(tag, tripId, userId)) ?? new List<Task<bool>>();

                var tagResults = await Task.WhenAll(tagTasks);

                bool anyDeleted = tagResults.Any(r => r);

                if (anyDeleted)
                {
                    return Ok(new { message = "Tag(s) removed successfully." });
                }
                else
                {
                    return NotFound(new { message = "No matching tags found to delete." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{tripId}/tags")]
        public async Task<IActionResult> GetTripTags(string tripId)
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
                var tags = await _tagService.GetTripTagsAsync(tripId, userId);

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

        [HttpGet("{tripId}/users")]
        public async Task<IActionResult> GetTripUsers([FromQuery] TripUserFilter usersFilter, string tripId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var users = await _tripService.GetTripUsersAsync(usersFilter, tripId);
                var count = users.Count;

                return Ok(new { count, usersFilter.Relation, users });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }

        }

        [Authorize(Policy = "FullyRegisteredUser")]
        [HttpPut("{tripId}/coverImage")]
        public async Task<IActionResult> UpdateTripCoverImage([FromForm] ImageUploadDto imageDto, string tripId)
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
                var url = await _tripService.UpdateTripCoverImageAsync(imageDto.ProfileImage, tripId, userId);

                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image update failed: {ex.Message}");
            }
        }
    }
}