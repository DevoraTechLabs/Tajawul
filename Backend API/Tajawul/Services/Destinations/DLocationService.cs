using Tajawul.Interfaces.General;
using Tajawul.Models.Domain;
using Tajawul.Repositories.Destinations;


namespace Tajawul.Services.Destinations
{
    public class DLocationService : IDLocationService
    {
        private readonly DLocationRepository _dLocationRepository;

        public DLocationService(DLocationRepository dLocationRepository)
        {

            _dLocationRepository = dLocationRepository;

        }
        public async Task<City?> AssignLocationAsync(string destinationId, string countryName, string cityName)
        {
            return await _dLocationRepository.AssignLocationAsync(destinationId, countryName, cityName);
        }

        public async Task<City?> GetDestinationLocationAsync(string destinationId)
        {
            return await _dLocationRepository.GetDestinationLocationAsync(destinationId);
        }
        public async Task<bool> DeleteLocationAssignmentsAsync(string destinationId)
        {
            return await _dLocationRepository.DeleteLocationAssignmentsAsync(destinationId);
        }

        public async Task<City?> AssignEventLocationAsync(string eventId, string countryName, string cityName)
        {
            return await _dLocationRepository.AssignEventLocationAsync(eventId, countryName, cityName);
        }

        public async Task<City?> GetEventLocationAsync(string eventId)
        {
            return await _dLocationRepository.GetEventLocationAsync(eventId);
        }
        public async Task<bool> DeleteEventLocationAssignmentAsync(string eventId)
        {
            return await _dLocationRepository.DeleteEventLocationAssignmentsAsync(eventId);
        }
    }
}
