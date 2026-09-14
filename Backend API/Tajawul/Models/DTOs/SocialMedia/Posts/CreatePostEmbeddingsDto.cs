namespace Tajawul.Models.DTOs.SocialMedia.Posts
{
    public class CreatePostEmbeddingsDto
    {
        public required string Type { get; set; }

        public required List<string> Values { get; set; }
    }
}
