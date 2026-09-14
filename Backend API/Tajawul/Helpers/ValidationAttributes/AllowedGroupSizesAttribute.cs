using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AllowedGroupSizesAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> _allowedGroupSizes = ["solo", "couple", "family", "group", "big group"];

        public AllowedGroupSizesAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage ?? $"Invalid group size. Allowed values are: Solo, Couple, Family, Group, Big Group.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle [Required] separately if needed
            }

            string groupSize = value.ToString() ?? ""; // Convert to lowercase for case-insensitive check

            if (!_allowedGroupSizes.Contains(groupSize, StringComparer.OrdinalIgnoreCase))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}