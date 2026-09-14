using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.Domain.General
{
    public class SocialMediaLink
    {
        [Required(ErrorMessage = "Social media platform name is required (e.g., Facebook, Instagram).")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Platform name must be between 2 and 50 characters.")]
        [Alphanumeric]
        public required string Platform { get; set; }

        [Required(ErrorMessage = "Social media URL is required.")]
        [Url(ErrorMessage = "Please provide a valid URL (e.g., https://www.instagram.com).")]
        [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters.")]
        public required string Url { get; set; }
    }
}