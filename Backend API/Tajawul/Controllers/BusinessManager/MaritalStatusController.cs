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
//    public class MaritalStatusController : ControllerBase
//    {
//        private readonly IMaritalStatusService _maritalStatusService;
//        private readonly ILogger<MaritalStatusController> _logger;

//        public MaritalStatusController(IMaritalStatusService maritalStatusService, ILogger<MaritalStatusController> logger)
//        {
//            _maritalStatusService = maritalStatusService ?? throw new ArgumentNullException(nameof(maritalStatusService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }


//        // GET all
//        [HttpGet("maritalStatuses")]
//        public async Task<IActionResult> GetAllMaritalStatuses()
//        {
//            try
//            {
//                var maritalStatuses = await _maritalStatusService.GetAllMaritalStatusAsync();
//                return Ok(maritalStatuses);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all marital statuses.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // GET by Name
//        [HttpGet("maritalStatus/{maritalStatusName}")]
//        public async Task<IActionResult> GetMaritalStatusById(string maritalStatusName)
//        {
//            try
//            {
//                var maritalStatus = await _maritalStatusService.GetMaritalStatusByNameAsync(maritalStatusName);
//                if (maritalStatus == null)
//                {
//                    return NotFound(new { Message = "Marital status not found" });
//                }
//                return Ok(maritalStatus);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting marital status by ID.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // CREATE
//        [HttpPost("businessManager/maritalStatus")]
//        public async Task<IActionResult> CreateMaritalStatus([FromBody] MaritalStatusDto maritalStatusDto)
//        {
//            try
//            {
//                var result = await _maritalStatusService.CreateMaritalStatusAsync(maritalStatusDto);
//                if (result == null)
//                    return BadRequest(new { Message = "Marital status already exists" });

//                return StatusCode(201, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating marital status.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // UPDATE
//        [HttpPut("businessManager/maritalStatus/{oldMaritalStatusName}")]
//        public async Task<IActionResult> UpdateMaritalStatusName(string maritalStatusName, [FromBody] MaritalStatusDto maritalStatusDto)
//        {
//            try
//            {
//                var maritalStatus = await _maritalStatusService.UpdateMaritalStatusNameAsync(maritalStatusName, maritalStatusDto.Name);
//                if (maritalStatus == null)
//                {
//                    return NotFound(new { Message = "Marital status not found or new marital status name already exists" });
//                }
//                return Ok(maritalStatus);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating marital status name.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // DELETE
//        [HttpDelete("businessManager/maritalStatus/{maritalStatusName}")]
//        public async Task<IActionResult> DeleteMaritalStatus(string maritalStatusName)
//        {
//            try
//            {
//                var result = await _maritalStatusService.DeleteMaritalStatusAsync(maritalStatusName);
//                if (result == null)
//                    return NotFound(new { Message = "Marital status not found" });

//                if (result == false)
//                    return BadRequest(new { Message = "Marital status in use" });

//                return Ok(new { Message = "Marital status deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting marital status.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//    }
//}
