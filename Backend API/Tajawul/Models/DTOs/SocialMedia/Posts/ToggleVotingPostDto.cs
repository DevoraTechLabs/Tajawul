using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.SocialMedia.Posts
{
    public class ToggleVotingPostDto
    {
        [Required(ErrorMessage = "Post ID is required.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Post ID not found.")]
        public required string PostId { get; set; }
    }
}
