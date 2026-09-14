using Tajawul.Models.Domain.General;

namespace Tajawul.Interfaces.General
{
    public interface IStatusService
    {

        Task<Status> AssignEventStatusAsync(string eventId, string statusName, string destinationId);

        Task<List<Status>> GetEventStatusesAsync(string eventId, string destinationId);

        Task<bool> DeleteEventStatusAsync(string eventId, string destinationId);

        Task<Status> AssignTripStatusAsync(string tripId, string statusName, string userId);

        Task<List<Status>> GetTripStatusesAsync(string tripId, string userId);

        Task<bool> DeleteTripStatusAsync(string tripId, string userId);

    }
}
