using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Tajawul.Helpers.Filters;
using Tajawul.Services.Weather;

namespace Tajawul.Controllers.ExternalApis
{
    [ApiController]
    [Route("api/[controller]/")]
    //[EnableRateLimiting("fixed")]
    public class WeatherController : ControllerBase
    {

        private readonly VCWeatherService _weatherService;

        public WeatherController(VCWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCityWeather([FromQuery] string city)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var weather = await _weatherService.GetCurrentCityWeather(city);

                return Ok(weather);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
