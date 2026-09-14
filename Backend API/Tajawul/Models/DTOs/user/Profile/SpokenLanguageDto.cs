using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.user.Profile
{
    public class SpokenLanguageDto
    {
        [Required(ErrorMessage = "Language is required.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Language must be between 3 and 20 characters.")]
        [Alphanumeric]
        public required string Name { get; set; }
    }

}
