namespace Tajawul.Interfaces.User.Interactions
{
    public interface IEventInteractionsService
    {
        Task<int> InterestedInEventAsync(string eventId, string userId);

        Task<int> NotInterestedInEventAsync(string eventId, string userId);

        Task<int> AttendEventAsync(string eventId, string userId);

        Task<int> UnAttendEventAsync(string eventId, string userId);

    }
}
