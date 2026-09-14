using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class HalfStarRatingAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is float rate)
            {
                if (rate >= 0 && rate <= 5 && rate * 2 % 1 == 0) // Ensures values like 0.0, 0.5, 1.0, ..., 5.0
                {
                    return ValidationResult.Success;
                }
            }
            return new ValidationResult("Rate must be between 0.0 and 5.0, in increments of 0.5.");
        }
    }
}
