using Tajawul.Models.Domain.General;

namespace Tajawul.Interfaces.General
{
    public interface IGroupSizeService
    {

        Task<GroupSize> AssignGroupSizeAsync(string size, string destinationId, string userId);

        Task<bool> RemoveDestinationGroupSizeAsync(string size, string destinationId, string userId);

        Task<List<GroupSize>> GetDestinationGroupSizesAsync(string destinationId);

    }
}
