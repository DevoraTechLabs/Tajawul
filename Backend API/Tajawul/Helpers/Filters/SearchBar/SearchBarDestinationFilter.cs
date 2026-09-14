using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Helpers.Filters.SearchBar
{
    public class SearchBarDestinationFilter
    {
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Type name must be between 3 and 20 characters.")]
        [Alphanumeric]
        public string? Type { get; set; }

        [PriceRange]
        public string? PriceRange { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "City name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? City { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? Country { get; set; }

        [ListSize(0, 10, ErrorMessage = "At most 10 tags allowed.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Tags must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string>? Tags { get; set; }

        [ListSize(0, 10, ErrorMessage = "At most 10 activities allowed.")]
        [AlphanumericStringList(3, 20, ErrorMessage = "Activities must be alphanumeric and between 3 and 20 characters long.")]
        [NoDuplicateStrings]
        public List<string>? Activities { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Group size must be between 3 and 20 characters.")]
        public string? GroupSize { get; set; }
    }
}