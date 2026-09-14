using System;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes{

public class GenderAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var genders = new List<string> { "Male", "Female" };

        if (genders.Any(gender => string.Equals(gender, value?.ToString(), StringComparison.OrdinalIgnoreCase)))
        {
            return ValidationResult.Success;
        }

        return new ValidationResult($"Invalid gender. Allowed values: {string.Join(", ", genders)}", [nameof(value)]);
    }
}
}