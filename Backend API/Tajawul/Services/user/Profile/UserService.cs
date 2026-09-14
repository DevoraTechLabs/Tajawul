using Tajawul.Interfaces.General;
using Tajawul.Interfaces.Media;
using Tajawul.Interfaces.User.ChatBot;
using Tajawul.Interfaces.User.Profile;
using Tajawul.Models.Domain.Chatbot;
using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs;
using Tajawul.Models.ViewModels.user;
using Tajawul.Repositories.user.Profile;
using Tajawul.Repositories.User;

namespace Tajawul.Services.User.Profile
{
    public class UserService : IUserService
    {
        private readonly UserProfileRepository _userProfileRepository;
        private readonly ILocationService _locationService;
        private readonly IMaritalStatusService _maritalStatusService;
        private readonly ISpokenLanguageService _spokenLanguageService;
        private readonly IAzureStorageService _azureStorageService;
        private readonly IChatbotService _chatbotService;

        public UserService(UserProfileRepository userProfileRepository, ILocationService locationService,
            IMaritalStatusService maritalStatusService, ISpokenLanguageService spokenLanguageService,
            IAzureStorageService azureStorageService, IChatbotService chatbotService)
        {
            _userProfileRepository = userProfileRepository;
            _locationService = locationService;
            _maritalStatusService = maritalStatusService;
            _spokenLanguageService = spokenLanguageService;
            _azureStorageService = azureStorageService;
            _chatbotService = chatbotService;
        }

        public async Task<UserInfoDto> GetUserInfoAsync(string userId)
        {
            UserInfoDto userInfo =  await _userProfileRepository.GetUserInfoAsync(userId);

            List<Message> messages = await _chatbotService.GetLastMessagesForUserAsync(userId, 5);

            userInfo.Messages = messages;

            return userInfo;
        }

        public async Task<UserModel?> GetUserProfileAsync(string userId)
        {

            var userResult = await _userProfileRepository.CreateUserNodeIfNotExistAsync(userId);

            if (userResult == false)
            {
                throw new Exception("Error creating User node");
            }

            var user = await _userProfileRepository.GetUserProfileAsync(userId);

            var userInterests = await _userProfileRepository.GetUserInterestsAsync(userId);

            user!.Interests = userInterests;

            return user;
        }

