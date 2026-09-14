using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Auth
{
    public class SigninDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [MaxLength(100, ErrorMessage = "Invalid email or password")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Invalid email or password")]
        [PasswordStrength(ErrorMessage = "Invalid email or password")]
        public required  string Password { get; set; } 
    }
}
