using Tajawul.Models.Domain.General;
using Tajawul.Models.ViewModels.SearchBar;
using static Tajawul.Helpers.GraphRelations;

namespace Tajawul.Interfaces.Destinations
{
    public interface ITypeService
    {
        Task<Types> AssignTypeAsync(string typeName, string destinationId, string userId);
        Task<List<Types>> GetDestinationTypesAsync(string destinationId, string userId);
        Task<List<DestinationTypeDto>> GetAllDestinationTypesAsync();
        Task<bool> DeleteDestinationTypeAsync(string destinationId, string userId);

    }
}
