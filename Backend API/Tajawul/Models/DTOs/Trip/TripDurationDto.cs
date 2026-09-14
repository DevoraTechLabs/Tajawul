using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Trip
{
    public class TripDurationDto
    {
        [Required(ErrorMessage = "Trip duration is required.")]
        [AllowedTripDurations]
        public required string Name { get; set; }
    }
}