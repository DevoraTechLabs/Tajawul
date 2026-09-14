using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.General;
using Tajawul.Repositories.Destinations;

namespace Tajawul.Services.Destinations
{
    public class GroupSizeService: IGroupSizeService
    {
        private readonly GroupSizeRepository _groupSizeRepository;

        public GroupSizeService(GroupSizeRepository groupSizeRepository)
        {

            _groupSizeRepository = groupSizeRepository;

        }

        public async Task<List<GroupSize>> GetDestinationGroupSizesAsync(string destinationId)
        {
            return await _groupSizeRepository.GetDestinationGroupSizesAsync(destinationId);
        }

        public async Task<GroupSize> AssignGroupSizeAsync(string size, string destinationId, string userId)
        {
            return await _groupSizeRepository.AssignGroupSizeAsync(size, destinationId, userId);
        }

        public async Task<bool> RemoveDestinationGroupSizeAsync(string size, string destinationId, string userId)
        {
            return await _groupSizeRepository.RemoveDestinationGroupSizeAsync(size, destinationId, userId);
        }
    }
}
