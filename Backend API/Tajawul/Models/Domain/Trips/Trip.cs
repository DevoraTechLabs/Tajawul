namespace Tajawul.Models.Domain.Trips
{
    public class Trip
    {
        public required string TripId { get; set; }

        public required string Title { get; set; }

        public required string Description { get; set; }

        public string CoverImage { get; set; }

        public string? PriceRange { get; set; }

        public string? Status { get; set; }

        public string? Visibility { get; set; }

        public string? TripDuration { get; set; }

        public bool SameCountry { get; set; }

        public required List<string> Creator { get; set; } // (id, name, imageUrl)

        public int WishesCount { get; set; }

        public int FavoritesCount { get; set; }

        public int ClonesCount { get; set; }

        public int DestinationsCount { get; set; }

        public List<TripDestination> Destinations { get; set; } = new List<TripDestination>();

        public DateTime CreationDate { get; set; }

        public DateTime LastEditDate { get; set; }
    }
}
