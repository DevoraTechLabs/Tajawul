using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.General;
using Tajawul.Repositories;

namespace Tajawul.Services
{
    public class StatusService : IStatusService
    {
        private readonly StatusRepository _statusRepository;

        public StatusService(StatusRepository statusRepository)
        {

            _statusRepository = statusRepository;

        }

        public async Task<Status> AssignTripStatusAsync(string tripId, string statusName, string userId)
        {
            return await _statusRepository.AssignTripStatusAsync(tripId, statusName, userId);
        }
        public async Task<List<Status>> GetTripStatusesAsync(string tripId, string userId)
        {
            return await _statusRepository.GetTripStatusesAsync(tripId, userId);
        }
        public async Task<bool> DeleteTripStatusAsync(string tripId, string userId)
        {
            return await _statusRepository.DeleteTripStatusAsync(tripId, userId);
        }

        public async Task<Status> AssignEventStatusAsync(string eventId, string statusName, string destinationId)
        {
            return await _statusRepository.AssignEventStatusAsync(eventId, statusName, destinationId);
        }

        public async Task<List<Status>> GetEventStatusesAsync(string eventId, string destinationId)
        {
            return await _statusRepository.GetEventStatusesAsync(eventId, destinationId);
        }

        public async Task<bool> DeleteEventStatusAsync(string eventId, string destinationId)
        {
            return await _statusRepository.DeleteEventStatusAsync(eventId, destinationId);
        }

    }
}
        


