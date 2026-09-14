using System;

namespace Tajawul.Models.Domain.ExternalAPIs;

public class CurrencyConverter
{

    public required string ExchangeRateApiUrl { get; set; }
    public required string ApiKey { get; set; }
}
