using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class PasswordStrengthAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Password is required."); // Or ValidationResult.Success if nulls are allowed and should be handled elsewhere
            }

            string password = value.ToString() ?? "";

            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{8,}$";

            if (!Regex.IsMatch(password, pattern))
            {
                return new ValidationResult(ErrorMessage ?? "Password must contain uppercase, lowercase, digit, and special character and be at least 8 characters long.");
            }

            return ValidationResult.Success;
        }
    }
}