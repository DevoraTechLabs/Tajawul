using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Comment;

public class CreateCommentDto
{
    [Required(ErrorMessage = "Post ID is required.")]
    [StringLength(36, MinimumLength = 36, ErrorMessage = "Post not found.")]
    public required string PostId { get; set; }
    [Required(ErrorMessage = "Content is required")]
    [StringLength(1000, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 1000 characters long")]
    public required string Content { get; set; }
}
