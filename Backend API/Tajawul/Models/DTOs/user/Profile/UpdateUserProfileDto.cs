using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;
using Tajawul.Models.Domain.General;

namespace Tajawul.Models.DTOs
{
    public class UpdateUserProfileDto
    {

        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters long.")]
        [Alphanumeric]
        public string? Username { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters long.")]
        [Alphanumeric]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters long.")]
        [Alphanumeric]
        public string? LastName { get; set; }

        [Required(ErrorMessage = "Bio is required")]
        [StringLength(1000, MinimumLength = 3, ErrorMessage = "Bio must be between 3 and 1000 characters long.")]
        public string? Bio { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Gender]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Marital Status is required")]
        [MaritalStatus]
        public string? MaritalStatus { get; set; }

        [Required(ErrorMessage = "Birth Date is required")]
        [PastDate(AllowToday = false)]
        public DateOnly? BirthDate { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? Country { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "City name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? City { get; set; }

        [Required(ErrorMessage = "Nationality is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Nationality must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? Nationality { get; set; }

        [ListSize(1, 20, ErrorMessage = "Spoken Languages must be between 1 and 20 languages.")]
        [NoDuplicateStrings]
        [AlphanumericStringList(3, 15, ErrorMessage = "Spoken Languages must be alphanumeric and between 3 and 15 characters long.")]
        public List<string> SpokenLanguages { get; set; } = [];

        [ListSize(0, 10, ErrorMessage = "Maximum of 10 social media links allowed.")]
        [UniqueSocialMediaLinks]
        public List<SocialMediaLink>? SocialMediaLinks { get; set; }


    }
}
