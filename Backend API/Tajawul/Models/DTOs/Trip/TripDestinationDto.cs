using System.ComponentModel.DataAnnotations;

namespace Tajawul.Models.DTOs.Trip
{
    public class TripDestinationDto
    {

        [Required(ErrorMessage = "Day is required")]
        [Range(1, 365, ErrorMessage = "Day must be between 1 and 365")]
        public required int Day { get; set; }

    }
}