using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AllowedVisibilitiesAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> _allowedVisibilities = ["public", "private", "tripHub"];

        public AllowedVisibilitiesAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage ?? $"Invalid visibility. Allowed values are: Public, Private, TripHub.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle [Required] separately if needed
            }

            string visibility = value.ToString() ?? ""; // Convert to lowercase for case-insensitive check

            if (!_allowedVisibilities.Contains(visibility, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}