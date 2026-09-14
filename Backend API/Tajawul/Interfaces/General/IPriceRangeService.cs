using Tajawul.Models.Domain.General;

namespace Tajawul.Interfaces.General
{
    public interface IPriceRangeService
    {

        Task<PriceRange> AssignPriceRangeAsync(string range, string destinationId, string userId);

        Task<bool> RemoveDestinationPriceRangeAsync(string range, string destinationId, string userId);

        Task<List<PriceRange>> GetDestinationPriceRangesAsync(string destinationId, string userId);

        Task<List<PriceRange>> GetEventPriceRangesAsync(string eventId, string destinationId);

        Task<PriceRange> AssignPriceRangeToEventAsync(string range, string eventId, string destinationId);

        Task<bool> RemoveEventPriceRangeAsync(string range, string eventId, string destinationId);
        Task<List<PriceRange>> GetTripPriceRangesAsync(string tripId, string userId);
        Task<PriceRange> AssignTripPriceRangeAsync(string range, string tripId, string userId);
        Task<bool> RemoveTripPriceRangeAsync(string range, string tripId, string userId);
    }
}
