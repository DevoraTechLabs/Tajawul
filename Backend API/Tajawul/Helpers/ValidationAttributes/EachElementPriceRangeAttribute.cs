using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class EachElementPriceRangeAttribute : ValidationAttribute
    {
        private readonly List<string> _allowedValues = ["Low", "Mid", "Luxury"];

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is List<string> priceRanges)
            {
                foreach (string priceRange in priceRanges)
                {
                    if (!IsValidPriceRange(priceRange))
                    {
                        return new ValidationResult(ErrorMessage, [validationContext.MemberName]); 
                    }
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Value must be a list of strings.", [validationContext.MemberName]);
        }

        private bool IsValidPriceRange(string priceRange)
        {
            return _allowedValues.Any(allowedValue => allowedValue.Equals(priceRange, StringComparison.OrdinalIgnoreCase));
        }
    }
}