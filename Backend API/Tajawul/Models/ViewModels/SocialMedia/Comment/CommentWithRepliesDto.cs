namespace Tajawul.Models.ViewModels.SocialMedia.Comment;

public class CommentWithRepliesDto : PaginatedResultDto<CommentWithAuthorDto>
{
    public required CommentWithAuthorDto RootComment { get; set; }
}
