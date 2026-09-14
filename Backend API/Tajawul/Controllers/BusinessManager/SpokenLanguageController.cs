//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.RateLimiting;
//using System.Security.Claims;
//using Tajawul.Interfaces;
//using Tajawul.Models.Domain;
//using Tajawul.Models.DTOs;

//namespace Tajawul.Controllers.BusinessManager
//{
//    [Authorize]
//    [ApiController]
//    [Route("api/")]
//    [EnableRateLimiting("fixed")]
//    public class SpokenLanguageController : ControllerBase
//    {
//        private readonly ISpokenLanguageService _spokenLanguageService;
//        private readonly ILogger<SpokenLanguageController> _logger;

//        public SpokenLanguageController(ISpokenLanguageService spokenLanguageService, ILogger<SpokenLanguageController> logger)
//        {
//            _spokenLanguageService = spokenLanguageService ?? throw new ArgumentNullException(nameof(spokenLanguageService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        // GET all
//        [HttpGet("spokenLanguages")]
//        public async Task<IActionResult> GetAllSpokenLanguages()
//        {
//            try
//            {
//                var spokenLanguages = await _spokenLanguageService.GetAllSpokenLanguagesAsync();
//                return Ok(spokenLanguages);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all spoken languages.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // GET by Name
//        [HttpGet("spokenLanguage/{spokenLanguageName}")]
//        public async Task<IActionResult> GetSpokenLanguageById(string spokenLanguageName)
//        {
//            try
//            {
//                var spokenLanguage = await _spokenLanguageService.GetSpokenLanguageByNameAsync(spokenLanguageName);
//                if (spokenLanguage == null)
//                {
//                    return NotFound(new { Message = "Spoken language not found" });
//                }
//                return Ok(spokenLanguage);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting spoken language by ID.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // CREATE
//        [HttpPost("businessManager/spokenLanguage")]
//        public async Task<IActionResult> CreateSpokenLanguage([FromBody] SpokenLanguageDto spokenLanguage)
//        {
//            try
//            {
//                var result = await _spokenLanguageService.CreateSpokenLanguageAsync(spokenLanguage);

//                return StatusCode(201, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating spoken language.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // UPDATE
//        [HttpPut("businessManager/spokenLanguage/{oldSpokenLanguageName}")]
//        public async Task<IActionResult> UpdateSpokenLanguageName(string spokenLanguageName, [FromBody] SpokenLanguageDto spokenLanguageDto)
//        {
//            try
//            {
//                var spokenLanguage = await _spokenLanguageService.UpdateSpokenLanguageNameAsync(spokenLanguageName, spokenLanguageDto.Name);
//                if (spokenLanguage == null)
//                {
//                    return BadRequest(new { Message = "Spoken language not found or new spoken language name already exists" });
//                }
//                return Ok(spokenLanguage);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating spoken language name.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // DELETE
//        [HttpDelete("businessManager/spokenLanguage/{spokenLanguageName}")]
//        public async Task<IActionResult> DeleteSpokenLanguage(string spokenLanguageName)
//        {
//            try
//            {
//                var result = await _spokenLanguageService.DeleteSpokenLanguageAsync(spokenLanguageName);
//                if (result == null)
//                    return NotFound(new { Message = "Spoken language not found" });

//                if (result == false)
//                    return BadRequest(new { Message = "Spoken language in use" });

//                return Ok(new { Message = "Spoken language deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting spoken language.");
//                return StatusCode(500, "Internal server error");
//            }
//        }


//    }
//}
