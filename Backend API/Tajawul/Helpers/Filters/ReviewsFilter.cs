using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Helpers.Filters
{
    public class ReviewsFilter
    {

        [StringLength(36, MinimumLength = 36, ErrorMessage = "Review not found")]
        public string? ReviewId { get; set; }

        [StringLength(36, MinimumLength = 36, ErrorMessage = "Destination not found")]
        public string? DestinationId { get; set; }

        [StringLength(36, MinimumLength = 36, ErrorMessage = "User not found")]
        public string? UserId { get; set; }

        public float? Rate { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "SortBy must be between 3 and 20 characters.")]
        public string sortBy { get; set; } = "date";

        public bool Ascending { get; set; } = true;

        private int _pageSize = 5;
        private int _pageNumber = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1")]
        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value > 0 ? value : 1; // Ensure it's at least 1
        }

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > 0 && value <= 100) ? value : 10; //Ensure is between 1-100
        }
    }
}
