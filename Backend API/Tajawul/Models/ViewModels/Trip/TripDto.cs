using Tajawul.Models.Domain.Trips;

namespace Tajawul.Models.ViewModels.Trip
{
    public class TripDto
    {
        public string TripId { get; set; } = null!;

        public required List<string> Creator { get; set; }

        public string Title { get; set; } = null!;

        public string PriceRange { get; set; } = null!;

        public string Status { get; set; } = null!;

        public string Visibility { get; set; } = null!;

        public string TripDuration { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string CoverImage { get; set; } = null!;

        public bool SameCountry { get; set; }

        public int WishesCount { get; set; }

        public int FavoritesCount { get; set; }

        public int ClonesCount { get; set; }

        public int DestinationCount { get; set; }

        public List<TripDestination> Destinations { get; set; } = new List<TripDestination>();

        public DateTime CreationDate { get; set; }

        public DateTime LastEditDate { get; set; }


    }
}