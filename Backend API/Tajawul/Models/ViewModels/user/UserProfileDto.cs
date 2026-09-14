namespace Tajawul.Models.ViewModels.user
{
    public class UserProfileDto
    {
        // Personal information
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public DateOnly? BirthDate { get; set; }
        public string? profileImage { get; set; }
        public List<string>? SocialMediaLinks { get; set; }
        public string? CityName { get; set; }
        public string? CountryName { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatusName { get; set; }
        public string? DestinationTypeName { get; set; }
        public string? GroupSizeName { get; set; }
        public string? TripDurationName { get; set; }
        public string? PriceRangeName { get; set; }

        // Lists of user preferences
        public List<string>? SpokenLanguageNamesList { get; set; }
        public List<string>? InterestNamesList { get; set; }
        public List<string>? TagNamesList { get; set; }
        public List<string>? ActivityNamesList { get; set; }

        // User statistics
        public int CreatedDestinationCount { get; set; }
        public int EditedDestinationCount { get; set; }
        public int WishedDestinationCount { get; set; }
        public int FavoriteDestinationCount { get; set; }
        public int VisitedDestinationCount { get; set; }
        public int FollowedDestinationCount { get; set; }
        public int CreatedTripCount { get; set; }
        public int WishedTripCount { get; set; }
        public int FavoriteTripCount { get; set; }
        public int ClonedTripCount { get; set; }
        public int PostsCount { get; set; }

        // 
        public DateTime? CreationDate { get; set; }
        public DateTime? LastEditDate { get; set; }
    }
}

// Now your UserProfileDto holds both the statistical data and all the personal details from UpdateUserProfileDto.
// Let me know if you’d like me to tweak anything or add more fields! 🚀
