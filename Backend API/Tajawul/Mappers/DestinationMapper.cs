using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.DTOs.Destination;
using Tajawul.Models.ViewModels.Destination;

namespace Tajawul.Mappers
{
    public static class DestinationMapper
    {
        public static DestinationDto ToDestinationDto(this Destination destination)
        {
            return new DestinationDto
            {
                DestinationId = destination.DestinationId,
                Name = destination.Name,
                Description = destination.Description,
                Type = destination.Type,
                PriceRange = destination.PriceRange,
                City = destination.City,
                Country = destination.Country,
                IsOpen24Hours = destination.IsOpen24Hours,
                OpenTime = destination.OpenTime,
                CloseTime = destination.CloseTime,
                CoverImage = destination.CoverImage,
                ContactInfo = destination.ContactInfo,
                AverageRating = destination.AverageRating,
                VisitorsCount = destination.VisitorsCount,
                ReviewsCount = destination.ReviewsCount,
                EventsCount = destination.EventsCount,
                FollowersCount = destination.FollowersCount,
                WishesCount = destination.WishesCount,
                FavoritesCount = destination.FavoritesCount,
                IsVerified = destination.IsVerified,
                Creator = destination.Creator,
                Images = destination.Images,
                Locations = destination.Locations,
                SocialMediaLinks = destination.SocialMediaLinks,
                EstablishedAt = destination.EstablishedAt,
                CreationDate = destination.CreationDate,
                LastEditDate = destination.LastEditDate
            };
        }
    }
}