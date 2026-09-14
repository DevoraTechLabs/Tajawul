using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Models.Domain.Users
{
    public class UserInterests
    {
        [ListSize(1, 10, ErrorMessage = "Destination types must be between 1 and 10 items long.")]
        [NoDuplicateStrings]
        [EachAllowedDestinationType]
        public List<string> DestinationTypes { get; set; } = [];

        [ListSize(1, 2, ErrorMessage = "Group sizes must be between 1 and 2 items long.")]
        [EachAllowedGroupSize]
        public List<string> GroupSizes { get; set; } = [];

        [ListSize(1, 10, ErrorMessage = "Activities must be between 1 and 10 items long.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Activities must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string> Activities { get; set; } = [];

        [ListSize(1, 10, ErrorMessage = "Tags must be between 1 and 10 items long.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Tags must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string> Tags { get; set; } = [];

        [ListSize(1, 3, ErrorMessage = "Trip durations must be between 1 and 3 items long.")]
        [EachAllowedTripDuration]
        public List<string> TripDurations { get; set; } = [];

        [ListSize(1, 1, ErrorMessage = "Price ranges must be 1 items long.")]
        [NoDuplicateStrings]
        [EachElementPriceRange]
        public List<string> PriceRanges { get; set; } = [];
    }
}
