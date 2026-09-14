using Tajawul.Models.Domain.Events;

namespace Tajawul.Interfaces.Events
{
    public interface IEventDateRangeService
    {
        Task<EventDateRange> SetEventDatesAsync(string eventId, DateTime startOn, DateTime endOn);

    }
}
