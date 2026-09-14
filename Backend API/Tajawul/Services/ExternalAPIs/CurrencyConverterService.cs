
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tajawul.Models.ViewModels.Weather;
using Tajawul.Models.Domain.ExternalAPIs;
using Tajawul.Models.DTOs.ExternalAPIs;

namespace Tajawul.Services.Weather;

public class CurrencyConverterService
{
    private readonly HttpClient _httpClient;
    private readonly CurrencyConverter _apiSettings;
    private readonly ILogger<CurrencyConverterService> _logger;

    public CurrencyConverterService(
        IHttpClientFactory httpClientFactory,
        IOptions<CurrencyConverter> apiSettings,
        ILogger<CurrencyConverterService> logger)
    {
        _httpClient = httpClientFactory.CreateClient("ExchangeRateApi");
        _apiSettings = apiSettings.Value;
        _logger = logger;
    }

    public async Task<ConvertCurrencyOutputDto?> ConvertCurrencyAsync(ConvertCurrencyInputDto inputDto)
    {

        string requestUrl = $"{_apiSettings.ExchangeRateApiUrl}{_apiSettings.ApiKey}/pair/{inputDto.FromCurrency.ToUpper()}/{inputDto.ToCurrency.ToUpper()}/{inputDto.Amount}";

        _logger.LogInformation($"Calling external currency API: {requestUrl}");

        HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);


        if (!response.IsSuccessStatusCode)
        {
            string errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError("External API returned non-success status code {StatusCode}. Request: {RequestUrl}. Response body: {ResponseBody}",
                             response.StatusCode, requestUrl, errorContent);
            return null;
        }

        string responseBody = await response.Content.ReadAsStringAsync();
        _logger.LogInformation($"External API response body: {responseBody}");


        using JsonDocument document = JsonDocument.Parse(responseBody);
        JsonElement root = document.RootElement;


        if (!root.TryGetProperty("result", out JsonElement resultElement) || resultElement.GetString() != "success")
        {
            string apiErrorType = "Unknown";
            if (root.TryGetProperty("error-type", out JsonElement errorTypeElement) && errorTypeElement.ValueKind == JsonValueKind.String)
            {
                apiErrorType = errorTypeElement.GetString() ?? "Unknown";
            }
            _logger.LogError("External API returned 'error' status in response body. Error Type: {ApiErrorType}. Request: {RequestUrl}",
                             apiErrorType, requestUrl);
            return null;
        }


        if (root.TryGetProperty("base_code", out JsonElement baseCodeElement) && baseCodeElement.ValueKind == JsonValueKind.String)
        {
            _logger.LogDebug("API response base_code: {BaseCode}", baseCodeElement.GetString());
        }


        if (root.TryGetProperty("target_code", out JsonElement targetCodeElement) && targetCodeElement.ValueKind == JsonValueKind.String)
        {
            _logger.LogDebug("API response target_code: {TargetCode}", targetCodeElement.GetString());
        }




        if (!root.TryGetProperty("conversion_result", out JsonElement conversionResultElement) || conversionResultElement.ValueKind != JsonValueKind.Number)
        {

            _logger.LogError("External API returned 'success' but missing or invalid 'conversion_result'. Request: {RequestUrl}. Response body: {ResponseBody}",
                            requestUrl, responseBody);
            return null;
        }


        decimal convertedAmount = conversionResultElement.GetDecimal();




        return new ConvertCurrencyOutputDto
        {
            ConvertedAmount = convertedAmount
        };

    }
}