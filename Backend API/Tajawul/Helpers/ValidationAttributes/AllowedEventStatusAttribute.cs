using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AllowedEventStatusAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> _allowedStatuses = ["Upcoming", "Ongoing", "Finished"];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle with [Required] if needed
            }

            string status = value.ToString() ?? "";

            if (!_allowedStatuses.Contains(status, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult($"Invalid status. Allowed values are: Upcoming, Ongoing, Finished.");
            }

            return ValidationResult.Success;
        }
    }
}