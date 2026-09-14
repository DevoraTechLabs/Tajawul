using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Auth
{
    public class SendResetPasswordDto
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        [MaxLength(100, ErrorMessage = "Invalid email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Client URI is required")]
        [Url(ErrorMessage = "Please provide a valid URL (e.g., https://www.instagram.com).")]
        [StringLength(2048, ErrorMessage = "URL cannot exceed 2048 characters.")]
        public string ClientURI { get; set; } = null!;

    }
}
