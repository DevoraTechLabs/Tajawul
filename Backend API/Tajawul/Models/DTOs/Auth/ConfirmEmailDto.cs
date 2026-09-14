using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Auth
{
    public class ConfirmEmailDto
    {
        [Required(ErrorMessage = "User id is required")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Invalid request")]
        public required string PersonId { get; set; }

        [Required(ErrorMessage = "Token is required")]
        [StringLength(800, MinimumLength = 30, ErrorMessage = "Invalid request")]
        public required string Token { get; set; }
    }
}
