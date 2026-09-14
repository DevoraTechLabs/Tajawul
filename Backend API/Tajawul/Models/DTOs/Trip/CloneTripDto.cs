using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Trip
{
    public class CloneTripDto
    {
        [Required(ErrorMessage = "Trip ID is required.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Trip not found.")]
        public required string TripId { get; set; }
    }
}
