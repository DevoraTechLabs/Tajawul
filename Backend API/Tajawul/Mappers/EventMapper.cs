using Tajawul.Models.Domain.Events;
using Tajawul.Models.DTOs.Event;
using Tajawul.Models.ViewModels.Event;

namespace Tajawul.Mappers
{
    public static class EventMapper
    {
        public static EventDto ToEventDto(this Event eventEntity)
        {
            return new EventDto
            {
                EventId = eventEntity.EventId,
                Name = eventEntity.Name,
                Description = eventEntity.Description,
                OrganizerId = eventEntity.OrganizerId,
                CreationDate = eventEntity.CreationDate,
                AttendeesCount = eventEntity.AttendeesCount,
                InterestedInCount = eventEntity.InterestedInCount,
                LastEditDate = eventEntity.LastEditDate,
                BookingUrl = eventEntity.BookingUrl,
                TicketPrice = eventEntity.TicketPrice,
                Location = eventEntity.Location,
                CoverImage = eventEntity.CoverImage,
                Images = eventEntity.Images,
                MaxTicketsNumber = eventEntity.MaxTicketsNumber,
                PriceRange = eventEntity.PriceRange,
                Status = eventEntity.Status,
                Country = eventEntity.Country,
                City = eventEntity.City,
                StartOn = eventEntity.StartOn,
                EndOn = eventEntity.EndOn
            };
        }
    }
}
