using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Event
{
    public class EventTagsDto
    {
        [ListSize(1, 10, ErrorMessage = "Tags must be between 1 and 10 items long.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Tags must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public required List<string> Tags { get; set; }
    }
}
