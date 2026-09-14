using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs;
using static Tajawul.Helpers.GraphRelations;

namespace Tajawul.Interfaces.General
{
    public interface ITagService
    {

        Task<List<string>> AssignTagsAsync(List<string> tagNames, string entityId, string relationName);

        Task<List<string>> GetEntityTagsAsync(string entityId, string relationName);

        Task<Tag> AssignDestinationTagAsync(string tagName, string destinationId, string userId);

        Task<List<Tag>> GetDestinationTagsAsync(string destinationId);

        Task<bool> DeleteDestinationTagAsync(string tagName, string destinationId, string userId);

        Task<Tag> AssignTripTagAsync(string tagName, string tripId, string userId);

        Task<List<Tag>> GetTripTagsAsync(string tripId, string userId);

        Task<bool> DeleteTripTagAsync(string tagName, string tripId, string userId);

        Task<Tag> AssignEventTagAsync(string tagName, string eventId, string destinationId);

        Task<List<Tag>> GetEventTagsAsync(string eventId);

        Task<bool> DeleteEventTagAsync(string tagName, string eventId, string destinationId);

        Task<List<TagDto>> GetAllTagsAsync();
    }
}
