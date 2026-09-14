using Tajawul.Models.DTOs.Event;
using Tajawul.Models.ViewModels.Event;
using Tajawul.Interfaces.Events;
using Tajawul.Repositories.Events;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tajawul.Models.ViewModels.Destination;
using Tajawul.Repositories.Destinations;
using Neo4j.Driver;
using static System.Net.Mime.MediaTypeNames;
using Tajawul.Services.Destinations;
using Tajawul.Helpers.Filters;
using Tajawul.Models.Domain.Uploads;
using Tajawul.Interfaces.Media;
using Tajawul.Interfaces.General;
using Tajawul.Models.Domain.Events;

namespace Tajawul.Services.Events
{
    public class EventService : IEventService
    {
        private readonly EventRepository _eventRepository;
        private readonly IPriceRangeService _priceRangeService;
        private readonly IStatusService _statusService;
        private readonly IDLocationService _dLocationService;
        private readonly IEventDateRangeService _eventDateRangeService;
        private readonly IAzureStorageService _azureStorageService;

        public EventService(
            EventRepository eventRepository,
            IPriceRangeService priceRangeService,
            IStatusService statusService,
            IDLocationService dLocationService,
            IEventDateRangeService eventDateRangeService,
            IAzureStorageService azureStorageService)
        {
            _eventRepository = eventRepository;
            _priceRangeService = priceRangeService;
            _statusService = statusService;
            _dLocationService = dLocationService;
            _eventDateRangeService = eventDateRangeService;
            _azureStorageService = azureStorageService;
        }

        public async Task<(Event eventEntity, List<string> failures)> CreateEventAsync(CreateEventDto eventDto, string destinationId)
        {
            if (eventDto.TicketPrice < 0)
            {
                throw new ArgumentException("Ticket price cannot be negative.");
            }

            if (eventDto.MaxTicketsNumber <= 0)
            {
                throw new ArgumentException("Max tickets must be greater than zero.");
            }

            if (eventDto.StartOn.HasValue && eventDto.EndOn.HasValue &&eventDto.StartOn > eventDto.EndOn)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            List<string> failures = new();
            Event eventEntity = await _eventRepository.CreateEventAsync(eventDto, destinationId);

            try
            {
                var priceRange = await _priceRangeService.AssignPriceRangeToEventAsync(
                            eventDto.PriceRange,
                            eventEntity.EventId,
                            destinationId);
                eventEntity.PriceRange = priceRange.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Price range assignment failed: {ex.Message}");
            }

            try
            {
                var status = await _statusService.AssignEventStatusAsync(
                    eventEntity.EventId,
                    eventDto.Status,
                    destinationId
                );
                eventEntity.Status = status.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign status: {ex.Message}");
            }

            try
            {
                var city = await _dLocationService.AssignEventLocationAsync(
                    eventEntity.EventId,
                    eventDto.Country,
                    eventDto.City
                );
                eventEntity.City = city.Name;
                eventEntity.Country = city.Country.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign location: {ex.Message}");
            }

            try
            {
                var date = await _eventDateRangeService.SetEventDatesAsync(
                   eventEntity.EventId,
                   eventDto.StartOn.Value,
                   eventDto.EndOn.Value

                );
                eventEntity.StartOn = date.StartOn;
                eventEntity.EndOn = date.EndOn;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign date range: {ex.Message}");
            }

            return (eventEntity, failures);
        }

        public async Task<(Event eventEntity, List<string> failures)> UpdateEventAsync(UpdateEventDto eventDto, string destinationId)
        {
            if (eventDto.TicketPrice < 0)
            {
                throw new ArgumentException("Ticket price cannot be negative.");
            }

            if (eventDto.MaxTicketsNumber <= 0)
            {
                throw new ArgumentException("Max tickets must be greater than zero.");
            }

            if (eventDto.StartOn.HasValue && eventDto.EndOn.HasValue && eventDto.StartOn > eventDto.EndOn)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            List<string> failures = new();
            Event eventEntity = await _eventRepository.UpdateEventAsync(eventDto, destinationId);

            try
            {
                var priceRange = await _priceRangeService.AssignPriceRangeToEventAsync(
                            eventDto.PriceRange,
                            eventEntity.EventId,
                            destinationId);
                eventEntity.PriceRange = priceRange.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Price range assignment failed: {ex.Message}");
            }

            try
            {
                var status = await _statusService.AssignEventStatusAsync(
                    eventEntity.EventId,
                    eventDto.Status,
                    destinationId
                );
                eventEntity.Status = status.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign status: {ex.Message}");
            }

            try
            {
                var city = await _dLocationService.AssignEventLocationAsync(
                    eventEntity.EventId,
                    eventDto.Country,
                    eventDto.City
                );
                eventEntity.City = city.Name;
                eventEntity.Country = city.Country.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign location: {ex.Message}");
            }

            try
            {
                var date = await _eventDateRangeService.SetEventDatesAsync(
                   eventEntity.EventId,
                   eventDto.StartOn.Value,
                   eventDto.EndOn.Value

                );
                eventEntity.StartOn = date.StartOn;
                eventEntity.EndOn = date.EndOn;
            }
            catch (Exception ex)
            {
                failures.Add($"Failed to assign date range: {ex.Message}");
            }

            return (eventEntity, failures);
        }

