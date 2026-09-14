namespace Tajawul.Models.ViewModels.SearchBar
{
    public class SearchResponseDto
    {
        public List<UserSearchResultDto> Users { get; set; } = new();
        public List<DestinationSearchResultDto> Destinations { get; set; } = new();
        public List<TripSearchResultDto> Trips { get; set; } = new();
        public List<EventSearchResultDto> Events { get; set; } = new();
    }
}