using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AllowedTripStatusAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var allowedValues = new List<string> { "notstarted", "inprogress", "completed", "canceled" };

            if (value == null)
            {
                return ValidationResult.Success; // Allow nulls, handle with [Required] if needed
            }
            if (value is string stringValue && allowedValues.Contains(stringValue, StringComparer.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Invalid status. Allowed values: notStarted, inProgress, completed, canceled",
                [validationContext.MemberName ?? "UnknownMember"]);
        }
    }
}
