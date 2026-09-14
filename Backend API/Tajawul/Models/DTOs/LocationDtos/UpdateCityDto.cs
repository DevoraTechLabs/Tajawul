using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;
using Tajawul.Models.Domain;

namespace Tajawul.Models.DTOs.LocationDtos
{
    public class UpdateCityDto
    {
        [Required(ErrorMessage = "City is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "City name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public required string Name { get; set; }


    }

}
