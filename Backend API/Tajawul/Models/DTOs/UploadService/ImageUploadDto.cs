using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.UploadService
{
    public class ImageUploadDto
    {
        [Required(ErrorMessage = "Image is required.")]
        [DataType(DataType.Upload)]
        [AllowedImageExtensions]
        [MaxImageFileSize(2)]
        public required IFormFile ProfileImage { get; set; }

    }
}

