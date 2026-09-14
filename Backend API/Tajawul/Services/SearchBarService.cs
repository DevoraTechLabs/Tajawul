using Tajawul.Helpers.Filters.SearchBar;
using Tajawul.Interfaces;
using Tajawul.Models.ViewModels.SearchBar;
using Tajawul.Repositories;

namespace Tajawul.Services
{
    public class SearchService : ISearchService
    {
        private readonly SearchBarRepository _searchRepository;

        public SearchService(SearchBarRepository searchRepository)
        {
            _searchRepository = searchRepository;
        }

        private string FormatQuery(string query, string mode)
        {
            query = query?.Trim().ToLower() ?? string.Empty;

            return mode.ToLower() switch
            {
                "wildcard" => $"*{query}*",
                "fuzzy" => $"{query}~0.5",
                _ => $"*{query}*" // Default to wildcard
            };
        }

        public async Task<SearchResponseDto> GeneralSearchAsync(string query, string mode)
        {
            var results = new SearchResponseDto();

            results.Users = await SearchUsersAsync(query, mode);
            results.Destinations = await SearchDestinationsAsync(query, null, mode);
            results.Trips = await SearchTripsAsync(query, null, mode);
            results.Events = await SearchEventsAsync(query, null, mode);

            // Limit results to 5 per category for general search
            results.Users = results.Users.Take(5).ToList();
            results.Destinations = results.Destinations.Take(5).ToList();
            results.Trips = results.Trips.Take(5).ToList();
            results.Events = results.Events.Take(5).ToList();

            return results;
        }

        public async Task<List<UserSearchResultDto>> SearchUsersAsync(string query, string mode, int limit = 5)
        {
            query = FormatQuery(query, mode);
            return await _searchRepository.SearchUsersAsync(query, limit);
        }
        public async Task<List<DestinationSearchResultDto>> SearchDestinationsAsync(string query, SearchBarDestinationFilter? filters, string mode, int limit = 5)
        {
            query = FormatQuery(query, mode);
            return await _searchRepository.SearchDestinationsAsync(query, filters, limit);
        }

        public async Task<List<TripSearchResultDto>> SearchTripsAsync(string query, SearchBarTripFilter? filters, string mode, int limit = 5)
        {
            query = FormatQuery(query, mode);
            return await _searchRepository.SearchTripsAsync(query, filters, limit);
        }

        public async Task<List<EventSearchResultDto>> SearchEventsAsync(string query, SearchBarEventFilter? filters, string mode, int limit = 5)
        {
            query = FormatQuery(query, mode);
            return await _searchRepository.SearchEventsAsync(query, filters, limit);
        }
    }
}