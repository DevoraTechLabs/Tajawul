using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Tajawul.Interfaces.User.Profile;
using Tajawul.Interfaces;
using System.Security.Claims;
using Tajawul.Models.DTOs;
using Tajawul.Models.DTOs.UploadService;
using Tajawul.Models.Domain.Users;

namespace Tajawul.Controllers.User.Profile
{
    [ApiController]
    [Route("api/[controller]")]
    //[EnableRateLimiting("fixed")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<Person> _personManager;
        private readonly ITokenService _tokenService;

        public UserController(IUserService userService, UserManager<Person> personManager, ITokenService tokenService)
        {
            _userService = userService;
            _personManager = personManager;
            _tokenService = tokenService;
        }

        private string? GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            try
            {
                var userProfile = await _userService.GetUserProfileAsync(userId);

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpGet("info")]
        public async Task<IActionResult> GetUserInfo()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            try
            {
                var userInfo = await _userService.GetUserInfoAsync(userId);

                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPut("info")]
        public async Task<IActionResult> UpdateUserInfo(UpdateUserProfileDto userProfileDto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            try
            {
                var person = await _personManager.FindByIdAsync(userId);

                if (person == null)
                    return BadRequest("User not found.");

                var (user, failures) = await _userService.UpdateUserInfoAsync(userId, userProfileDto);

                if (failures.Count > 0)
                {
                    return StatusCode(207, failures); // 207: Multi-Status (indicates partial success)
                }

                var roles = await _personManager.GetRolesAsync(person);

                List<IdentityResult> roleResults = new();

                if (roles.Contains("CompletedInterestInfo"))
                {
                    var userRoleResult = await _personManager.AddToRoleAsync(person, "User");
                    roleResults.Add(userRoleResult);
                    var interestsRoleResult = await _personManager.RemoveFromRoleAsync(person, "CompletedInterestInfo");
                    roleResults.Add(interestsRoleResult);
                    var personRoleResult = await _personManager.RemoveFromRoleAsync(person, "Person");
                    roleResults.Add(personRoleResult);
                }
                else
                {
                    var socialRoleResult = await _personManager.AddToRoleAsync(person, "CompletedSocialInfo");
                    roleResults.Add(socialRoleResult);
                }

                //if (roleResults.Any(r => !r.Succeeded))
                //{
                //    var errors = roleResults.SelectMany(r => r.Errors).ToList();
                //    return BadRequest(errors);
                //}

                var userRoles = await _personManager.GetRolesAsync(person);
                var token = _tokenService.CreateToken(person, userRoles);

                var refreshToken = _tokenService.CreateRefreshToken(person, userRoles);

                person.RefreshToken = refreshToken;
                var updateResult = await _personManager.UpdateAsync(person);

                if (!updateResult.Succeeded)
                {
                    return StatusCode(500);
                }

                return Ok(new
                {
                    SocialInfo = user,
                    Role = userRoles,
                    Token = token,
                    RefreshToken = refreshToken,
                    Failures = failures
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPut("interests")]
        public async Task<IActionResult> UpdateUserInterests(UserInterests userInterests)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            try
            {

                var person = await _personManager.FindByIdAsync(userId);

                if (person == null)
                    return BadRequest("User not found.");

                var (user, failures) = await _userService.UpdateUserInterestsAsync(userId, userInterests);

                if (failures.Count > 0)
                {
                    return StatusCode(207, failures); // 207: Multi-Status (indicates partial success)
                }

                var roles = await _personManager.GetRolesAsync(person);

                List<IdentityResult> roleResults = new();

                if (roles.Contains("CompletedSocialInfo"))
                {
                    var userRoleResult = await _personManager.AddToRoleAsync(person, "User");
                    roleResults.Add(userRoleResult);
                    var socialRoleResult = await _personManager.RemoveFromRoleAsync(person, "CompletedSocialInfo");
                    roleResults.Add(socialRoleResult);
                    var personRoleResult = await _personManager.RemoveFromRoleAsync(person, "Person");
                    roleResults.Add(personRoleResult);
                }
                else
                {
                    var interestsRoleResult = await _personManager.AddToRoleAsync(person, "CompletedInterestInfo");
                    roleResults.Add(interestsRoleResult);
                }

                //if (roleResults.Any(r => !r.Succeeded))
                //{
                //    var errors = roleResults.SelectMany(r => r.Errors).ToList();
                //    return BadRequest(errors);
                //}

                var userRoles = await _personManager.GetRolesAsync(person);
                var token = _tokenService.CreateToken(person, userRoles);

                var refreshToken = _tokenService.CreateRefreshToken(person, userRoles);

                person.RefreshToken = refreshToken;
                var updateResult = await _personManager.UpdateAsync(person);

                if (!updateResult.Succeeded)
                {
                    return StatusCode(500);
                }

                return Ok(new
                {
                    GraphInfo = user,
                    Role = userRoles,
                    Token = token,
                    RefreshToken = refreshToken,
                    Failures = failures
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize]
        [HttpPut("profile/image")]
        public async Task<IActionResult> UpdateProfileImage(ImageUploadDto profileImageDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { message = "User not found" });
            }

            try
            {
                var url = await _userService.UpdateProfileImageAsync(profileImageDto.ProfileImage, userId);

                return Ok(new { url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Image update failed: {ex.Message}");
            }
        }

        [Authorize(Roles = "Person")]
        [HttpPost("info")]
        public async Task<IActionResult> CompleteUserInfo(UpdateUserProfileDto userProfileDto)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            try
            {

                var person = await _personManager.FindByIdAsync(userId);

                if (person == null)
                    return BadRequest("User not found.");

                var roles = await _personManager.GetRolesAsync(person);

                if (roles.Contains("User") || roles.Contains("CompletedSocialInfo"))
                {
                    return BadRequest("User has already completed this step.");
                }

                var (user, failures) = await _userService.UpdateUserInfoAsync(userId, userProfileDto);

                if (failures.Count > 0)
                {
                    return StatusCode(207, failures); // 207: Multi-Status (indicates partial success)
                }

                List<IdentityResult> roleResults = new();

                if (roles.Contains("CompletedInterestInfo"))
                {
                    var userRoleResult = await _personManager.AddToRoleAsync(person, "User");
                    roleResults.Add(userRoleResult);
                    var interestsRoleResult = await _personManager.RemoveFromRoleAsync(person, "CompletedInterestInfo");
                    roleResults.Add(interestsRoleResult);
                    var personRoleResult = await _personManager.RemoveFromRoleAsync(person, "Person");
                    roleResults.Add(personRoleResult);
                }
                else
                {
                    var socialRoleResult = await _personManager.AddToRoleAsync(person, "CompletedSocialInfo");
                    roleResults.Add(socialRoleResult);
                }

                if (roleResults.Any(r => !r.Succeeded))
                {
                    var errors = roleResults.SelectMany(r => r.Errors).ToList();
                    return BadRequest(errors);
                }

                var userRoles = await _personManager.GetRolesAsync(person);
                var token = _tokenService.CreateToken(person, userRoles);

                var refreshToken = _tokenService.CreateRefreshToken(person, userRoles);

                person.RefreshToken = refreshToken;
                var updateResult = await _personManager.UpdateAsync(person);

                if (!updateResult.Succeeded)
                {
                    return StatusCode(500);
                }

                return Ok(new
                {
                    SocialInfo = user,
                    Role = userRoles,
                    Token = token,
                    RefreshToken = refreshToken,
                    Failures = failures
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [Authorize(Roles = "Person")]
        [HttpPost("interests")]
        public async Task<IActionResult> CompleteUserInterests(UserInterests userInterests)
        {
            var userId = GetUserId();
            if (userId == null)
            {
                return BadRequest("User not found.");
            }

            try
            {

                var person = await _personManager.FindByIdAsync(userId);

                if (person == null)
                    return BadRequest("User not found.");

                var roles = await _personManager.GetRolesAsync(person);

                if (roles.Contains("User") || roles.Contains("CompletedInterestInfo"))
                {
                    return BadRequest("User has already completed this step.");
                }

                var (user, failures) = await _userService.UpdateUserInterestsAsync(userId, userInterests);

                if (failures.Count > 0)
                {
                    return StatusCode(207, failures); // 207: Multi-Status (indicates partial success)
                }

                List<IdentityResult> roleResults = new();

                if (roles.Contains("CompletedSocialInfo"))
                {
                    var userRoleResult = await _personManager.AddToRoleAsync(person, "User");
                    roleResults.Add(userRoleResult);
                    var socialRoleResult = await _personManager.RemoveFromRoleAsync(person, "CompletedSocialInfo");
                    roleResults.Add(socialRoleResult);
                    var personRoleResult = await _personManager.RemoveFromRoleAsync(person, "Person");
                    roleResults.Add(personRoleResult);
                }
                else
                {
                    var interestsRoleResult = await _personManager.AddToRoleAsync(person, "CompletedInterestInfo");
                    roleResults.Add(interestsRoleResult);
                }

                if (roleResults.Any(r => !r.Succeeded))
                {
                    var errors = roleResults.SelectMany(r => r.Errors).ToList();
                    return BadRequest(errors);
                }
                ;

                var userRoles = await _personManager.GetRolesAsync(person);
                var token = _tokenService.CreateToken(person, userRoles);

                var refreshToken = _tokenService.CreateRefreshToken(person, userRoles);

                person.RefreshToken = refreshToken;
                var updateResult = await _personManager.UpdateAsync(person);

                if (!updateResult.Succeeded)
                {
                    return StatusCode(500);
                }

                return Ok(new
                {
                    GraphInfo = user,
                    Role = userRoles,
                    Token = token,
                    RefreshToken = refreshToken,
                    Failures = failures
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
