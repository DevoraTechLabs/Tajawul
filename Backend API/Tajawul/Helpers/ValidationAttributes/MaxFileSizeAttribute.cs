using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSizeInMB;
        private readonly int _maxFileSizeInBytes;

        public MaxFileSizeAttribute(int maxFileSizeInMB)
        {
            _maxFileSizeInMB = maxFileSizeInMB;
            _maxFileSizeInBytes = maxFileSizeInMB * 1024 * 1024;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                if (file.Length > _maxFileSizeInBytes)
                {
                    return new ValidationResult(ErrorMessage ?? $"The file size exceeds the maximum allowed size of {_maxFileSizeInMB} MB.");
                }
            }
            else
            {
                return new ValidationResult("The value must be a file.");
            }

            return ValidationResult.Success;
        }
    }
}