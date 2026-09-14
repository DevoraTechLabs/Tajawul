using Microsoft.Extensions.Options;
using System.Text.Json;
using Tajawul.Data.Configuration;
using Tajawul.Models.ViewModels.Weather;

namespace Tajawul.Services.Weather
{
    public class VCWeatherService
    {
        private readonly string _apiUrl;
        private readonly string _apiKey;
        private readonly string _contentType;
        private readonly HttpClient _httpClient;

        public VCWeatherService(HttpClient httpClient, IOptions<VCWeatherConfiguration> options)
        {
            _httpClient = httpClient;
            _apiUrl = options.Value.Url;
            _apiKey = options.Value.Key;
            _contentType = options.Value.ContentType;
        }

        public async Task<WeatherResposeDto> GetCurrentCityWeather(string city)
        {
            var url = $"{_apiUrl}/timeline/{city}?unitGroup=metric&key={_apiKey}&contentType={_contentType}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var responseContent = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<WeatherResposeDto>(responseContent, options);
                return data;
            }
            else
            {
                throw new Exception("failed to get the weather data" + response.StatusCode);
            }
        }
    }
}
