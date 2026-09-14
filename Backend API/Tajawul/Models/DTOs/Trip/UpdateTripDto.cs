using Neo4j.Driver;
using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;
using Tajawul.Models.Domain;

namespace Tajawul.Models.DTOs.Trip
{
    public class UpdateTripDto
    {
        [Required(ErrorMessage = "Trip ID is required.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Trip not found.")]
        public required string TripId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Title must be between 3 and 100 characters")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Description must be between 3 and 1000 characters")]
        public required string Description { get; set; }


        [Required(ErrorMessage = "Price range is required")]
        [PriceRange]
        public required string PriceRange { get; set; }

        [Required(ErrorMessage = "Trip duration is required")]
        [TripDuration]
        public required string TripDuration { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [AllowedTripStatus]
        public required string Status { get; set; }

        [Required(ErrorMessage = "Visibility is required")]
        [Visibility]
        public required string Visibility { get; set; }


    }
}
