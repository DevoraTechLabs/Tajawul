using Tajawul.Models.Domain;

namespace Tajawul.Interfaces.General
{
    public interface IDLocationService
    {
        Task<City?> AssignLocationAsync(string destinationId, string countryName, string cityName);

        Task<City?> GetDestinationLocationAsync(string destinationId);

        Task<bool> DeleteLocationAssignmentsAsync(string destinationId);

        Task<City?> AssignEventLocationAsync(string eventId, string countryName, string cityName);

        Task<City?> GetEventLocationAsync(string eventId);

        Task<bool> DeleteEventLocationAssignmentAsync(string eventId);

    }
}
