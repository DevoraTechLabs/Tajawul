using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Auth
{
    public class ChangePasswordDto
    {
        [Required(ErrorMessage = "Old password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Invalid request.")]
        [PasswordStrength(ErrorMessage = "Invalid request.")]
        public required string OldPassword { get; set; }

        [Required(ErrorMessage = "New password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long.")]
        [PasswordStrength]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Please confirm your new password.")]
        [Compare(nameof(NewPassword), ErrorMessage = "The new password and confirmation password do not match.")]
        public required string ConfirmNewPassword { get; set; }

    }
}
