namespace Tajawul.Models.ViewModels.user
{
    public class RecommendedUserDto
    {
        public required string UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ProfileImage { get; set; }
        public string? CityName { get; set; }
        public string? CountryName { get; set; }
        public int CreatedDestinationCount { get; set; }
        public int EditedDestinationCount { get; set; }
        public int VisitedDestinationCount { get; set; }
        public int FollowedDestinationCount { get; set; }
        public int CreatedTripCount { get; set; }
        public int ClonedTripCount { get; set; }
        public int PostsCount { get; set; }
    }
}
