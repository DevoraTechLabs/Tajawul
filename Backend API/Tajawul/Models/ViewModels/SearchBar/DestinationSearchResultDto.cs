namespace Tajawul.Models.ViewModels.SearchBar
{
    public class DestinationSearchResultDto
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public bool? IsVerified { get; set; }
        public int? VisitorsCount { get; set; }
        public int? FollowersCount { get; set; }
        public double? AverageRating { get; set; }
    }
}