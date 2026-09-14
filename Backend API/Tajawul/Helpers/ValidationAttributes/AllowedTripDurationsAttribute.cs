using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AllowedTripDurationsAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> _allowedTripDurations = ["day", "3 days", "week", "2 weeks", "month", "more than month"];

        public AllowedTripDurationsAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage ?? $"Invalid trip duration. Allowed values are: Day, 3 Days, Week, 2 Weeks, Month, More than Month.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle [Required] separately if needed
            }

            string tripDuration = value.ToString() ?? ""; // Convert to lowercase for case-insensitive check

            if (!_allowedTripDurations.Contains(tripDuration, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}