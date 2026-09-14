using Tajawul.Helpers;
using Tajawul.Helpers.Filters.SocialMedia;
using Tajawul.Interfaces.General;
using Tajawul.Interfaces.Media;
using Tajawul.Interfaces.SocialMedia;
using Tajawul.Interfaces.Trips;
using Tajawul.Models.Domain.SocialMedia.Posts;
using Tajawul.Models.Domain.Uploads;
using Tajawul.Models.DTOs.Comment;
using Tajawul.Models.DTOs.SocialMedia.Posts;
using Tajawul.Models.Enums;
using Tajawul.Models.ViewModels.SocialMedia;
using Tajawul.Repositories.SocialMedia;
using Tajawul.Repositories.SocialMedia.Posts;


namespace Tajawul.Services.SocialMedia.Posts
{
    public class PostsService : IPostsService
    {
        private readonly PostRepository _postRepository;
        private readonly VotingRepository _votingRepository;
        private readonly IVisibilityService _visibilityService;
        private readonly ITagService _tagService;
        private readonly IAzureStorageService _azureStorageService;

        public PostsService(PostRepository postRepository, VotingRepository votingRepository, IVisibilityService visibilityService, ITagService tagService, IAzureStorageService azureStorageService)
        {
            _postRepository = postRepository;
            _votingRepository = votingRepository;
            _visibilityService = visibilityService;
            _tagService = tagService;
            _azureStorageService = azureStorageService;
        }

        public async Task<(Post, List<string>)> CreatePostAsync(CreatePostDto postDto, string userId)
        {
            List<string> failures = new();

            Post post = await _postRepository.CreatePostAsync(postDto, userId);

            try
            {
                var visibility = await _visibilityService.AssignVisibilityAsync(
                    postDto.Visibility ?? "Private",
                    post.PostId,
                    GraphRelations.Post.HasVisibility
                );
                post.Visibility = visibility.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign visibility: {ex.Message}");
            }

            try
            {
                var tags = await _tagService.AssignTagsAsync(postDto.Tags, post.PostId, GraphRelations.Post.HadTag);

                post.Tags = tags;
            }
            catch (Exception ex)
            {
                failures.Add($"Tags assignment failed: {ex.Message}");
            }

            try
            {
                if (postDto.Destinations != null && postDto.Destinations.Any())
                {
                    var Destinations = await _postRepository.CreatePostEmbeddingsAsync(post.PostId, postDto.Destinations, "Destination");
                    post.Destinations = Destinations;
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Creating destination embeddings failed: {ex.Message}");
            }

            try
            {
                if (postDto.Trips != null && postDto.Trips.Any())
                {
                    var Trips = await _postRepository.CreatePostEmbeddingsAsync(post.PostId, postDto.Trips, "Trip");
                    post.Trips = Trips;
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Creating trip embeddings failed: {ex.Message}");
            }

            try
            {
                if (postDto.Events != null && postDto.Events.Any())
                {
                    var Events = await _postRepository.CreatePostEmbeddingsAsync(post.PostId, postDto.Events, "Event");
                    post.Events = Events;
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Creating event embeddings failed: {ex.Message}");
            }


            if (postDto.Images != null && postDto.Images.Images.Any())
            {
                ImageUploadResult uploadResults;

                try
                {
                    uploadResults = await _azureStorageService.UploadImagesAsync(postDto.Images.Images, "posts", post.PostId);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to upload images to storage.", ex);
                }

                try
                {
                    await _postRepository.UpdatePostImagesAsync(uploadResults.Success, post.PostId);
                    post.Images = uploadResults.Success;
                }
                catch (Exception ex)
                {
                    try
                    {
                        await _azureStorageService.DeleteImagesAsync(uploadResults.Success);
                    }
                    catch (Exception)
                    {
                        throw new Exception("Failed to delete the file.", ex);
                    }
                    throw new Exception("Failed to update post images.", ex);
                }
            }

            return (post, failures);
        }

        public async Task<bool> DeleteUserPostAsync(string postId, string userId)
        {
            return await _postRepository.DeletePostAsync(postId, userId);
        }

        public async Task<List<Post>> GetUserFeedAsync(PostsFilter postsFilter)
        {
            return await _postRepository.GetUserFeedAsync(postsFilter) ?? [];
        }

        public async Task<List<Post>> GetUserPostsAsync(string userId)
        {
            return await _postRepository.GetUserPostsAsync(userId) ?? [];
        }

        public async Task<VotingResult?> ToggleDownvotePostAsync(ToggleVotingPostDto upvotePostDto, string userId)
        {
            return await _votingRepository.ToggleVoting(VotingEnum.Post, VotingRelationshipEnum.DOWNVOTED, upvotePostDto.PostId, userId);
        }

        public async Task<VotingResult?> ToggleUpvotePostAsync(ToggleVotingPostDto upvotePostDto, string userId)
        {
            return await _votingRepository.ToggleVoting(VotingEnum.Post, VotingRelationshipEnum.UPVOTED, upvotePostDto.PostId, userId);
        }
    }
}
