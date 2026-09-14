using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Review
{
    public class CreateReviewDto
    {

        [Required(ErrorMessage = "Destination ID is required.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Destination not found.")]
        public string DestinationId { get; set; } = null!;

        [Required(ErrorMessage = "Comment is required")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Comment must be between 1 and 1000 characters long")]
        public string Comment { get; set; } = null!;

        [Required(ErrorMessage = "Rate is required")]
        [HalfStarRating(ErrorMessage = "Rate must be between 0.0 and 5.0, in increments of 0.5")]
        public float Rate { get; set; }

    }
}
