using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class EachImageExtensionAttribute : ValidationAttribute
    {
        public string[] AllowedImageExtensions { get; set; } = [".jpg", ".jpeg", ".png", ".webp"];

        public EachImageExtensionAttribute(string? errorMessage = null)
        {
            ErrorMessage = errorMessage ?? $"Only image files with the following extensions are allowed: .jpg, .jpeg, .png, .gif, .bmp, .webp";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not List<IFormFile> fileList)
            {
                return new ValidationResult("Value must be a list of IFormFile objects.");
            }

            foreach (IFormFile file in fileList)
            {
                string extension = Path.GetExtension(file.FileName).ToLower();
                if (!AllowedImageExtensions.Contains(extension))
                {
                    return new ValidationResult(ErrorMessage);
                }
            }

            return ValidationResult.Success;
        }
    }
}