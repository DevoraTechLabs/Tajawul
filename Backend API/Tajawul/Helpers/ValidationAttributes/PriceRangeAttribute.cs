using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class PriceRangeAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedNames = new List<string> { "low", "mid", "luxury" };

            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle nulls
            }

            if (value is string stringValue && allowedNames.Contains(stringValue, StringComparer.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Invalid name. Allowed values: {string.Join(", ", allowedNames)}", [nameof(value)]);
        }
    }
}

