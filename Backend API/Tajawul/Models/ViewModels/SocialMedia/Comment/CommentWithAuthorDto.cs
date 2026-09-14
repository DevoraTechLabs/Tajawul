
using Tajawul.Models.Domain.SocialMedia;
using Tajawul.Models.ViewModels.user;

namespace Tajawul.Models.ViewModels.SocialMedia.Comment;

public class CommentWithAuthorDto : CommentModel
{
    public UserBasicInfoDto? Author { get; set; }
    public bool IsUpvotedByCurrentUser { get; set; }
    public bool IsDownvotedByCurrentUser { get; set; }
}
