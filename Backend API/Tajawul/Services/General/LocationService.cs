using Tajawul.Interfaces.General;
using Tajawul.Models.Domain;
using Tajawul.Models.DTOs.LocationDtos;
using Tajawul.Models.ViewModels;
using Tajawul.Repositories.General;

namespace Tajawul.Services
{
    public class LocationService(LocationRepository locationRepository) : ILocationService
    {
        private readonly LocationRepository _locationRepository = locationRepository;

        public async Task<Country?> CreateCountryAsync(CountryDto country)
        {
            return await _locationRepository.CreateCountryAsync(country);
        }

        public async Task<City?> CreateCityAsync(CityDto cityDto)
        {
            return await _locationRepository.CreateCityAsync(cityDto);
        }


        public async Task<bool?> DeleteCountryAsync(string name)
        {

            var country = await _locationRepository.GetCountryByNameAsync(name);

            if (country == null)
                return null;

            return await _locationRepository.DeleteCountryAsync(name);

        }

        public async Task<bool?> DeleteCityAsync(string oldName, string countryName)
        {

            var city = await _locationRepository.GetCityByNameAsync(oldName, countryName);

            if (city == null)
                return null;

            return await _locationRepository.DeleteCityAsync(oldName, countryName);

        }



        public async Task<Country?> GetCountryByNameAsync(string name)
        {
            return await _locationRepository.GetCountryByNameAsync(name);

        }

        public async Task<City?> GetCityByNameAsync(string cityName, string countryName)
        {
            return await _locationRepository.GetCityByNameAsync(cityName, countryName);
        }


        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _locationRepository.GetAllCountriesAsync();
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _locationRepository.GetAllCitiesAsync();
        }


        public async Task<Country?> UpdateCountryNameAsync(string oldName, string newName)
        {
            return await _locationRepository.UpdateCountryNameAsync(oldName, newName);
        }

        public async Task<City?> UpdateCityNameAsync(string oldName, string newName, string countryName)
        {
            return await _locationRepository.UpdateCityNameAsync(oldName, newName, countryName);
        }

        public async Task<City?> AddUserLocationAsync(string userId, string countryName, string cityName)
        {
            return await _locationRepository.AddUserLocationAsync(userId, countryName, cityName);
        }

        public async Task<bool> DeleteUserLocationAsync(string userId)
        {
            return await _locationRepository.DeleteUserLocationAsync(userId);
        }

        public async Task<City?> GetUserLocationAsync(string userId)
        {
            return await _locationRepository.GetUserLocationByIdAsync(userId);
        }


    }
}
