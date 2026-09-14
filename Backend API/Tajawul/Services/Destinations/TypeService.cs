using Tajawul.Interfaces.Destinations;
using Tajawul.Models.Domain.General;
using Tajawul.Models.ViewModels.SearchBar;
using Tajawul.Repositories;
using Tajawul.Repositories.Destinations;

namespace Tajawul.Services.Destinations
{
    public class TypeService : ITypeService
        {
        private readonly TypeRepository _typeRepository;

        public TypeService(TypeRepository typeRepository)
        {

                _typeRepository = typeRepository;

        }
        public async Task<Types> AssignTypeAsync(string typeName, string destinationId, string userId)
        {
            return await _typeRepository.AssignTypeAsync(typeName, destinationId, userId);
        }

        public async Task<List<Types>> GetDestinationTypesAsync(string destinationId, string userId)
        {
            return await _typeRepository.GetDestinationTypesAsync(destinationId, userId);
        }

        public async Task<bool> DeleteDestinationTypeAsync(string destinationId, string userId)
        {
            return await _typeRepository.DeleteDestinationTypeAsync(destinationId, userId);
        }

        public async Task<List<DestinationTypeDto>> GetAllDestinationTypesAsync()
        {
            return await _typeRepository.GetAllTypesAsync();
        }
    }
}
