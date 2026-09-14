using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.UploadService
{
    public class ImagesUploadDto
    {
        [ListSize(1, 5, ErrorMessage = "You must upload between 1 and 5 images.")]
        [EachImageMaxSize(2, ErrorMessage = "Each image must be less than 2 MB.")]
        [EachImageExtension]
        public required List<IFormFile> Images { get; set; }
    }
}
