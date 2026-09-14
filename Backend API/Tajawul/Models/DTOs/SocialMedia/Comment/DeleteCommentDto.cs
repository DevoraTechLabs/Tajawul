using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Comment;

public class DeleteCommentDto
{
    [Required(ErrorMessage = "Comment ID is required.")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "Comment not found.")]
    public required string CommentId { get; set; }
}