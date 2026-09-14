using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.General
{
    public class InterestDto
    {
        [Required(ErrorMessage = "Interest name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Interest name must be between 3 and 50 characters.")]
        [Alphanumeric]
        public required string Name { get; set; }
    }

}
