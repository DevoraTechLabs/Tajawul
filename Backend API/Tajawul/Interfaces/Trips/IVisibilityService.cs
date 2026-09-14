using Tajawul.Models.Domain.General;

namespace Tajawul.Interfaces.Trips
{
    public interface IVisibilityService
    {
        Task<Visibility> AssignVisibilityAsync(string visibilityName, string entityId, string relationName);
        Task<Visibility> GetPostVisibilityAsync(string entityId, string relationName);

        Task<Visibility> AssignTripVisibilityAsync(string visibilityName, string tripId, string userId);
        Task<List<Visibility>> GetTripVisibilityAsync(string tripId, string userId);
        Task<bool> DeleteTripVisibilityAsync(string tripId, string userId);
    }
}
