using Tajawul.Models.ViewModels.Destination;

namespace Tajawul.Models.Domain.SocialMedia.Posts
{
    public class Post
    {
        public required string PostId { get; set; }

        public string? Content { get; set; }

        public string? Visibility { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public List<DestinationUserDto>? Destinations { get; set; }

        public List<DestinationUserDto>? Trips { get; set; }

        public List<DestinationUserDto>? Events { get; set; }

        public required List<string> Creator { get; set; }

        public int UpVotesCount { get; set; }

        public int DownVotesCount { get; set; }

        public int CommentsCount { get; set; }

        public int SharesCount { get; set; }

        public List<string> Tags { get; set; } = new List<string>();

        public DateTime CreationDate { get; set; }

        public DateTime LastEditDate { get; set; }
    }
}
