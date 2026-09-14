using Neo4j.Driver;
using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;
using Tajawul.Models.Domain.Destinations;
using Tajawul.Models.Domain.General;

namespace Tajawul.Models.DTOs.Destination
{
    public class UpdateDestinationDto : IValidatableObject
    {
        [Required(ErrorMessage = "Destination ID is required.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Destination not found")]
        public required string DestinationId { get; set; }


        [Required(ErrorMessage = "Destination name is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        [Alphanumeric]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters.")] // Increased max length
        public required string Description { get; set; }

        [Required(ErrorMessage = "Destination type is required.")]
        [DestinationTypeExists]
        public required string Type { get; set; }

        [Required(ErrorMessage = "Price range is required.")]
        [PriceRange]
        public required string PriceRange { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public required string Country { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "City name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public required string City { get; set; }

        [ListSize(1, 100, ErrorMessage = "At least one location and at most 100 locations are allowed.")]
        public List<DestinationLocation> Locations { get; set; } = [];

        [Required]
        public bool IsOpen24Hours { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly? OpenTime { get; set; }

        [DataType(DataType.Time)]
        public TimeOnly? CloseTime { get; set; }

        [PastDate]
        public DateOnly? EstablishedAt { get; set; }

        [ListSize(0, 10, ErrorMessage = "At most 10 social media links are allowed.")]
        [UniqueSocialMediaLinks]
        public List<SocialMediaLink>? SocialMediaLinks { get; set; } = [];


        [ListSize(0, 10, ErrorMessage = "At most 10 contact infos are allowed.")]
        public List<ContactInfo>? ContactInfo { get; set; } = [];

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1. Validate Open/Close Times if IsOpen24Hours is false
            if (!IsOpen24Hours)
            {
                if (!OpenTime.HasValue)
                {
                    yield return new ValidationResult(
                        "Open time is required when the destination is not open 24 hours.",
                        [nameof(OpenTime)]);
                }
                if (!CloseTime.HasValue)
                {
                    yield return new ValidationResult(
                        "Close time is required when the destination is not open 24 hours.",
                        [nameof(CloseTime)]);
                }
            }
            else // If open 24 hours, times should ideally be null
            {
                if (OpenTime.HasValue || CloseTime.HasValue)
                {
                    yield return new ValidationResult(
                       "Open and Close times should not be set if the destination is open 24 hours.",
                       [nameof(OpenTime), nameof(CloseTime)]);
                }
            }
        }

    }
}