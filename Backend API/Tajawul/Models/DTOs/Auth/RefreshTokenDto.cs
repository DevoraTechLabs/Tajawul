using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Auth
{
    public class RefreshTokenDto
    {
        [Required(ErrorMessage = "Refresh token is required")]
        [StringLength(800, MinimumLength = 30, ErrorMessage = "Invalid request")]
        public required string RefreshToken { get; set; }

    }

}

