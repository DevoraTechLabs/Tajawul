namespace Tajawul.Models.Domain.Uploads
{
    public class ImageUploadResult
    {
        public List<string> Success { get; set; } = new();
        public List<string> Failed { get; set; } = new();
    }
}
