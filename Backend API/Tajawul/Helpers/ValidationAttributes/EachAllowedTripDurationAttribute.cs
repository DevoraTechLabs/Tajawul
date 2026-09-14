using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class EachAllowedTripDurationAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> _allowedTripDurations = ["day", "3 days", "week", "2 weeks", "month", "more than month"];

        public EachAllowedTripDurationAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage ?? $"Invalid trip duration. Allowed values are: Day, 3 Days, Week, 2 Weeks, Month, More than Month.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle [Required] separately if needed
            }

            if (value is not List<string> tripDurations)
            {
                return new ValidationResult("Value must be a list of strings.");
            }

            foreach (string tripDuration in tripDurations)
            {
                if (string.IsNullOrEmpty(tripDuration))
                {
                    continue; // Skip empty strings
                }

                if (!_allowedTripDurations.Contains(tripDuration, StringComparer.OrdinalIgnoreCase))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}