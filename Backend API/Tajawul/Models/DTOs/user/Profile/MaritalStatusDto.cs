using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.user.Profile
{
    public class MaritalStatusDto
    {
        [Required(ErrorMessage = "Marital status is required.")]
        [MaritalStatus]
        public required string Name { get; set; }
    }

}
