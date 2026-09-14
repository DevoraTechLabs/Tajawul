using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.DTOs.Destination
{

    public class DestinationAttributesDto
    {

        [ListSize(0, 5, ErrorMessage = "Group sizes must be between 1 and 5 items long.")]
        [EachAllowedGroupSize]
        public List<string>? GroupSizes { get; set; }

        [ListSize(0, 20, ErrorMessage = "Activities must be between 1 and 20 items long.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Activities must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string>? Activities { get; set; } = [];

        [ListSize(0, 20, ErrorMessage = "Tags must be between 1 and 20 items long.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Tags must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string>? Tags { get; set; } = [];

    }
}
