using System;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class PastDateAttribute : ValidationAttribute
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

            if (dateValue > today)
            {
                return new ValidationResult(ErrorMessage ?? "Date cannot be in the future.");
            }

            return ValidationResult.Success;
        }
    }
}