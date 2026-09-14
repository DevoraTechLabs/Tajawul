using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.General;
using Tajawul.Repositories.Destinations;

namespace Tajawul.Services.Destinations
{
    public class PriceRangeService : IPriceRangeService
    {
        private readonly PriceRangeRepository _priceRangeRepository;

        public PriceRangeService(PriceRangeRepository priceRangeRepository)
        {
            _priceRangeRepository = priceRangeRepository;
        }

        public async Task<List<PriceRange>> GetDestinationPriceRangesAsync(string destinationId, string userId)
        {
            return await _priceRangeRepository.GetDestinationPriceRangesAsync(destinationId, userId);
        }

        public async Task<PriceRange> AssignPriceRangeAsync(string range, string destinationId, string userId)
        {
            return await _priceRangeRepository.AssignPriceRangeAsync(range, destinationId, userId);
        }

        public async Task<bool> RemoveDestinationPriceRangeAsync(string range, string destinationId, string userId)
        {
            return await _priceRangeRepository.RemoveDestinationPriceRangeAsync(range, destinationId, userId);
        }

        public async Task<List<PriceRange>> GetEventPriceRangesAsync(string eventId, string destinationId)
        {
            return await _priceRangeRepository.GetEventPriceRangesAsync(eventId, destinationId);
        }

        public async Task<PriceRange> AssignPriceRangeToEventAsync(string range, string eventId, string destinationId)
        {
            return await _priceRangeRepository.AssignPriceRangeToEventAsync(range, eventId, destinationId);
        }

        public async Task<bool> RemoveEventPriceRangeAsync(string range, string eventId, string destinationId)
        {
            return await _priceRangeRepository.RemoveEventPriceRangeAsync(range, eventId, destinationId);
        }

        public async Task<List<PriceRange>> GetTripPriceRangesAsync(string tripId, string userId)
        {
            return await _priceRangeRepository.GetTripPriceRangesAsync(tripId, userId);
        }
        public async Task<PriceRange> AssignTripPriceRangeAsync(string range, string tripId, string userId)
        {
            return await _priceRangeRepository.AssignTripPriceRangeAsync(range, tripId, userId);
        }
        public async Task<bool> RemoveTripPriceRangeAsync(string range, string tripId, string userId)
        {
            return await _priceRangeRepository.RemoveTripPriceRangeAsync(range, tripId, userId);
        }
    }
}
