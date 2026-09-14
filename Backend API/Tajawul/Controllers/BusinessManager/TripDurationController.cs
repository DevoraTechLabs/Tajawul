//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.RateLimiting;
//using System.Security.Claims;
//using Tajawul.Interfaces;
//using Tajawul.Models.DTOs;

//namespace Tajawul.Controllers.BusinessManager
//{
//    [Authorize]
//    [ApiController]
//    [Route("api/")]
//    [EnableRateLimiting("fixed")]

//    public class TripDurationController : ControllerBase
//    {
//        private readonly ILogger<TripDurationController> _logger;
//        private readonly ITripDurationService _tripDurationService;


//        public TripDurationController(ITripDurationService tripDurationService, ILogger<TripDurationController> logger)
//        {
//            _tripDurationService = tripDurationService ?? throw new ArgumentNullException(nameof(tripDurationService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }


//        // GET all
//        [HttpGet("tripDurations")]
//        public async Task<IActionResult> GetAllTripDurations()
//        {
//            try
//            {
//                var tripDurations = await _tripDurationService.GetAllTripDurationAsync();
//                return Ok(tripDurations);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all trip durations.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // GET by name
//        [HttpGet("tripDuration/{tripDurationName}")]
//        public async Task<IActionResult> GetTripDuration(string tripDurationName)
//        {
//            try
//            {
//                var tripDuration = await _tripDurationService.GetTripDurationByNameAsync(tripDurationName);
//                if (tripDuration == null)
//                {
//                    return NotFound(new { Message = "Trip duration not found" });
//                }
//                return Ok(tripDuration);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting trip duration by ID.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // CREATE
//        [HttpPost("businessManager/tripDuration")]
//        public async Task<IActionResult> CreateTripDuration([FromBody] TripDurationDto tripDurationDto)
//        {
//            try
//            {
//                var result = await _tripDurationService.CreateTripDurationAsync(tripDurationDto);
//                if (result == null)
//                    return BadRequest(new { Message = "Trip duration already exists" });

//                return StatusCode(201, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating trip duration.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // UPDATE
//        [HttpPut("businessManager/tripDuration/{oldTripDurationName}")]
//        public async Task<IActionResult> UpdateTripDurationName(string oldTripDurationName, [FromBody] TripDurationDto maritalStatusDto)
//        {
//            try
//            {
//                var tripDuration = await _tripDurationService.UpdateTripDurationNameAsync(oldTripDurationName, maritalStatusDto.Name);
//                if (tripDuration == null)
//                {
//                    return NotFound(new { Message = "Trip duration cannot be updated" });
//                }
//                return Ok(tripDuration);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating trip duration name.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // DELETE
//        [HttpDelete("businessManager/tripDuration/{tripDurationName}")]
//        public async Task<IActionResult> DeleteTripDuration(string tripDurationName)
//        {
//            try
//            {
//                var result = await _tripDurationService.DeleteTripDurationAsync(tripDurationName);
//                if (result == null)
//                    return NotFound(new { Message = "Trip duration not found" });

//                if (result == false)
//                    return BadRequest(new { Message = "Trip duration in use" });

//                return Ok(new { Message = "Trip duration deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting trip duration.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//    }
//}
