using Tajawul.Interfaces.Events;
using Tajawul.Models.Domain.Events;
using Tajawul.Repositories.Events;

namespace Tajawul.Services
{
    public class EventDateRangeService : IEventDateRangeService
    {
        private readonly EventDateRangeRepository _eventDateRangeRepository;

        public EventDateRangeService(EventDateRangeRepository eventDateRangeRepository)
        {

            _eventDateRangeRepository = eventDateRangeRepository;

        }
        public async Task<EventDateRange> SetEventDatesAsync(string eventId, DateTime startOn, DateTime endOn)
        {
            return await _eventDateRangeRepository.SetEventDatesAsync(eventId, startOn, endOn);
        }

    }
}
