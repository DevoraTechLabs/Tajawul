using Tajawul.Models.Domain.Events;

namespace Tajawul.Models.ViewModels.Event
{
    public class EventDto
    {
        public string EventId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string OrganizerId { get; set; }
        public DateTime CreationDate { get; set; }
        public int AttendeesCount { get; set; }
        public int InterestedInCount { get; set; }
        public DateTime LastEditDate { get; set; }
        public string? BookingUrl { get; set; }
        public double TicketPrice { get; set; }
        public List<EventLocation> Location { get; set; } = new List<EventLocation>();  // (lat, long, address)
        public string CoverImage { get; set; }
        public List<string> Images { get; set; } = new List<string>();
        public int MaxTicketsNumber { get; set; }
        public string? PriceRange { get; set; }
        public string? Status { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public DateTime? StartOn { get; set; }
        public DateTime? EndOn { get; set; }
    }
}

