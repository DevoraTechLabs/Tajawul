using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Trips;
using Tajawul.Models.DTOs.Trip;
using Tajawul.Models.ViewModels.Trip;

namespace Tajawul.Mappers
{
    public static class TripMapper
    {
        public static TripDto ToTripDto(this Trip trip)
        {
            return new TripDto
            {
                TripId = trip.TripId,
                CoverImage = trip.CoverImage,
                CreationDate = trip.CreationDate,
                LastEditDate = trip.LastEditDate,
                Creator = trip.Creator,
                Description = trip.Description,
                Title = trip.Title,
                SameCountry = trip.SameCountry,
                WishesCount = trip.WishesCount,
                FavoritesCount = trip.FavoritesCount,
                ClonesCount = trip.ClonesCount,
                DestinationCount = trip.DestinationsCount,
                Destinations = trip.Destinations,
                Status = trip.Status,
                Visibility = trip.Visibility,
                TripDuration = trip.TripDuration,
                PriceRange = trip.PriceRange
            };
        }
        public static TripDestinationDto ToTripDestinationDto(this TripDestination tripDestination)
        {
            return new TripDestinationDto
            {
                Day = tripDestination.Day,
            };
        }

        public static TripFilter ToTripFilter(this TripHubFilter hubFilter)
        {
            return new TripFilter
            {
                TripId = hubFilter.TripId,
                Title = hubFilter.Title,
                Status = hubFilter.Status,
                TripDuration = hubFilter.TripDuration,
                PriceRange = hubFilter.PriceRange,
                Tag = hubFilter.Tag,
                CreationDate = hubFilter.CreationDate,
                LastEditDate = hubFilter.LastEditDate,
                CreatorId = hubFilter.CreatorId,
                sortBy = hubFilter.sortBy,
                Ascending = hubFilter.Ascending,
                PageNumber = hubFilter.PageNumber,
                PageSize = hubFilter.PageSize,

                Visibility = null
            };
        }
    }
}
