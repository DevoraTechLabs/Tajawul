using Tajawul.Models.Domain.SocialMedia;
using Tajawul.Models.DTOs.Comment;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.SocialMedia.Comment;

namespace Tajawul.Interfaces.SocialMedia;

public interface ICommentService
{
    Task<(CommentModel?, List<string> errors)> AddCommentAsync(CreateCommentDto commentDto, string userId);
    Task<(CommentModel?, List<string> errors)> UpdateCommentAsync(UpdateCommentDto commentDto, string userId);
    Task<(CommentModel?, List<string> errors)> ReplyCommentAsync(ReplyCommentDto commentDto, string userId);
    Task<bool> DeleteCommentAsync(DeleteCommentDto commentDto, string userId);
     Task<VotingResult?> ToggleUpvoteCommentAsync(ToggleVotingCommentDto upvoteCommentDto, string userId);
     Task<VotingResult?> ToggleDownvoteCommentAsync(ToggleVotingCommentDto upvoteCommentDto, string userId);
     Task<CommentWithRepliesDto?> GetCommentWithRepliesAsync(string commentId, string userId,int pageNumber, int pageSize);
     Task<PaginatedResultDto<CommentWithAuthorDto>> GetPostCommentsAsync(string postId, string currentUserId, int pageNumber, int pageSize);
}
