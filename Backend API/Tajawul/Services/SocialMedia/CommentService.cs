using System;
using Tajawul.Interfaces.Media;
using Tajawul.Interfaces.SocialMedia;
using Tajawul.Models.Domain.SocialMedia;
using Tajawul.Models.DTOs.Comment;
using Tajawul.Models.Enums;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Models.ViewModels.SocialMedia.Comment;
using Tajawul.Repositories.SocialMedia;

namespace Tajawul.Services.SocialMedia;

public class CommentService(
    IAzureStorageService azureStorageService,
     CommentRepository commentRepository,
     VotingRepository votingRepository) : ICommentService
{
    private readonly IAzureStorageService _azureStorageService = azureStorageService;
    private readonly CommentRepository _commentRepository = commentRepository;
    private readonly VotingRepository _votingRepository = votingRepository;

    public async Task<(CommentModel?, List<string> errors)> AddCommentAsync(CreateCommentDto commentDto, string userId)
    {

        var comment = await _commentRepository.AddCommentAsync(commentDto, userId);
        if (comment == null)
        {
            return (null, ["Failed to create comment. post not found or user not found."]);
        }
        return (comment, []);
    }

    public async Task<(CommentModel?, List<string> errors)> UpdateCommentAsync(UpdateCommentDto commentDto, string userId)
    {

        var comment = await _commentRepository.UpdateCommentAsync(commentDto, userId);
        if (comment == null)
        {
            return (null, ["Failed to update comment. comment not found."]);
        }

        return (comment, []);
    }


    public async Task<(CommentModel?, List<string> errors)> ReplyCommentAsync(ReplyCommentDto commentDto, string userId)
    {
        var comment = await _commentRepository.ReplyCommentAsync(commentDto, userId);
        if (comment == null)
        {
            return (null, ["Failed to reply comment. comment not found."]);
        }
        return (comment, []);
    }

    public async Task<bool> DeleteCommentAsync(DeleteCommentDto commentDto, string userId)
    {
        try
        {
            var comment = await _commentRepository.DeleteCommentAsync(commentDto.CommentId, userId);
            if (comment == null)
            {
                return false;
            }

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<VotingResult?> ToggleUpvoteCommentAsync(ToggleVotingCommentDto upvoteCommentDto, string userId)
    {
        return await _votingRepository.ToggleVoting(VotingEnum.Comment, VotingRelationshipEnum.UPVOTED, upvoteCommentDto.CommentId, userId);
    }

    public async Task<VotingResult?> ToggleDownvoteCommentAsync(ToggleVotingCommentDto upvoteCommentDto, string userId)
    {
        return await _votingRepository.ToggleVoting(VotingEnum.Comment, VotingRelationshipEnum.DOWNVOTED, upvoteCommentDto.CommentId, userId);
    }

    public async Task<CommentWithRepliesDto?> GetCommentWithRepliesAsync(string commentId, string userId, int pageNumber, int pageSize)
    {
        return await _commentRepository.GetCommentWithRepliesAsync(commentId, userId, pageNumber, pageSize);
    }

    public async Task<PaginatedResultDto<CommentWithAuthorDto>> GetPostCommentsAsync(string postId, string currentUserId, int pageNumber, int pageSize)
    {
        return await _commentRepository.GetCommentsForPostAsync(postId, currentUserId, pageNumber, pageSize);
    }
}

