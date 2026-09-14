using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [MaxLength(100, ErrorMessage = "Invalid request")]
        public string? Email { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [PasswordStrength]

        public string? NewPassword { get; set; }

        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [PasswordStrength]
        public string? ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Refresh token is required")]
        [StringLength(800, MinimumLength = 30, ErrorMessage = "Invalid request")]
        public string? Token { get; set; }

    }
}
