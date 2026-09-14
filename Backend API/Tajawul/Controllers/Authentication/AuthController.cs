using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using Tajawul.Interfaces;
using Tajawul.Models.Domain.Users;
using Tajawul.Models.DTOs.Auth;
using Tajawul.Models.ViewModels.Auth;
using Tajawul.Services;

namespace Tajawul.Controllers.Auth
{
    [ApiController]
    [Route("api/[controller]")]
    //[EnableRateLimiting("fixed")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Person> _personManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<Person> _signInManager;
        private readonly IEmailService _emailService;

        public AuthController(UserManager<Person> userManager, ITokenService tokenService, SignInManager<Person> signInManager, IEmailService emailService)
        {
            _personManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _emailService = emailService;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> Signup([FromBody] SignupDto signupDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var person = new Person
                    {
                        Email = signupDto.Email,
                        UserName = signupDto.Email
                    };

                    var createdPerson = await _personManager.CreateAsync(person, signupDto.Password);

                    if (!createdPerson.Succeeded)
                        return BadRequest(createdPerson.Errors);

                    var roleResult = await _personManager.AddToRoleAsync(person, "Person");

                    if (!roleResult.Succeeded)
                        return BadRequest(roleResult.Errors);

                    var token = await _personManager.GenerateEmailConfirmationTokenAsync(person);

                    var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var confirmationLink = $"{signupDto.ClientURI}?personId={Uri.EscapeDataString(person.Id)}&token={Uri.EscapeDataString(encodedToken)}";

                    var template = await _emailService.GetEmailTemplate("emailVerification");

                    var emailBackgroundService = HttpContext.RequestServices.GetRequiredService<EmailBackgroundService>();
                    await emailBackgroundService.QueueEmailAsync(person.Email, "Confirm Your Tajawul Email", template.Replace("{{confirmationLink}}", confirmationLink));

                    scope.Complete();

                    return CreatedAtAction(nameof(Signup), new { id = person.Id });
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin(SigninDto signinDto)
        {
            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                try
                {
                    var person = await _personManager.FindByEmailAsync(signinDto.Email);

                    if (person == null)
                        return BadRequest("Invalid request!");

                    if (!await _personManager.IsEmailConfirmedAsync(person))
                    {
                        return Unauthorized(new { message = "Email not confirmed." });
                    }

                    var result = await _signInManager.PasswordSignInAsync(person.Email!, signinDto.Password, isPersistent: false, lockoutOnFailure: true);


                    if (result.IsLockedOut)
                        return Unauthorized(new
                        {
                            message = "account locked due to too many failed attempts. try again later."
                        });

                    if (!result.Succeeded)
                        return Unauthorized(new
                        {
                            message = "Invalid email or password."
                        });


                    var userRoles = await _personManager.GetRolesAsync(person);
                    var token = _tokenService.CreateToken(person, userRoles);

                    var refreshToken = _tokenService.CreateRefreshToken(person, userRoles);
                    
                    person.RefreshToken = refreshToken;
                    person.LastLogin = DateTime.Now;
                    var updateResult = await _personManager.UpdateAsync(person);

                    if (!updateResult.Succeeded)
                    {
                        return StatusCode(500);
                    }

                    var loginResult = new SuccessSigninDto
                    {
                        Email = person.Email!,
                        Role = userRoles,
                        Token = token,
                        RefreshToken = refreshToken
                    };

                    scope.Complete();

                    return CreatedAtAction(nameof(Signin), loginResult);
                }
                catch (Exception e)
                {
                    return StatusCode(500, e.Message);
                }
            }

        }

        [HttpPost("refreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                // Step 1: Validate the refresh token
                var principal = _tokenService.ValidateRefreshToken(refreshTokenDto.RefreshToken);
                

                if (principal == null)
                {
                    return Unauthorized(new { message = "Invalid or expired refresh token" });
                }

                // Step 5: Check if the refresh token has expired
                var Exp_date = _tokenService.GetExpirationDate(refreshTokenDto.RefreshToken);
                if (Exp_date < DateTime.UtcNow)
                {
                    return Unauthorized(new { message = "Refresh token expired" });
                }

                //// Step 2: Extract email from the token claims
                var email = principal.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Step 3: Retrieve the user based on the email
                var person = await _personManager.FindByEmailAsync(email);
                if (person == null)
                {
                    return Unauthorized(new { message = "User not found" });
                }

                // Step 4: Check if the stored refresh token matches the one in the request
                if (person.RefreshToken != refreshTokenDto.RefreshToken)
                {
                    return Unauthorized(new { message = "Invalid refresh token" });
                }

                var userRoles = await _personManager.GetRolesAsync(person);
                if (userRoles == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var newAccessToken = _tokenService.CreateToken(person, userRoles);

                // Step 7: Return the new access token (no refresh token regeneration)
                return Ok(new { Token = newAccessToken });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPost("sendEmailVerification")]
        public async Task<IActionResult> SendEmailVerification([FromBody] SendEmailVerificationDto sendEmailVerification)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var person = await _personManager.FindByEmailAsync(sendEmailVerification.Email!);

                if (person == null)
                    return Ok(new { Message = "Email verification link has been sent to your email!" });

                var token = await _personManager.GenerateEmailConfirmationTokenAsync(person);

                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var confirmationLink = $"{sendEmailVerification.ClientURI}?personId={Uri.EscapeDataString(person.Id)}&token={Uri.EscapeDataString(encodedToken)}";

                var template = await _emailService.GetEmailTemplate("emailVerification");

                var emailBackgroundService = HttpContext.RequestServices.GetRequiredService<EmailBackgroundService>();
                await emailBackgroundService.QueueEmailAsync(person.Email!, "Confirm Your Tajawul Email", template.Replace("{{confirmationLink}}", confirmationLink!));

                return Ok(new { Message = "Email verification link has been sent to your email!" });
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto confirmDto)
        {
            var person = await _personManager.FindByIdAsync(confirmDto.PersonId);

            if (person == null)
                return BadRequest("Invalid request!");


            var decodedBytes = WebEncoders.Base64UrlDecode(confirmDto.Token);
            var decodedToken = Encoding.UTF8.GetString(decodedBytes);

            var result = await _personManager.ConfirmEmailAsync(person, decodedToken);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(result);
        }

        [HttpPost("sendResetPasswordEmail")]
        public async Task<IActionResult> SendResetPassword([FromBody] SendResetPasswordDto resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var person = await _personManager.FindByEmailAsync(resetPassword.Email!);

                if (person == null)
                {
                    return Ok(new { Message = "Reset password link has been sent to your email" });
                }

                var token = await _personManager.GeneratePasswordResetTokenAsync(person);

                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var resetLink = $"{resetPassword.ClientURI}?email={person.Email}&token={Uri.EscapeDataString(encodedToken)}";
                
                var template = await _emailService.GetEmailTemplate("forgotPassword");

                var emailBackgroundService = HttpContext.RequestServices.GetRequiredService<EmailBackgroundService>();
                await emailBackgroundService.QueueEmailAsync(person.Email!, "Change your Tajawul password", template.Replace("{{resetLink}}", resetLink!));

                return Ok(new { Message = "Reset password link has been sent to your email" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPassword)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var person = await _personManager.FindByEmailAsync(resetPassword.Email!);

                if (person == null)
                {
                    return BadRequest("Invalid Request!");
                }

                var decodedBytes = WebEncoders.Base64UrlDecode(resetPassword.Token!);
                var decodedToken = Encoding.UTF8.GetString(decodedBytes);

                var result = await _personManager.ResetPasswordAsync(person, decodedToken, resetPassword.NewPassword!);

                if (result.Succeeded)
                {
                    return Ok(new { Message = "Password has been reset successfully" });
                }
                else
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return BadRequest(new { Errors = errors });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var person = await _personManager.GetUserAsync(User);

                if (person == null)
                {
                    return Unauthorized(new { message = "User not found." });
                }

                // Invalidate the refresh token
                person.RefreshToken = null;

                // Update the user entity in the database to reflect changes
                var updateResult = await _personManager.UpdateAsync(person);

                if (!updateResult.Succeeded)
                {
                    return StatusCode(500, new { message = "Error invalidating refresh token" });
                }

                // Sign out the user
                await _signInManager.SignOutAsync();

                return Ok(new { message = "You have been logged out successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
