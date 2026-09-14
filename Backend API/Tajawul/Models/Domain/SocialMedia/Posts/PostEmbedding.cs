namespace Tajawul.Models.Domain.SocialMedia.Posts
{
    public class PostEmbedding
    {
        public required string Type { get; set; }

        public required List<Tuple<string, string, string>> Values { get; set; }   // id name image
    }
}
