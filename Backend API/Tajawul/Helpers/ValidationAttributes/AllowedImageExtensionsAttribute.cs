using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Linq;

namespace Tajawul.Helpers.ValidationAttributes
{
    public class AllowedImageExtensionsAttribute : AllowedExtensionsAttribute
    {
        private static readonly string[] _allowedImageExtensions = [".jpg", ".jpeg", ".png", ".webp"];

        public AllowedImageExtensionsAttribute(string? errorMessage = null)
         : base(_allowedImageExtensions)
        {
            ErrorMessage = errorMessage ?? "Only image files with the following extensions are allowed: .jpg, .jpeg, .png, .gif, .bmp, .webp"; 
        }
    }
}