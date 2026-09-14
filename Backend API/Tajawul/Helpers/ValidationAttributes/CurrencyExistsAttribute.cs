using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Tajawul.Helpers.ValidationAttributes
{

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class CurrencyExistsAttribute : ValidationAttribute
{
    private static HashSet<string>? _supportedCurrencies;
    private static readonly object _lock = new object(); // For thread-safe loading
    private static ILogger<CurrencyExistsAttribute>? _logger; // Static logger

    public CurrencyExistsAttribute() : base("The currency code is not supported.")
    {
    }

    public CurrencyExistsAttribute(string errorMessage) : base(errorMessage)
    {
    }

    // Method to initialize the logger (call this from Program.cs)
    public static void SetLogger(ILogger<CurrencyExistsAttribute> logger)
    {
        _logger = logger;
    }

        // Method to load currencies - now handles the dictionary format
        public static void LoadSupportedCurrencies(string contentRootPath, string subfolder = "Helpers", string fileName = "supported_currencies.json")
        {
            if (_supportedCurrencies != null) return; // Already loaded

            lock (_lock) // Ensure thread safety for loading
            {
                if (_supportedCurrencies != null) return; // Check again inside the lock

                string filePath = Path.Combine(contentRootPath, subfolder, fileName);

                try
                {
                    if (!File.Exists(filePath))
                    {
                        string errorMessage = $"FATAL ERROR: Supported currencies file not found: {filePath}";
                        _logger?.LogError(errorMessage); // Use the static logger
                                                         // Use a more specific exception type if possible, or just throw InvalidOperationException
                        throw new InvalidOperationException(errorMessage);
                    }

                    string jsonString = File.ReadAllText(filePath);

                    // --- CHANGE HERE: Deserialize into a dictionary ---
                    // We deserialize into a Dictionary<string, JsonElement> or <string, object>
                    // because the outer structure is a JSON object where keys are currency codes.
                    // We don't need the inner details, just the keys.
                    var currenciesDictionary = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString); // Or <string, object>

                    if (currenciesDictionary == null || !currenciesDictionary.Any())
                    {
                        string errorMessage = $"FATAL ERROR: Supported currencies file is empty or invalid JSON dictionary: {filePath}";
                        _logger?.LogError(errorMessage);
                        throw new InvalidOperationException(errorMessage);
                    }

                    // --- CHANGE HERE: Extract the keys from the dictionary ---
                    // Get the keys (currency codes) and store them in the HashSet (case-insensitive)
                    _supportedCurrencies = new HashSet<string>(currenciesDictionary.Keys.Select(c => c.Trim().ToUpper()), StringComparer.OrdinalIgnoreCase);
                    // --- END CHANGE ---

                    _logger?.LogInformation($"Loaded {_supportedCurrencies.Count} supported currencies from {filePath}."); // Use the static logger
                }
                catch (JsonException ex) // Catch JSON deserialization errors specifically
                {
                    string errorMessage = $"FATAL ERROR: Could not parse supported currencies JSON file {filePath}: {ex.Message}";
                    _logger?.LogError(ex, errorMessage);
                    throw new InvalidOperationException(errorMessage, ex); // Wrap JSON exception
                }
                catch (Exception ex) // Catch any other errors during loading
                {
                    string errorMessage = $"FATAL ERROR: An unexpected error occurred loading supported currencies from {filePath}: {ex.Message}";
                    _logger?.LogError(ex, errorMessage);
                    // Re-throw the exception to prevent application startup if loading fails.
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }
        }
        // Override the IsValid method to perform the validation
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Ensure currencies are loaded before validation runs
        // This check is a safeguard, but LoadSupportedCurrencies must be called at startup
        if (_supportedCurrencies == null)
        {
            // This indicates a serious issue during application startup.
            // Log this error if the static logger is available, otherwise rely on the load error.
            _logger?.LogError("Currency validation failed because the supported currency list was not loaded.");
            return new ValidationResult("Currency validation list is not loaded correctly. Server configuration error.");
        }

        var currency = value as string;

        // If the currency is null or empty, let the [Required] attribute handle it.
        // Returning success here means this attribute doesn't fail, but [Required] will.
        if (string.IsNullOrWhiteSpace(currency))
        {
            return ValidationResult.Success;
        }

        // Check if the upper-cased currency code exists in our set
        if (_supportedCurrencies.Contains(currency.Trim().ToUpper()))
        {
            return ValidationResult.Success; // Validation passed
        }
        else
        {
            // Validation failed - return a ValidationResult with the error message
            // Ensure MemberName is not null (it shouldn't be for property validation)
            return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName! });
        }
    }
}}