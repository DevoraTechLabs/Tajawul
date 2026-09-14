using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class NoDuplicateStringsAttribute : ValidationAttribute
    {
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

            var duplicates = stringList.GroupBy(x => x)
                                         .Where(g => g.Count() > 1)
                                         .Select(g => g.Key)
                                         .ToList();

            if (duplicates.Count != 0)
            {
                return new ValidationResult($"The following strings are duplicated: {string.Join(", ", duplicates)}");
            }

            return ValidationResult.Success;
        }
    }
}