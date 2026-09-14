using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;

namespace Tajawul.Helpers.Filters
{
    public class DestinationFilter
    {
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Destination not found")]
        public string? DestinationId { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        [Alphanumeric]
        public string? Name { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Type name must be between 3 and 20 characters.")]
        [Alphanumeric]
        public string? Type { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Tag name must be between 3 and 20 characters.")]
        [Alphanumeric]
        public string? Tag { get; set; }

        [StringLength(20, MinimumLength = 3, ErrorMessage = "Activity name must be between 3 and 20 characters.")]
        [Alphanumeric]
        public string? Activity { get; set; }

        [AllowedGroupSizes]
        public string? GroupSize { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "City name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? City { get; set; }

        [StringLength(30, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? Country { get; set; }

        public bool? IsOpen24Hours { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly? OpenTime { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly? CloseTime { get; set; }

        //[PriceRange]
        public string? PriceRange { get; set; }

        [Range(0, 5, ErrorMessage = "AverageRating must be between 0 and 5")]
        public float? AverageRating { get; set; }

        public bool? IsVerified { get; set; }

        [StringLength(36, MinimumLength = 36, ErrorMessage = "Creator not found")]
        public string? CreatorId { get; set; }

        [FutureDate]
        public DateOnly? EstablishedAt { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? CreationDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastEditDate { get; set; }
        [StringLength(20, MinimumLength = 3, ErrorMessage = "SortBy must be between 3 and 20 characters.")]
        public string sortBy { get; set; } = "CreationDate";

        public bool Ascending { get; set; } = true;

        private int _pageSize = 10;
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