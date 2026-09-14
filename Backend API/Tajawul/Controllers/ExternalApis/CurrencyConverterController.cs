using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tajawul.Models.DTOs.ExternalAPIs;
using Tajawul.Models.ViewModels.Weather;
using Tajawul.Services.Weather;

namespace Tajawul.Controllers.ExternalApis;

[ApiController]
[Route("[controller]")]
public class CurrencyConverterController : ControllerBase
{
    private readonly CurrencyConverterService _currencyConverterService;
    private readonly ILogger<CurrencyConverterController> _logger;

    public CurrencyConverterController(
        CurrencyConverterService currencyConverterService,
        ILogger<CurrencyConverterController> logger)
    {
        _currencyConverterService = currencyConverterService;
        _logger = logger;
    }

    [HttpGet("convert")]
    public async Task<IActionResult> Convert([FromQuery] ConvertCurrencyInputDto input)
    {

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid input received for currency conversion.");
            return BadRequest(ModelState);
        }

        _logger.LogInformation($"Attempting to convert {input.Amount} {input.FromCurrency} to {input.ToCurrency}");

        try
        {
            ConvertCurrencyOutputDto? result = await _currencyConverterService.ConvertCurrencyAsync(input);

            if (result == null || result.ConvertedAmount == null)
            {
                _logger.LogError("Currency conversion service returned null (indicating failure).");
                return StatusCode(500, "Failed to perform currency conversion");
            }

            _logger.LogInformation($"Conversion successful: {input.Amount} {input.FromCurrency} = {result.ConvertedAmount} {input.ToCurrency}");
            return Ok(result);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred during currency conversion.");
            return StatusCode(500, "Internal server error occurred during currency conversion.");
        }
    }
}
