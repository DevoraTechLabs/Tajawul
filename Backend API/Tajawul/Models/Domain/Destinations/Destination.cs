using Neo4j.Driver;
using Tajawul.Models.Domain.General;

namespace Tajawul.Models.Domain.Destinations
{
    public class Destination
    {
        public required string DestinationId { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public required string CoverImage { get; set; }

        public string? Type { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? PriceRange { get; set; }

        public bool IsVerified { get; set; }

        public required List<string> Creator { get; set; }              // (id, name, imageUrl)

        public bool IsOpen24Hours { get; set; }

        public TimeOnly? OpenTime { get; set; }

        public TimeOnly? CloseTime { get; set; }

        public float AverageRating { get; set; }

        public int VisitorsCount { get; set; }

        public int ReviewsCount { get; set; }

        public int EventsCount { get; set; }

        public int FollowersCount { get; set; }

        public int WishesCount { get; set; }

        public int FavoritesCount { get; set; }

        public List<string> Images { get; set; } = new List<string>();

        public DateOnly? EstablishedAt { get; set; }

        public List<DestinationLocation>? Locations { get; set; } = new List<DestinationLocation>();      // (long, lat, address)

        public List<SocialMediaLink>? SocialMediaLinks { get; set; }

        public List<ContactInfo>? ContactInfo { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastEditDate { get; set; }
    }
}