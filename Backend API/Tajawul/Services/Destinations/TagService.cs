using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.General;
using Tajawul.Models.DTOs;
using Tajawul.Repositories.Destinations;
using static Tajawul.Helpers.GraphRelations;

namespace Tajawul.Services.Destinations
{
    public class TagService : ITagService
    {
        private readonly TagRepository _tagRepository;

        public TagService(TagRepository tagRepository)
        {

            _tagRepository = tagRepository;

        }

        public async Task<Tag> AssignDestinationTagAsync(string tagName, string destinationId, string userId)
        {
            return await _tagRepository.AssignDestinationTagAsync(tagName, destinationId, userId);
        }

        public async Task<List<Tag>> GetDestinationTagsAsync(string destinationId)
        {
            return await _tagRepository.GetDestinationTagsAsync(destinationId);
        }

        public async Task<bool> DeleteDestinationTagAsync(string tagName, string destinationId, string userId)
        {
            return await _tagRepository.DeleteDestinationTagAsync(tagName, destinationId, userId);
        }

        public async Task<Tag> AssignTripTagAsync(string tagName, string tripId, string userId)
        {
            return await _tagRepository.AssignTripTagAsync(tagName, tripId, userId);
        }

        public async Task<List<Tag>> GetTripTagsAsync(string tripId, string userId)
        {
            return await _tagRepository.GetTripTagsAsync(tripId, userId);
        }

        public async Task<bool> DeleteTripTagAsync(string tagName, string tripId, string userId)
        {
            return await _tagRepository.DeleteTripTagAsync(tagName, tripId, userId);
        }

        public async Task<Tag> AssignEventTagAsync(string tagName, string eventId, string destinationId)
        {
            return await _tagRepository.AssignEventTagAsync(tagName, eventId, destinationId);
        }

        public async Task<List<Tag>> GetEventTagsAsync(string eventId)
        {
            return await _tagRepository.GetEventTagsAsync(eventId);
        }

        public async Task<bool> DeleteEventTagAsync(string tagName, string eventId, string destinationId)
        {
            return await _tagRepository.DeleteEventTagAsync(tagName, eventId, destinationId);
        }

        public async Task<List<TagDto>> GetAllTagsAsync()
        {
            return await _tagRepository.GetAllTagsAsync();
        }

        public async Task<List<string>> AssignTagsAsync(List<string> tagNames, string entityId, string relationName)
        {
            return await _tagRepository.AssignTagsAsync(tagNames, entityId, relationName);
        }

        public async Task<List<string>> GetEntityTagsAsync(string entityId, string relationName)
        {
            return await _tagRepository.GetEntityTagsAsync(entityId, relationName);
        }
    }
}
