using Tajawul.Models.DTOs.UploadService;

namespace Tajawul.Models.DTOs.SocialMedia.Posts
{
    public class CreatePostDto
    {
        public string? Content { get; set; }

        public string? Visibility { get; set; }

        public ImagesUploadDto? Images { get; set; }

        public List<string>? Destinations { get; set; }

        public List<string>? Trips { get; set; }

        public List<string>? Events { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
