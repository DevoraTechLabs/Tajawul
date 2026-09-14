namespace Tajawul.Models.ViewModels.SearchBar
{
    public class TripSearchResultDto
    {
        public string? Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public int? CloneCount { get; set; }
        public int? FavoriteCount { get; set; }
        public int? WishedCount { get; set; }
    }
}