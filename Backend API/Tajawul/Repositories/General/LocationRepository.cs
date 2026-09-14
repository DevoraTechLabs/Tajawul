using Neo4j.Driver;
using Tajawul.Helpers;
using Tajawul.Models.Domain;
using Tajawul.Models.DTOs.LocationDtos;
using Tajawul.Queries;
using Tajawul.Services;

namespace Tajawul.Repositories.General
{

    public class LocationRepository
    {
        private readonly Neo4jService _neo4jService;

        public LocationRepository(Neo4jService neo4jService)
        {
            _neo4jService = neo4jService;
        }


        // CREATE Methods
        public async Task<Country?> CreateCountryAsync(CountryDto country)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.CreateCountryQuery,
                new { name = country.Name },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new Country { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                }
            );
        }

        public async Task<City?> CreateCityAsync(CityDto city)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.CreateCityQuery,
                new { cityName = city.City, countryName = city.Country },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new City
                    {
                        Id = record.Single()["cityId"].As<string>(),
                        Name = record.Single()["cityName"].As<string>(),
                        Country = new Country
                        {
                            Id = record.Single()["countryId"].As<string>(),
                            Name = record.Single()["countryName"].As<string>()
                        }
                    };
                }
                );
        }

        // DELETE Methods
        public async Task<bool?> DeleteCountryAsync(string name)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.CountCountryLocatedInQuery,
             new { name },
             async result =>
             {
                 var record = await result.ToListAsync();
                 if (record.SingleOrDefault()?["relCount"].As<int>() > 0)
                 {
                     return false; // Node has relationships, refuse deletion
                 }

                 return await _neo4jService.ExecuteReadAsync(
                     LocationQueries.DeleteCountryQuery,
                     new { name },

                     async result =>
                     {
                         var record = await result.ToListAsync();
                         return record.SingleOrDefault() == null ? false : true;
                     }

                 );
             }
            );
        }

        public async Task<bool?> DeleteCityAsync(string cityName, string countryName)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.CountCityLocatedInQuery,
                new { cityName, countryName },
                async result =>
                {
                    var records = await result.ToListAsync(); // Get all records
                    var record = records.SingleOrDefault(); // Apply SingleOrDefault on the list

                    if (record["userCount"].As<int>() > 0)
                        return false; // Users are located in this city, cannot delete

                    // No users in the city, proceed with deleting relationships and city
                    return await _neo4jService.ExecuteReadAsync(
                        LocationQueries.DeleteCityQuery,
                        new { cityName, countryName },
                        async deleteResult =>
                        {
                            var deleteRecords = await deleteResult.ToListAsync(); // Get records
                            return deleteRecords.SingleOrDefault() != null; // Return true if deleted
                        }
                    );
                }
            );
        }


        //GET Methods
        public async Task<Country?> GetCountryByNameAsync(string name)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.GetCountryByNameQuery,
                new { name },
                 async result =>
                 {
                     var record = await result.ToListAsync();
                     return record.SingleOrDefault() == null ? null : new Country { Id = record.Single()["id"].As<string>(), Name = record.Single()["name"].As<string>() };
                 }
            );
        }

        public async Task<City?> GetCityByNameAsync(string cityName, string countryName)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.GetCityByNameQuery,
                new { cityName, countryName },
                 async result =>
                 {
                     var record = await result.ToListAsync();
                     return record.SingleOrDefault() == null ? null : new City
                     {
                         Id = record.Single()["id"].As<string>(),
                         Name = record.Single()["name"].As<string>(),
                         Country = new Country
                         {
                             Id = record.Single()["countryId"].As<string>(),
                             Name = record.Single()["countryName"].As<string>()
                         }
                     };
                 }
            );
        }


        public async Task<List<Country>> GetAllCountriesAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
               LocationQueries.GetAllCountriesQuery,
                new { },
            async result =>
            {
                var list = new List<Country>();
                await result.ForEachAsync(r => list.Add(new Country { Id = r["id"].As<string>(), Name = r["name"].As<string>() }));
                return list;
            }
            ) ?? [];
        }

        public async Task<List<City>> GetAllCitiesAsync()
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.GetAllCitiesQuery,
                new { },
                async result =>
                {
                    var list = new List<City>();
                    await result.ForEachAsync(r => list.Add(new City
                    {
                        Id = r["id"].As<string>(),
                        Name = r["name"].As<string>(),
                        Country = new Country
                        {
                            Id = r["countryId"].As<string>(),
                            Name = r["countryName"].As<string>()
                        }
                    }));
                    return list;
                }
            ) ?? [];
        }


        public async Task<City?> AddUserLocationAsync(string userId, string countryName, string cityName)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.AddUserLocationQuery,
                new { id = userId, countryName, cityName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    var singleRecord = record.SingleOrDefault();
                    if (singleRecord == null) return null;

                    return new City
                    {
                        Id = singleRecord["cityId"].As<string>(),
                        Name = singleRecord["city"].As<string>(),
                        Country = new Country
                        {
                            Id = singleRecord["countryId"].As<string>(),
                            Name = singleRecord["country"].As<string>()
                        }
                    };
                }
            );
        }


        public async Task<City?> GetUserLocationByIdAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.GetUserLocationQuery,
                new { id = userId },
                async result =>
                {
                    var record = await result.ToListAsync();
                    var singleRecord = record.SingleOrDefault();
                    if (singleRecord == null) return null;

                    return new City
                    {
                        Id = singleRecord["cityId"].As<string>(),
                        Name = singleRecord["city"].As<string>(),
                        Country = new Country
                        {
                            Id = singleRecord["countryId"].As<string>(),
                            Name = singleRecord["country"].As<string>()
                        }
                    };
                });
        }

        public async Task<bool> DeleteUserLocationAsync(string userId)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.DeleteUserLocation,
                 new { id = userId },
                 async result =>
                 {

                     var record = await result.ToListAsync();
                     return record.SingleOrDefault()?["relCount"].As<int>() > 0 ? true : false;
                 }

            );
        }
        // UPDATE Methods
        public async Task<Country?> UpdateCountryNameAsync(string oldName, string newName)
        {
            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.UpdateCountryName,
                new { oldName, newName },
                async result =>
                {
                    var record = await result.ToListAsync();
                    return record.SingleOrDefault() == null ? null : new Country
                    {
                        Id = record.Single()["id"].As<string>(),
                        Name = record.Single()["name"].As<string>()
                    };
                }
            );
        }


        public async Task<City?> UpdateCityNameAsync(string oldName, string newName, string countryName)
        {

            return await _neo4jService.ExecuteReadAsync(
                LocationQueries.UpdateCityName,
                 new { oldName, newName, countryName },
                 async result =>
                 {
                     var record = await result.ToListAsync();
                     return record.SingleOrDefault() == null ? null : new City
                     {
                         Id = record.Single()["id"].As<string>(),
                         Name = record.Single()["name"].As<string>(),
                         Country = new Country
                         {
                             Id = record.Single()["countryId"].As<string>(),
                             Name = record.Single()["countryName"].As<string>()
                         }
                     };
                 }
             );
        }



    }
}