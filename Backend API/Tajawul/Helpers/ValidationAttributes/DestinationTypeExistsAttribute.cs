using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Tajawul.Helpers.ValidationAttributes 
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class DestinationTypeExistsAttribute : ValidationAttribute
    {
        // Private helper class to match the JSON structure { "destinations": [...] }
        private class DestinationTypesJson
        {
            // Property name must match the key in the JSON file ("destinations")
            public List<string>? Destinations { get; set; }
        }

        internal static HashSet<string>? _supportedDestinationTypes;
        internal static readonly object _lock = new(); // For thread-safe loading

        public DestinationTypeExistsAttribute() : base("The destination type is not supported.")
        {
        }

        public DestinationTypeExistsAttribute(string errorMessage) : base(errorMessage)
        {
        }

       

        // Method to load destination types
        public static void LoadSupportedDestinationTypes(string contentRootPath, string subfolder = "Helpers", string fileName = "destination_types.json") // Default subfolder changed to 'Data' as an example
        {
            if (_supportedDestinationTypes != null) return; // Already loaded

            lock (_lock) // Ensure thread safety for loading
            {
                if (_supportedDestinationTypes != null) return; // Check again inside the lock

                string filePath = Path.Combine(contentRootPath, subfolder, fileName);

                try
                {
                    if (!File.Exists(filePath))
                    {
                        string errorMessage = $"FATAL ERROR: Supported destination types file not found: {filePath}";
                        throw new FileNotFoundException(errorMessage, filePath); // More specific exception
                    }

                    string jsonString = File.ReadAllText(filePath);

                    var deserializedData = JsonSerializer.Deserialize<DestinationTypesJson>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }); // Case-insensitive property name matching

                    var destinationList = deserializedData?.Destinations;

                    if (destinationList == null || destinationList.Count == 0)
                    {
                        string errorMessage = $"FATAL ERROR: Supported destination types file is empty, invalid JSON, or missing the 'destinations' array: {filePath}";
                        throw new InvalidOperationException(errorMessage);
                    }

                    _supportedDestinationTypes = new HashSet<string>(destinationList.Where(d => !string.IsNullOrWhiteSpace(d)).Select(d => d.Trim().ToUpperInvariant()), StringComparer.OrdinalIgnoreCase);

                }
                catch (JsonException ex)
                {
                    string errorMessage = $"FATAL ERROR: Could not parse supported destination types JSON file {filePath}: {ex.Message}";
                    throw new InvalidOperationException(errorMessage, ex);
                }
                catch (Exception ex) // Catch other potential errors (IO, etc.)
                {
                    string errorMessage = $"FATAL ERROR: An unexpected error occurred loading supported destination types from {filePath}: {ex.Message}";
                    throw new InvalidOperationException(errorMessage, ex);
                }
            }
        }

        // Override the IsValid method to perform the validation
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Ensure destination types are loaded before validation runs
            if (_supportedDestinationTypes == null)
            {
                // This indicates a startup configuration problem.
                return new ValidationResult("Destination Type validation list is not available. Server configuration error.");
            }

            var destinationType = value as string;

            // If the value is null or empty, let [Required] handle it.
            if (string.IsNullOrWhiteSpace(destinationType))
            {
                return ValidationResult.Success;
            }

            // Check if the destination type is supported
            if (_supportedDestinationTypes.Contains(destinationType, StringComparer.OrdinalIgnoreCase))
            {
                return ValidationResult.Success; // Validation passed
            }
            else
            {
                // Use the ErrorMessage property set in the constructor or the attribute declaration
                string specificErrorMessage = string.Format(ErrorMessageString, validationContext.DisplayName);
                // Ensure MemberName is not null
                return new ValidationResult(specificErrorMessage, new[] { validationContext.MemberName! });
            }
        }
    }
}