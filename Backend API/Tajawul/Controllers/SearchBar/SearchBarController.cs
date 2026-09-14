using Microsoft.AspNetCore.Mvc;
using Tajawul.Helpers.Filters.SearchBar;
using Tajawul.Interfaces;

namespace Tajawul.Controllers
{
    [ApiController]
    [Route("api/Search")]
    public class SearchBarController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ILogger<SearchBarController> _logger;

        public SearchBarController(ISearchService searchService, ILogger<SearchBarController> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GeneralSearch([FromQuery] string query, [FromQuery] string mode = "wildcard")
        {
            if (string.IsNullOrEmpty(query)) return BadRequest("Query is required.");

            try
            {
                var results = await _searchService.GeneralSearchAsync(query, mode);
                return Ok(results);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error in general search");
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }

        [HttpGet("user")]
        public async Task<IActionResult> UserSearch([FromQuery] string? query, [FromQuery] string mode = "wildcard")
        {
            try
            {
                var results = await _searchService.SearchUsersAsync(query, mode, 15);
                return Ok(new { users = results });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error in user search");
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }

        [HttpGet("destination")]
        public async Task<IActionResult> SearchDestinations(
            [FromQuery] string? query,
            [FromQuery] SearchBarDestinationFilter? filters,
            [FromQuery] string mode = "wildcard")
        {
            try
            {
                var results = await _searchService.SearchDestinationsAsync(query, filters, mode, 15);
                return Ok(new { destinations = results });
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, "Error in destination search");
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }

        [HttpGet("trip")]
        public async Task<IActionResult> SearchTrips(
            [FromQuery] string? query,
            [FromQuery] SearchBarTripFilter? filters,
            [FromQuery] string mode = "wildcard")
        {
            try
            {
                var results = await _searchService.SearchTripsAsync(query, filters, mode, 15);
                return Ok(new { trips = results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in trip search");
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }

        [HttpGet("event")]
        public async Task<IActionResult> SearchEvents(
            [FromQuery] string? query,
            [FromQuery] SearchBarEventFilter? filters,
            [FromQuery] string mode = "wildcard")
        {
            try
            {
                var results = await _searchService.SearchEventsAsync(query, filters, mode, 15);
                return Ok(new { events = results });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event search");
                return StatusCode(500, new { Message = "An error occurred" });
            }
        }
    }
}