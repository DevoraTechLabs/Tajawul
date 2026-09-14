using Tajawul.Helpers.Filters.SearchBar;
using Tajawul.Models.ViewModels.SearchBar;

namespace Tajawul.Interfaces
{
    public interface ISearchService
    {
       Task<SearchResponseDto> GeneralSearchAsync(string query, string mode);
        Task<List<UserSearchResultDto>> SearchUsersAsync(string query, string mode, int limit);
        Task<List<DestinationSearchResultDto>> SearchDestinationsAsync(string query, SearchBarDestinationFilter? filters, string mode, int limit);
        Task<List<TripSearchResultDto>> SearchTripsAsync(string query, SearchBarTripFilter? filters, string mode, int limit);
        Task<List<EventSearchResultDto>> SearchEventsAsync(string query, SearchBarEventFilter? filters, string mode, int limit);
    }
}