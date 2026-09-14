using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class TripDurationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedValues = new List<string> { "short", "mid", "long" };

            if (value is string stringValue && allowedValues.Contains(stringValue, StringComparer.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Invalid Duration. Allowed values: Short, Mid, Long", validationContext.MemberName != null ? new[] { validationContext.MemberName } : null);
        }
    }
}
