using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class EachAllowedGroupSizeAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> _allowedGroupSizes = ["solo", "couple", "family", "group", "big group"];

        public EachAllowedGroupSizeAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage ?? $"Invalid group size. Allowed values are: Solo, Couple, Family, Group, Big Group.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle [Required] separately if needed
            }

            if (value is not List<string> groupSizes)
            {
                return new ValidationResult("Value must be a list of strings.");
            }

            foreach (string groupSize in groupSizes)
            {
                if (string.IsNullOrEmpty(groupSize))
                {
                    continue; // Or return an error if empty strings are not allowed
                }

                if (!_allowedGroupSizes.Contains(groupSize, StringComparer.OrdinalIgnoreCase))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}