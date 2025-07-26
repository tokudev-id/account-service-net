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
        private readonly IAccountService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;

        public ProfileController(
            IAccountService userService,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IAccountService accountService)
        {
            _userService = userService;
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
                City = claims.FindFirstValue(ClaimsConstants.City),
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

        [HttpPost]
        public IActionResult PostFallback()
        {
            return BadRequest(new
            {
                error = "unsupported_grant_type",
                error_description = "Only GET with cookie session is supported"
            });
        }

        [HttpPost("set-pin")]
        public async Task<IActionResult> SetPin([FromBody] SetPinRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.Pin) || string.IsNullOrWhiteSpace(request.UserId))
            {
                return BadRequest("UserId and PIN must be provided.");
            }

            var success = await _accountService.SetUserPinAsync(request.UserId, request.Pin);
            if (!success)
            {
                return NotFound("User not found or failed to update PIN.");
            }

            return Ok(new { message = "PIN updated successfully." });
        }
    }
}
