using System.ComponentModel.DataAnnotations;

namespace Tajawul.Helpers.Filters
{
    public class TripHubFilter
    {
        public string? TripId { get; set; }

        public string? Title { get; set; }

        public string? Status { get; set; }

        public string? TripDuration { get; set; }

        public string? PriceRange { get; set; }

        public string? Tag { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? LastEditDate { get; set; }

        public string? CreatorId { get; set; }

        public string sortBy { get; set; } = "CreationDate";

        public bool Ascending { get; set; } = true;

        [Range(1, int.MaxValue, ErrorMessage = "PageNumber must be at least 1")]
        public int PageNumber { get; set; } = 1;

        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be at least 1")]
        public int PageSize { get; set; } = 10;
    }
}