        public async Task<List<Event>> GetEventsAsync(EventFilter filter)
        {

            if (filter.StartOn.HasValue && filter.EndOn.HasValue &&
                filter.StartOn.Value > filter.EndOn.Value)
            {
                throw new ArgumentException("Start date cannot be after end date.");
            }

            if (filter.TicketPrice.HasValue && filter.TicketPrice.Value < 0)
            {
                throw new ArgumentException("Ticket price cannot be negative.");
            }

            return await _eventRepository.GetEventsAsync(filter);
        }

        public async Task<List<EventUserDto>> GetEventUsersAsync(EventUsersFilter usersFilter, string eventId)
        {
            return await _eventRepository.GetEventUsersAsync(usersFilter, eventId);
        }

        public async Task<bool> DeleteEventAsync(string organizerId, string eventId)
        {
            return await _eventRepository.DeleteEventAsync(organizerId, eventId);
        }

        public async Task<string> UpdateEventCoverImageAsync(IFormFile file, string eventId, string userId)
        {
            string imageUrl;

            try
            {
                imageUrl = await _azureStorageService.UploadImageAsync(file, "events", eventId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload image to storage.", ex);
            }

            try
            {
                await _eventRepository.UpdateEventCoverImageAsync(imageUrl, eventId, userId);
                return imageUrl;
            }
            catch (Exception ex)
            {
                try
                {
                    await _azureStorageService.DeleteImagesAsync([imageUrl]);
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete the file.", ex);
                }
                throw new Exception("Failed to update event cover image.", ex);
            }
        }

        public async Task<ImageUploadResult> UpdateEventImagesAsync(List<IFormFile> imageFiles, string eventId, string userId)
        {

            var existingImageCount = await _eventRepository.GetImageCountForEventAsync(eventId);
            int newImageCount = imageFiles.Count;

            if (existingImageCount + newImageCount > 5)
                throw new InvalidOperationException($"Adding {newImageCount} image(s) exceeds the limit of 5 per event. Currently has {existingImageCount}.");

            ImageUploadResult uploadResults;

            try
            {
                uploadResults = await _azureStorageService.UploadImagesAsync(imageFiles, "events", eventId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload images to storage.", ex);
            }
            try
            {
                await _eventRepository.UpdateEventImagesAsync(uploadResults.Success, eventId, userId);
                return uploadResults;
            }
            catch (Exception ex)
            {
                try
                {
                    await _azureStorageService.DeleteImagesAsync(uploadResults.Success);
                }
                catch (Exception)
                {
                    throw new Exception("Failed to delete the file.", ex);
                }
                throw new Exception("Failed to update event images.", ex);
            }
        }

        public async Task<ImageUploadResult> DeleteEventImagesAsync(List<string> imageUrls, string eventId, string userId)
        {
            if (imageUrls == null || !imageUrls.Any())
            {
                throw new ArgumentException("No image URLs provided for deletion.", nameof(imageUrls));
            }

            // Validate all URLs belong to the given event
            var invalidUrls = imageUrls
                .Where(url => !url.Contains($"/{eventId}/"))
                .ToList();

            if (invalidUrls.Any())
            {
                throw new ArgumentException($"Some URLs do not belong to the given event: {string.Join(", ", invalidUrls)}");
            }

            try
            {
                var deletionResult = await _azureStorageService.DeleteImagesAsync(imageUrls);

                if (deletionResult.Success.Any())
                {
                    var dbUpdateSuccess = await _eventRepository.DeleteEventImagesAsync(deletionResult.Success, eventId, userId);

                    if (!dbUpdateSuccess)
                    {
                        throw new Exception("Image URLs deleted from storage but failed to update database.");
                    }
                }

                return deletionResult;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete event images.", ex);
            }
        }
    }
}
