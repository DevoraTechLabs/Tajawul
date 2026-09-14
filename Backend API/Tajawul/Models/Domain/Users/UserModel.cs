using Tajawul.Models.Domain.General;

namespace Tajawul.Models.Domain.Users
{
    public class UserModel
    {
        public required string UserId { get; set; }

        public required string Username { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Bio { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public string? MaritalStatus { get; set; }

        public DateOnly? BirthDate { get; set; }

        public string? Country { get; set; }

        public string? City { get; set; }

        public string? Nationality { get; set; }

        public bool IsTopTraveler { get; set; }

        public string? ProfileImage { get; set; }

        public List<string> SpokenLanguages { get; set; } = new List<string>();

        public List<SocialMediaLink>? SocialMediaLinks { get; set; }

        public int CreatedDestinationCount { get; set; }

        public int EditedDestinationCount { get; set; }

        public int WishedDestinationCount { get; set; }

        public int FavoriteDestinationCount { get; set; }

        public int VisitedDestinationCount { get; set; }

        public int FollowedDestinationCount { get; set; }

        public int FollowersCount { get; set; }

        public int FollowingsCount { get; set; }

        public bool IsFollowedByCurrentUser { get; set; }

        public int CreatedTripCount { get; set; }

        public int WishedTripCount { get; set; }

        public int FavoriteTripCount { get; set; }

        public int ClonedTripCount { get; set; }

        public int PostsCount { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime LastEditDate { get; set; }

        public UserInterests Interests { get; set; } = new UserInterests();
    }
}
