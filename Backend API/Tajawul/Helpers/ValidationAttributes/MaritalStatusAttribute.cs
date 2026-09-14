using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes;

public class MaritalStatusAttribute: ValidationAttribute
{

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var maritalStatuses = new List<string> { "Single", "Married", "Divorced", "Widowed" };

        if (maritalStatuses.Any(status => string.Equals(status, value?.ToString(), StringComparison.OrdinalIgnoreCase)))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"Invalid marital status. Allowed values: {string.Join(", ", maritalStatuses)}", [nameof(value)]);
    }

}
