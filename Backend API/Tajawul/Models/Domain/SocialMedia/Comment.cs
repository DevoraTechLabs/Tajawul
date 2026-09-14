namespace Tajawul.Models.Domain.SocialMedia;

public class CommentModel
{
    public required string CommentId { get; set; }
    public required string Content { get; set; }
    public required int UpvoteCount { get; set; }
    public required int DownvoteCount { get; set; }
    public required int RepliesCount { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}