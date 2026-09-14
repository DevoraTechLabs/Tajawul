using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class EachImageMaxSizeAttribute : ValidationAttribute
    {
        private readonly int _maxImageSizeInMB;

        public EachImageMaxSizeAttribute(int maxImageSizeInMB)
        {
            _maxImageSizeInMB = maxImageSizeInMB;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not List<IFormFile> fileList)
            {
                return new ValidationResult("Value must be a list of IFormFile objects.");
            }

            foreach (IFormFile file in fileList)
            {
                if (file.Length > _maxImageSizeInMB * 1024 * 1024) // Convert MB to bytes
                {
                    return new ValidationResult(ErrorMessage ?? $"Each image must be less than {_maxImageSizeInMB} MB.");
                }
            }

            return ValidationResult.Success;
        }
    }
}