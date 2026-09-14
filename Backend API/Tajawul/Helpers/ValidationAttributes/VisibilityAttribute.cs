using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class VisibilityAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedValues = new List<string> { "Public", "Private", "TripHub" };

            if (value is string stringValue && allowedValues.Contains(stringValue, StringComparer.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Invalid visibility. Allowed values: Public, Private, TripHub", validationContext.MemberName != null ? new[] { validationContext.MemberName } : null);
        }
    }
}