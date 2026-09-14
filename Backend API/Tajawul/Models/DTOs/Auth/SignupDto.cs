using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Auth
{
    public class SignupDto
    {

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [MaxLength(100, ErrorMessage = "Email must be at most 100 characters long.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [PasswordStrength]
        public required string Password { get; set; }

        [Required(ErrorMessage = "Please confirm your password.")]
        [Compare(nameof(Password), ErrorMessage = "The password and confirmation password do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Client URI is required")]
        [Url(ErrorMessage = "Please provide a valid URL (e.g., https://www.instagram.com).")]
        [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters.")]
        public required string ClientURI { get; set; }
    }
}
