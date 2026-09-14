using Tajawul.Models.Domain;
using Tajawul.Models.DTOs.LocationDtos;
using Tajawul.Models.ViewModels;

namespace Tajawul.Interfaces.General
{
    public interface ILocationService
    {
        Task<Country?> CreateCountryAsync(CountryDto country);
        Task<City?> CreateCityAsync(CityDto city);
        Task<bool?> DeleteCountryAsync(string name);
        Task<bool?> DeleteCityAsync(string cityName, string countryName);
        Task<Country?> GetCountryByNameAsync(string Name);
        Task<City?> GetCityByNameAsync(string cityName, string countryName);
        Task<List<Country>> GetAllCountriesAsync();
        Task<List<City>> GetAllCitiesAsync();
        Task<Country?> UpdateCountryNameAsync(string oldName, string newName);
        Task<City?> UpdateCityNameAsync(string oldName, string newName, string countryName);
        Task<City?> AddUserLocationAsync(string userId, string countryName, string cityName);
        Task<City?> GetUserLocationAsync(string userId);
        Task<bool> DeleteUserLocationAsync(string userId);

    }
}
