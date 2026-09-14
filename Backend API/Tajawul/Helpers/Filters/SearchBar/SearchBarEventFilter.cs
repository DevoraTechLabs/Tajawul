using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Helpers.Filters.SearchBar
{
    public class SearchBarEventFilter
    {
        [PriceRange]
        public string? PriceRange { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "City name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? City { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? Country { get; set; }

        [AllowedEventStatus]
        public string? Status { get; set; }

        [ListSize(0, 10, ErrorMessage = "At most 10 tags allowed.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Tags must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string>? Tags { get; set; }
    }
}