        public async Task<string> UpdateProfileImageAsync(IFormFile file, string userId)
        {

            var userResult = await _userProfileRepository.CreateUserNodeIfNotExistAsync(userId);

            if (userResult == false)
            {
                throw new Exception("Error creating User node");
            }

            string imageUrl;

            try
            {
                imageUrl = await _azureStorageService.UploadImageAsync(file, "users", userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to upload image to storage.", ex);
            }

            try
            {
                var result = await _userProfileRepository.UpdateProfileImageAsync(imageUrl, userId);

                if (result == true)
                {
                    return imageUrl;
                }
                else
                {
                    throw new Exception();
                }
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
                throw new Exception("Failed to update user image.", ex);
            }
        }

        public async Task<(UserModel, List<string>)> UpdateUserInfoAsync(string userId, UpdateUserProfileDto userProfileDto)
        {
            var userResult = await _userProfileRepository.CreateUserNodeIfNotExistAsync(userId);

            if (userResult == false)
            {
                throw new Exception("Error creating User node");
            }

            List<string> failures = [];
            UserModel user = await _userProfileRepository.UpdateUserProfileAsync(userProfileDto, userId);

            try
            {
                var spokenLanguagesResult = await _spokenLanguageService.AddUserSpokenLanguagesAsync(userId, userProfileDto.SpokenLanguages);
                if (spokenLanguagesResult.relationshipsCreated == userProfileDto.SpokenLanguages.Count)
                {
                    user.SpokenLanguages = spokenLanguagesResult.spokenLanguages;
                }
                else
                {
                    var missMatch = userProfileDto.SpokenLanguages
                     .Except(spokenLanguagesResult.spokenLanguages, StringComparer.OrdinalIgnoreCase)
                     .ToList();
                    failures.Add($"Some spoken languages not found: {string.Join(", ", missMatch)}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Languages assignment failed: {ex.Message}");
            }

            try
            {
                var maritalStatusResult = await _maritalStatusService.AddUserMaritalStatusAsync(userId, userProfileDto.MaritalStatus!);
                if (maritalStatusResult > 0)
                {
                    user.MaritalStatus = userProfileDto.MaritalStatus;
                }
                else
                {
                    failures.Add($"Marital Status not found: {userProfileDto.MaritalStatus}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Marital Status assignment failed: {ex.Message}");
            }

            try
            {
                var genderResult = await _userProfileRepository.AddUserGender(userId, userProfileDto.Gender!);
                if (genderResult > 0)
                {
                    user.Gender = userProfileDto.Gender;
                }
                else
                {
                    failures.Add($"Gender not found: {userProfileDto.Gender}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Gender assignment failed: {ex.Message}");
            }

            try
            {
                var city = await _locationService.AddUserLocationAsync(userId, userProfileDto.Country!, userProfileDto.City!);
                user.City = city!.Name;
                user.Country = city.Country.Name;
            }
            catch (Exception ex)
            {
                failures.Add($"Location assignment failed: {ex.Message}");
            }

            return (user, failures);
        }

        public async Task<(UserModel, List<string>)> UpdateUserInterestsAsync(string userId, UserInterests updatedInterests)
        {
            var userResult = await _userProfileRepository.CreateUserNodeIfNotExistAsync(userId);

            if (userResult == false)
            {
                throw new Exception("Error creating User node");
            }

            var user = await _userProfileRepository.GetUserProfileAsync(userId);

            var userInterests = new UserInterests();

            user!.Interests = userInterests;

            List<string> failures = [];

            try
            {
                var tags = await _userProfileRepository.AddUserTags(userId, updatedInterests.Tags);

                user.Interests.Tags = tags;
            }
            catch (Exception ex)
            {
                failures.Add($"Tags assignment failed: {ex.Message}");
            }

            try
            {
                var priceRangesResult = await _userProfileRepository.AddUserPriceRangesAsync(userId, updatedInterests.PriceRanges);

                if (priceRangesResult.relationshipsCreated == updatedInterests.PriceRanges.Count)
                {
                    user.Interests.PriceRanges = priceRangesResult.priceRanges;
                }
                else
                {
                    var missMatch = updatedInterests.PriceRanges
                    .Except(priceRangesResult.priceRanges, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                    failures.Add($"Price Ranges not found: {string.Join(", ", missMatch)}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Price Ranges assignment failed: {ex.Message}");
            }

            try
            {
                var activities = await _userProfileRepository.AddUserActivities(userId, updatedInterests.Activities);
                user.Interests.Activities = activities;
            }
            catch (Exception ex)
            {
                failures.Add($"Activities assignment failed: {ex.Message}");
            }

            try
            {
                var destinationTypes = await _userProfileRepository.AddUserDestinationTypes(userId, updatedInterests.DestinationTypes);
                if (destinationTypes.relationshipsCreated == updatedInterests.DestinationTypes.Count)
                {
                    user.Interests.DestinationTypes = destinationTypes.destinationTypes;
                }
                else
                {
                    var missMatch = updatedInterests.DestinationTypes
                    .Except(destinationTypes.destinationTypes, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                    failures.Add($"Destination Types not found: {string.Join(", ", missMatch)}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Destination Types assignment failed: {ex.Message}");
            }

            // make this take a list

            try
            {
                var groupSizes = await _userProfileRepository.AddUserGroupSizes(userId, updatedInterests.GroupSizes);
                if (groupSizes.relationshipsCreated == updatedInterests.GroupSizes.Count)
                {
                    user.Interests.GroupSizes = groupSizes.groupSizes;
                }
                else
                {
                    var missMatch = updatedInterests.GroupSizes
                    .Except(groupSizes.groupSizes, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                    failures.Add($"Group Sizes not found: {string.Join(", ", missMatch)}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Group Sizes assignment failed: {ex.Message}");
            }

            // trip durations function does not exist
            try
            {
                var tripDurations = await _userProfileRepository.AddUserTripDurationsAsync(userId, updatedInterests.TripDurations);
                if (tripDurations.relationshipsCreated == updatedInterests.TripDurations.Count)
                {
                    user.Interests.TripDurations = tripDurations.tripDurations;
                }
                else
                {
                    var missMatch = updatedInterests.TripDurations
                    .Except(tripDurations.tripDurations, StringComparer.OrdinalIgnoreCase)
                    .ToList();
                    failures.Add($"Trip Durations not found: {string.Join(", ", missMatch)}");
                }
            }
            catch (Exception ex)
            {
                failures.Add($"Trip Durations assignment failed: {ex.Message}");
            }

            return (user, failures);
        }
    }
}
