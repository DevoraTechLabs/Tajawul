using Tajawul.Helpers.Filters.SocialMedia;
using Tajawul.Models.Domain.SocialMedia.Posts;
using Tajawul.Models.DTOs.Comment;
using Tajawul.Models.DTOs.SocialMedia.Posts;
using Tajawul.Models.ViewModels.SocialMedia;

namespace Tajawul.Interfaces.SocialMedia
{
    public interface IPostsService
    {
        Task<(Post, List<string>)> CreatePostAsync(CreatePostDto postDto, string userId);
        Task<List<Post>> GetUserPostsAsync(string userId);
        Task<List<Post>> GetUserFeedAsync(PostsFilter postsFilter);
        Task<bool> DeleteUserPostAsync(string postId, string userId);
        Task<VotingResult?> ToggleUpvotePostAsync(ToggleVotingPostDto upvotePostDto, string userId);
        Task<VotingResult?> ToggleDownvotePostAsync(ToggleVotingPostDto upvotePostDto, string userId);
    }
}
