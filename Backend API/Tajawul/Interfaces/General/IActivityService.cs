using Tajawul.Models.Domain.General;
using Tajawul.Models.ViewModels.SearchBar;

namespace Tajawul.Interfaces.General
{
    public interface IActivityService
    {
        Task<Activity> AssignActivityAsync(string activityName, string destinationId, string userId);

        Task<bool> DeleteDestinationActivityAsync(string activityName, string destinationId, string userId);

        Task<List<Activity>> GetDestinationActivitiesAsync(string destinationId);
        Task<List<ActivityDto>> GetAllActivitiesAsync();
    }
}
