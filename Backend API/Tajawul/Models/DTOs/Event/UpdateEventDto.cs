using System.ComponentModel.DataAnnotations;
using Tajawul.Helpers.ValidationAttributes;
using Tajawul.Models.Domain;
using Tajawul.Models.Domain.Events;

namespace Tajawul.Models.DTOs.Event
{
    public class UpdateEventDto : IValidatableObject
    {

        [Required(ErrorMessage = "Destination ID is required.")]
        [StringLength(36, MinimumLength = 36, ErrorMessage = "Destination not found.")]
        public string? EventId { get; set; }

        [Required(ErrorMessage = "Event name is required.")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 250 characters.")]
        [Alphanumeric]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Booking URL is required.")]
        [Url(ErrorMessage = "Please provide a valid Booking URL (e.g., https://example.com).")]
        [StringLength(2048, ErrorMessage = "Booking URL cannot exceed 2048 characters.")]
        public required string BookingUrl { get; set; }

        [Required(ErrorMessage = "Ticket price is required.")]
        [Range(0, double.MaxValue, ErrorMessage = "Ticket price must be a non-negative value.")]
        public required double TicketPrice { get; set; }

        [ListSize(1, 100, ErrorMessage = "Event must have at least one location and up to 100 locations.")]
        public List<EventLocation> Location { get; set; } = [];

        [Required(ErrorMessage = "Cover image URL is required.")]
        [Url(ErrorMessage = "Please provide a valid Cover Image URL (e.g., https://example.com/image.png).")]
        [StringLength(2048, ErrorMessage = "Cover Image URL cannot exceed 2048 characters.")]
        public required string CoverImage { get; set; }

        [ListSize(0, 5, ErrorMessage = "Event must have at most 5 images.")]
        [ListUrlValidate]
        [NoDuplicateStrings]
        public List<string>? Images { get; set; }

        [Required(ErrorMessage = "Maximum number of tickets is required.")]
        [Range(0, 1000000, ErrorMessage = "Maximum tickets must be between 0 and 1,000,000.")]
        public required int MaxTicketsNumber { get; set; }

        [Required(ErrorMessage = "Price range is required.")]
        [PriceRange]
        public required string PriceRange { get; set; }

        [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
        [AllowedEventStatus]
        public string? Status { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "Country name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? Country { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(30, MinimumLength = 3, ErrorMessage = "City name must be between 3 and 30 characters.")]
        [Alphanumeric]
        public string? City { get; set; }

        [FutureDate]
        public DateTime? StartOn { get; set; }

        [FutureDate]
        public DateTime? EndOn { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // 1. Validate EndOn is after StartOn
            if (StartOn.HasValue && EndOn.HasValue && EndOn.Value <= StartOn.Value)
            {
                yield return new ValidationResult(
                    "End date must be after the start date.",
                    [nameof(EndOn), nameof(StartOn)]);
            }


            if ((StartOn.HasValue && !EndOn.HasValue) || (!StartOn.HasValue && EndOn.HasValue))
            {
                yield return new ValidationResult("Both Start Date and End Date are required if one is specified.", [nameof(StartOn), nameof(EndOn)]);
            }
        }
    }

}
