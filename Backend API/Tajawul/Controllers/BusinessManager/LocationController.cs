//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Tajawul.Interfaces;
//using Tajawul.Models.DTOs.LocationDtos;
//using Microsoft.AspNetCore.RateLimiting;

//namespace Tajawul.Controllers.BusinessManager
//{
//    [Authorize]
//    [ApiController]
//    [Route("api/")]
//    [EnableRateLimiting("fixed")]
//    public class LocationController : ControllerBase
//    {
//        private readonly ILocationService _locationService;
//        private readonly ILogger<LocationController> _logger;

//        public LocationController(ILocationService locationService, ILogger<LocationController> logger)
//        {
//            _locationService = locationService ?? throw new ArgumentNullException(nameof(locationService));
//            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
//        }

//        [HttpGet("location/countries")]
//        public async Task<IActionResult> GetAllCountries()
//        {
//            try
//            {
//                var countries = await _locationService.GetAllCountriesAsync();
//                return Ok(countries);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all countries.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpGet("location/cities")]
//        public async Task<IActionResult> GetAllCities()
//        {
//            try
//            {
//                var cities = await _locationService.GetAllCitiesAsync();
//                return Ok(cities);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting all cities.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // GET Actions
//        [HttpGet("location/country/{CountryName}")]
//        public async Task<IActionResult> GetCountry(string CountryName)
//        {
//            try
//            {
//                var country = await _locationService.GetCountryByNameAsync(CountryName);
//                if (country == null)
//                {
//                    return NotFound(new { Message = "Country not found" });
//                }
//                return Ok(country);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting Country by ID.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpGet("location/city/{countryName}/{cityName}")]
//        public async Task<IActionResult> GetCity(string cityName, string countryName)
//        {
//            try
//            {
//                var city = await _locationService.GetCityByNameAsync(cityName, countryName);
//                if (city == null)
//                {
//                    return NotFound(new { Message = "City not found" });
//                }
//                return Ok(city);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error getting city by ID.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // CREATE Actions
//        [HttpPost("businessManager/Location/country")]
//        public async Task<IActionResult> CreateCountry([FromBody] CountryDto country)
//        {
//            try
//            {
//                var result = await _locationService.CreateCountryAsync(country);
//                return StatusCode(201, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating Country.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpPost("businessManager/Location/city")]
//        public async Task<IActionResult> CreateCity([FromBody] CityDto city)
//        {
//            try
//            {
//                var result = await _locationService.CreateCityAsync(city);
//                return StatusCode(201, result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating city.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        // UPDATE Actions
//        [HttpPut("businessManager/Location/country/{oldCountryName}")]
//        public async Task<IActionResult> UpdateCountryName(string CountryName, [FromBody] CountryDto countryDto)
//        {
//            try
//            {
//                var updatedName = await _locationService.UpdateCountryNameAsync(CountryName, countryDto.Name);
//                if (updatedName == null)
//                {
//                    return NotFound(new { Message = "Country not found or new country name already exists" });
//                }
//                return Ok(new { newName = updatedName });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating Country name.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpPut("businessManager/Location/city/{countryName}/{oldCityName}")]
//        public async Task<IActionResult> UpdateCityName(string cityOldName, string countryName, [FromBody] UpdateCityDto updateCityDto)
//        {
//            try
//            {
//                var city = await _locationService.UpdateCityNameAsync(cityOldName, updateCityDto.Name, countryName);
//                if (city == null)
//                {
//                    return NotFound(new { Message = "City not found or new city name already exists" });
//                }
//                return Ok(city);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating city name.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        //// DELETE Actions
//        //[HttpDelete("Country/{CountryName}")]
//        //public async Task<IActionResult> DeleteCountry(string CountryName)
//        //{
//        //    try
//        //    {
//        //        var result = await _locationService.DeleteCountryAsync(CountryName);
//        //        if (result == null)
//        //            return NotFound(new { Message = "Country not found" });

//        //        if (result == false)
//        //            return BadRequest(new { Message = "Country in use" });

//        //        return Ok(new { Message = "Country deleted successfully" });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        _logger.LogError(ex, "Error deleting Country.");
//        //        return StatusCode(500, "Internal server error");
//        //    }
//        //}

//        [HttpDelete("businessManager/Location/city/{countryName}/{cityName}")]
//        public async Task<IActionResult> DeleteCity(string cityName, string countryName)
//        {
//            try
//            {
//                var result = await _locationService.DeleteCityAsync(cityName, countryName);
//                if (result == null)
//                    return NotFound(new { Message = "City not found" });

//                if (result == false)
//                    return BadRequest(new { Message = "City in use" });

//                return Ok(new { Message = "City deleted successfully" });
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting city.");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//    }
//}