using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.General;
using Tajawul.Models.ViewModels.SearchBar;
using Tajawul.Repositories.Destinations;

namespace Tajawul.Services.Destinations
{
    public class ActivityService : IActivityService
    {
        private readonly ActivityRepository _activityRepository;

        public ActivityService(ActivityRepository activityRepository)
        {

            _activityRepository = activityRepository;

        }

        public async Task<Activity> AssignActivityAsync(string activityName, string destinationId, string userId)
        {
            return await _activityRepository.AssignActivityAsync(activityName, destinationId, userId);
        }

        public async Task<bool> DeleteDestinationActivityAsync(string activityName, string destinationId, string userId)
        {
            return await _activityRepository.DeleteDestinationActivityAsync(activityName, destinationId, userId);
        }

        public async Task<List<ActivityDto>> GetAllActivitiesAsync()
        {
            return await _activityRepository.GetAllActivitiesAsync();
        }

        public async Task<List<Activity>> GetDestinationActivitiesAsync(string destinationId)
        {
            return await _activityRepository.GetDestinationActivitiesAsync(destinationId);
        }
    }
}
