using AccountService.Server.Constant.IdentityServer;
using AccountService.Server.Dto;
using AccountService.Server.Models;
using AccountService.Server.Services.Account;
using Duende.IdentityServer.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Security.Claims;

namespace AccountService.Server.Controllers.Profile
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;

        public ProfileController(
            IAccountService userService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAccountService accountService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _accountService = accountService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserInfo()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);

            if (user == null)
            {
                return Unauthorized(new
                {
                    error = "invalid_token",
                    error_description = "User not found."
                });
            }
            var userLogins = await _userManager.GetLoginsAsync(user);
            var claims = await _signInManager.CreateUserPrincipalAsync(user);

            // Define what claims to return (you can also base this on scopes)
            var birthdate = claims.FindFirstValue(ClaimsConstants.BirthDate);
            DateTime? birthdateValue = null;
            if (birthdate != null)
            {
                birthdateValue = DateTime.ParseExact(birthdate, ClaimsConstants.DateFormat, CultureInfo.InvariantCulture);
            }
            var picture = claims.FindFirstValue(ClaimsConstants.Picture);
            var name = claims.FindFirstValue(ClaimsConstants.Name);

            string actualName = user.Name;
            if (string.IsNullOrEmpty(actualName))
            {
                actualName = name;
            }
            var response = new UserProfileGetResponseDto
            {
                Name = actualName,
                Email = claims.FindFirstValue(ClaimsConstants.Email),
                BirthPlace = claims.FindFirstValue(ClaimsConstants.BirthPlace),
                BirthDate = birthdateValue,
                Gender = claims.FindFirstValue(ClaimsConstants.Gender),
                Country = claims.FindFirstValue(ClaimsConstants.Country),
                Timezone = claims.FindFirstValue("timezone"),
                ProfilePictureUrl = picture,
                IsEmailVerified = user.EmailConfirmed,
                CurrentLogins = userLogins,
                HasPassword = !string.IsNullOrEmpty(user.PasswordHash),
                PasswordLastUpdate = user.PasswordLastUpdate
            };
            var dictionary = new Dictionary<string, object>();
            dictionary.Add("data", response);
            return Ok(dictionary);
        }

        [HttpPatch]
        [Authorize] // ensure only authenticated users can patch
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            try
            {
                // Ensure user is authenticated
                if (!User.Identity?.IsAuthenticated ?? true)
                {
                    return Unauthorized(new { message = "Unauthorized: Invalid or missing token." });
                }

                // Get UserId from claims
                var userId = User.FindFirst("sub")?.Value ??
                             User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "Unauthorized: User ID not found in claims." });
                }

                // Validate at least one property is being updated
                if (dto == null || (string.IsNullOrEmpty(dto.Name) &&
                                    string.IsNullOrEmpty(dto.Gender) &&
                                    string.IsNullOrEmpty(dto.Country) &&
                                    string.IsNullOrEmpty(dto.Birthdate) &&
                                    string.IsNullOrEmpty(dto.Timezone)))
                {
                    return BadRequest(new { message = "No changes provided." });
                }

                var result = await _accountService.UpdateUserProfileAsync(userId, dto);

                if (!result.Success)
                    return StatusCode(500, new { message = result.ErrorMessage ?? "Failed to update profile." });

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Profile Update Error: {ex}");
                return StatusCode(500, new { message = "Internal server error." });
            }
        }

        [HttpPost("change-pin")]
        public async Task<IActionResult> SetPinWithPassword([FromBody] ChangePinWithPasswordDto request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized(new
                {
                    error = "invalid_token",
                    error_description = "User not found."
                });
            }

            if (string.IsNullOrWhiteSpace(request.NewPin) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("PIN or Password must be provided.");
            }

            var success = await _accountService.ChangePinWithPasswordAsync(user, request.Password, request.NewPin);
            if (!success)
                return BadRequest(new { message = "Invalid password or failed to update PIN" });

            return Ok(new { message = "PIN updated successfully" });
        }

        [HttpPost("verify-password")]
        public async Task<IActionResult> VerifyPassword([FromBody] VerifyPasswordDto request)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return Unauthorized(new
                {
                    error = "invalid_token",
                    error_description = "User not found."
                });
            }
            if (string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Password must be provided.");
            }

            var success = await _accountService.VerifyPasswordAsync(user, request.Password);
            if (!success)
            {
                return NotFound("User not found or failed to verify password.");
            }

            return Ok(new { message = "Password is valid." });
        }
    }
}
