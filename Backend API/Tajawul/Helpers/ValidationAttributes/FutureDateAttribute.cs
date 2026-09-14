using System;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public bool AllowToday { get; set; } = true; // Optional: Allow today's date?

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success; // Let [Required] handle nulls if needed
            }

            if (value is not DateOnly dateValue)
            {
                return new ValidationResult("Value must be a DateOnly.");
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            if (dateValue < today)
            {
                return new ValidationResult(ErrorMessage ?? "Date cannot be in the past.");
            }

            if (!AllowToday && dateValue == today)
            {
                return new ValidationResult(ErrorMessage ?? "Date cannot be today.");
            }

            return ValidationResult.Success;
        }
    }
}