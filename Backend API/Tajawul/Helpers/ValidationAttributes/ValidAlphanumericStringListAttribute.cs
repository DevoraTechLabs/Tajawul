

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AlphanumericStringListAttribute : ValidationAttribute
    {
        private readonly int _minLength;
        private readonly int _maxLength;

        public AlphanumericStringListAttribute(int minLength, int maxLength, string errorMessage = "Invalid alphanumeric string in list.") : base(errorMessage)
        {
            _minLength = minLength;
            _maxLength = maxLength;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle [Required] separately if needed
            }

            if (value is not List<string> stringList)
            {
                return new ValidationResult("Value must be a list of strings.");
            }

            foreach (string item in stringList)
            {
                if (string.IsNullOrEmpty(item))
                {
                    return new ValidationResult("List cannot contain empty or null strings.");
                }

                if (item.Length < _minLength || item.Length > _maxLength)
                {
                    return new ValidationResult($"String length must be between {_minLength} and {_maxLength} characters.");
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(item, "^[a-zA-Z0-9_\\-\\s&]*$"))
                {
                    return new ValidationResult(ErrorMessage ?? "String contains invalid characters. Only alphanumeric characters (letters, numbers, underscores, hyphens, and spaces) are allowed.");
                }
            }

            return ValidationResult.Success;
        }
    }
}