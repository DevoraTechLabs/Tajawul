using Tajawul.Models.DTOs.Event;
using Tajawul.Helpers.Filters;
using Tajawul.Models.ViewModels.Event;
using Tajawul.Models.Domain.Uploads;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Models.Domain.Events;

namespace Tajawul.Interfaces.Events
{
    public interface IEventService
    {
        Task<(Event eventEntity, List<string> failures)> CreateEventAsync(CreateEventDto eventDto, string destinationId);

        Task<(Event eventEntity, List <string> failures)> UpdateEventAsync(UpdateEventDto eventDto, string destinationId);

        Task<List<Event>> GetEventsAsync(EventFilter filter);

        Task<List<EventUserDto>> GetEventUsersAsync(EventUsersFilter usersFilter, string eventId);

        Task<bool> DeleteEventAsync(string organizerId, string eventId);

        Task<string> UpdateEventCoverImageAsync(IFormFile file, string eventId, string userId);

        Task<ImageUploadResult> UpdateEventImagesAsync(List<IFormFile> imageFiles, string eventId, string userId);

        Task<ImageUploadResult> DeleteEventImagesAsync(List<string> imageUrls, string eventId, string userId);

    }
}
