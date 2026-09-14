using Tajawul.Interfaces.Trips;
using Tajawul.Models.Domain.General;
using Tajawul.Repositories;

namespace Tajawul.Services
{
    public class VisibilityService : IVisibilityService
    {
        private readonly VisibilityRepository _visibilityRepository;

        public VisibilityService(VisibilityRepository visibilityRepository)
        {

            _visibilityRepository = visibilityRepository;

        }
        public async Task<Visibility> AssignTripVisibilityAsync(string visibilityName, string tripId, string userId)
        {
            return await _visibilityRepository.AssignTripVisibilityAsync(visibilityName, tripId, userId);
        }
        public async Task<List<Visibility>> GetTripVisibilityAsync(string tripId, string userId)
        {
            return await _visibilityRepository.GetTripVisibilityAsync(tripId, userId);
        }
        public async Task<bool> DeleteTripVisibilityAsync(string tripId, string userId)
        {
            return await _visibilityRepository.DeleteTripVisibilityAsync(tripId, userId);
        }

        public async Task<Visibility> AssignVisibilityAsync(string visibilityName, string entityId, string relationName)
        {
            return await _visibilityRepository.AssignVisibilityAsync(entityId, visibilityName, relationName);
        }

        public async Task<Visibility> GetPostVisibilityAsync(string entityId, string relationName)
        {
            return await _visibilityRepository.GetEntityVisibilityAsync(entityId, relationName);
        }
    }
}
