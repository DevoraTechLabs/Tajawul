using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AlphanumericAttribute : ValidationAttribute
    {
        public AlphanumericAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage; // Set the ErrorMessage property
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle nulls
            }

            string stringValue = value.ToString() ?? ""; //Handle nulls and ensure its a string

            if (!Regex.IsMatch(stringValue, "^[a-zA-Z0-9_\\-\\s&]*$"))
            {
                return new ValidationResult(ErrorMessage ?? "Value contains invalid characters. Only alphanumeric characters (letters, numbers, underscores, hyphens, and spaces) are allowed.");
            }

            return ValidationResult.Success;
        }
    }
}

