using Tajawul.Models.Domain.SocialMedia.Posts;

namespace Tajawul.Models.ViewModels.SocialMedia.Posts
{
    public class FeedPostDto
    {
        public required string PostId { get; set; }

        public string? PostTitle { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public PostEmbedding? Embeddings { get; set; }

        public required List<string> Creator { get; set; }

        public int UpVotesCount { get; set; }

        public int DownVotesCount { get; set; }

        public int CommentsCount { get; set; }

        public int SharesCount { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public DateTime CreationDate { get; set; }
    }
}
