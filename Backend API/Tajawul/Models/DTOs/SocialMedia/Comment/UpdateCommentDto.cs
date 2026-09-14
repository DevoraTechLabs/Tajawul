using System;
using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Comment;

public class UpdateCommentDto
{
    [Required(ErrorMessage = "Comment ID is required.")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "Comment not found.")]
    public required string CommentId { get; set; }

    [Required(ErrorMessage = "Content is required")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 1000 characters long")]
    public required string Content { get; set; }
}
