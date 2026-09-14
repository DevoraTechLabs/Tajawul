using Microsoft.AspNetCore.Mvc;
using Tajawul.Interfaces.Destinations;
using Tajawul.Interfaces.General;

namespace Tajawul.Controllers.SearchBar
{
    [Route("api/")]
    [ApiController]
    public class SearchBarFiltersOptionsController(
        ITagService tagService,
        IActivityService activityService,
     ILogger<SearchBarFiltersOptionsController> logger,
      ITypeService typeService) : ControllerBase
    {
        private readonly ITagService _tagService = tagService;
        private readonly IActivityService _activityService = activityService;
        private readonly ITypeService _typeService = typeService;
        private readonly ILogger<SearchBarFiltersOptionsController> _logger = logger;

        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            try
            {
                var tags = await _tagService.GetAllTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting tags");
                return StatusCode(500, new { Message = $"Error while getting tags" });
            }
        }
                
        [HttpGet("destination-types")]
        public async Task<IActionResult> GetAllDestinationTypes()
        {
            try
            {
                var types = await _typeService.GetAllDestinationTypesAsync();
                return Ok(types);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting destination types");
                return StatusCode(500, new { Message = $"Error while getting destination types" });
            }
        }

        [HttpGet("activities")]
        public async Task<IActionResult> GetActivities()
        {
            try
            {
                var activities = await _activityService.GetAllActivitiesAsync();
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting activities");
                return StatusCode(500, new { Message = $"Error while getting activities" });
            }
        }
    }
}
