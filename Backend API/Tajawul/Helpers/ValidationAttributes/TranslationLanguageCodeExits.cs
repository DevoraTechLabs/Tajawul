using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Tajawul.Helpers.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class TranslationLanguageCodeExits : ValidationAttribute
{
    private class TranslationLanguageCodesJson
    {
        // Property name must match the key in the JSON file ("LanguageCodes")
        public List<string>? LanguageCodes { get; set; }
    }

    internal static HashSet<string>? _supportedLanguageCodes;
    internal static readonly object _lock = new(); // For thread-safe loading

    public TranslationLanguageCodeExits() : base("The language code is not supported.")
    {
    }

    public TranslationLanguageCodeExits(string errorMessage) : base(errorMessage)
    {
    }



    // Method to load language codes
    public static void LoadSupportedLanguageCodes(string contentRootPath, string subfolder = "Helpers", string fileName = "translation_language_codes.json") // Default subfolder changed to 'Data' as an example
    {
        if (_supportedLanguageCodes != null) return; // Already loaded

        lock (_lock) // Ensure thread safety for loading
        {
            if (_supportedLanguageCodes != null) return; // Check again inside the lock

            string filePath = Path.Combine(contentRootPath, subfolder, fileName);

            try
            {
                if (!File.Exists(filePath))
                {
                    string errorMessage = $"FATAL ERROR: Supported language codes file not found: {filePath}";
                    throw new FileNotFoundException(errorMessage, filePath); // More specific exception
                }

                string jsonString = File.ReadAllText(filePath);

                var deserializedData = JsonSerializer.Deserialize<TranslationLanguageCodesJson>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // Case-insensitive property name matching

                var LanguageCodesList = deserializedData?.LanguageCodes;

                if (LanguageCodesList == null || LanguageCodesList.Count == 0)
                {
                    string errorMessage = $"FATAL ERROR: Supported language codes file is empty, invalid JSON, or missing the 'languageCodes' array: {filePath}";
                    throw new InvalidOperationException(errorMessage);
                }

                _supportedLanguageCodes = new HashSet<string>(LanguageCodesList.Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => d.Trim().ToUpperInvariant()), StringComparer.OrdinalIgnoreCase);

            }
            catch (JsonException ex)
            {
                string errorMessage = $"FATAL ERROR: Could not parse supported language codes JSON file {filePath}: {ex.Message}";
                throw new InvalidOperationException(errorMessage, ex);
            }
            catch (Exception ex) // Catch other potential errors (IO, etc.)
            {
                string errorMessage = $"FATAL ERROR: An unexpected error occurred loading supported language codes from {filePath}: {ex.Message}";
                throw new InvalidOperationException(errorMessage, ex);
            }
        }
    }

    // Override the IsValid method to perform the validation
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Ensure language codes are loaded before validation runs
        if (_supportedLanguageCodes == null)
        {
            // This indicates a startup configuration problem.
            return new ValidationResult("language codes validation list is not available. Server configuration error.");
        }

        var languageCode = value as string;

        // If the value is null or empty, let [Required] handle it.
        if (string.IsNullOrWhiteSpace(languageCode))
        {
            return ValidationResult.Success;
        }

        // Check if the language codes is supported
        if (_supportedLanguageCodes.Contains(languageCode, StringComparer.OrdinalIgnoreCase))
        {
            return ValidationResult.Success; // Validation passed
        }
        else
        {
            // Use the ErrorMessage property set in the constructor or the attribute declaration
            string specificErrorMessage = string.Format(ErrorMessageString, validationContext.DisplayName);
            // Ensure MemberName is not null
            return new ValidationResult(specificErrorMessage, [validationContext.MemberName!]);
        }
    }

}
