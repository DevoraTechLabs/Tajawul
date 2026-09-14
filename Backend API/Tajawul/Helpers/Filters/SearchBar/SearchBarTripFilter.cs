using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Helpers.Filters.SearchBar
{
    public class SearchBarTripFilter
    {
        [AllowedTripDurations]
        public string? TripDuration { get; set; }

        [PriceRange]
        public string? PriceRange { get; set; }

        [AllowedTripStatus]
        public string? Status { get; set; }

        [ListSize(0, 10, ErrorMessage = "At most 10 tags allowed.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Tags must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string>? Tags { get; set; }
    }
}