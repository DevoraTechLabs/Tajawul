using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.Domain.Events
{
    public class EventLocation
    {
        [Required(ErrorMessage = "Longitude is required.")]
        [Range(-180.0, 180.0, ErrorMessage = "Longitude must be between -180 and 180 degrees.")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Latitude is required.")]
        [Range(-90.0, 90.0, ErrorMessage = "Latitude must be between -90 and 90 degrees.")]
        public double Latitude { get; set; }

        [Required(ErrorMessage = "Address is required for the event location.")]

        [StringLength(500, MinimumLength = 5, ErrorMessage = "Address must be between 5 and 250 characters.")]
        public required string Address { get; set; }
    }
}
