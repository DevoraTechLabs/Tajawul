//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.RateLimiting;
//using System.Security.Claims;
//using Tajawul.Models.DTOs;
//using Tajawul.Services;

//namespace Tajawul.Controllers.BusinessManager
//{
//    [Authorize] 
//    [ApiController]
//    [Route("api/")]
//    [EnableRateLimiting("fixed")]
//    public class InterestController : ControllerBase
//    {
//        private readonly IInterestService _interestService;
//        private readonly ILogger<InterestController> _logger;

//        public InterestController(IInterestService interestService, ILogger<InterestController> logger)
//        {
//            _interestService = interestService ?? throw new ArgumentNullException(nameof(interestService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }


//        // GET all
//        [HttpGet("interests")]
//        public async Task<IActionResult> GetAllInterests()
//        {
//            try
//            {
//                var interests = await _interestService.GetAllInterestsAsync();
//                return Ok(interests);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all interests.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // GET by Name
//        [HttpGet("interest/{interestName}")]
//        public async Task<IActionResult> GetInterestByName(string interestName)
//        {
//            try
//            {
//                var interest = await _interestService.GetInterestByNameAsync(interestName);
//                if (interest == null)
//                {
//                    return NotFound(new { Message = "Interest not found" });
//                }
//                return Ok(interest);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting interest by ID.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // CREATE
//        [HttpPost("businessManager/interest")]
//        public async Task<IActionResult> CreateInterest([FromBody] InterestDto interestDto)
//        {
//            try
//            {
//                var result = await _interestService.CreateInterestAsync(interestDto);
//                if (result == null)
//                {
//                    return BadRequest(new { Message = "Interest already exists" });
//                }

//                return StatusCode(201, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating interest.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // UPDATE
//        [HttpPut("businessManager/interest/{oldInterestName}")]
//        public async Task<IActionResult> UpdateInterestName(string InterestName, [FromBody] InterestDto interestDto)
//        {
//            try
//            {
//                var interest = await _interestService.UpdateInterestNameAsync(InterestName, interestDto.Name);
//                if (interest == null)
//                {
//                    return NotFound(new { Message = "Interest not found or new name already exists" });
//                }
//                return Ok(interest);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating interest name.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // DELETE
//        [HttpDelete("businessManager/interest/{InterestName}")]
//        public async Task<IActionResult> DeleteInterest(string InterestName)
//        {
//            try
//            {
//                var result = await _interestService.DeleteInterestAsync(InterestName);
//                if (result == null)
//                    return NotFound(new { Message = "Interest not found" });

//                if (result == false)
//                    return BadRequest(new { Message = "Interest in use" });

//                return Ok(new { Message = "Interest deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting interest.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//    }
//}
