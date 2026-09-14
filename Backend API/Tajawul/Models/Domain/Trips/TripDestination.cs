using Tajawul.Models.Domain.Destinations;

namespace Tajawul.Models.Domain.Trips
{
    public class TripDestination
    {
        public string DestinationId { get; set; }

        public int Day { get; set; }

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string CoverImage { get; set; } = null!;

        public List<string> Images { get; set; } = null!;

        public string City { get; set; } = null!;

        public string Country { get; set; } = null!;

        public List<DestinationLocation> Locations { get; set; } = new List<DestinationLocation>();

        public bool IsOpen24Hours { get; set; }

        public string OpenTime { get; set; } = null!;

        public string CloseTime { get; set; } = null!;

        public string PriceRange { get; set; } = null!;

        public float AverageRating { get; set; }

        public int ReviewsCount { get; set; }
    }
}