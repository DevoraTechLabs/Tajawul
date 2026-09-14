using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Comment;

public class ToggleVotingCommentDto
{
    [Required(ErrorMessage = "Comment ID is required.")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "Comment ID not found.")]
    public required string CommentId { get; set; }
}
