using Microsoft.Extensions.Logging;
using Tajawul.Interfaces.User.Interactions;
using Tajawul.Repositories.User.Interaction;

namespace Tajawul.Services.User.Interactions
{
    public class EventInteractionsService : IEventInteractionsService
    {
        private readonly EventInteractionsRepository _eventInteractionsRepository;

        public EventInteractionsService(EventInteractionsRepository eventInteractionsRepository)
        {
            _eventInteractionsRepository = eventInteractionsRepository;
        }

        public async Task<int> InterestedInEventAsync(string eventId, string userId)
        {
            return await _eventInteractionsRepository.InterestedInEventAsync(eventId, userId);
        }

        public async Task<int> NotInterestedInEventAsync(string eventId, string userId)
        {
            return await _eventInteractionsRepository.NotInterestedInEventAsync(eventId, userId);
        }

        public async Task<int> AttendEventAsync(string eventId, string userId)
        {
            return await _eventInteractionsRepository.AttendEventAsync(eventId, userId);
        }

        public async Task<int> UnAttendEventAsync(string eventId, string userId)
        {
            return await _eventInteractionsRepository.UnattendEventAsync(eventId, userId);
        }

    }
}
