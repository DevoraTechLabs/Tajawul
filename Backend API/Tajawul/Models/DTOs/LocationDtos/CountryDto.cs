using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.LocationDtos
{
    public class CountryDto
    {
        [Required(ErrorMessage = "Country is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public required string Name { get; set; }
    }

}
