using AccountService.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AccountService.Server.Services.Account;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using AccountService.Server.Dto;
using Microsoft.VisualBasic;
using Duende.IdentityServer.Events;
using Duende.IdentityServer.Extensions;

namespace AccountService.Server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AccountController : ControllerBase
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IAccountService _accountService;
        private readonly IIdentityServerInteractionService _interactionService;
        private readonly IClientStore _clientStore; // Needed for OIDC context
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(
            IIdentityServerInteractionService interaction,
            IAccountService accountService,
            IIdentityServerInteractionService interactionService,
            IClientStore clientStore,
            SignInManager<ApplicationUser> singinManager)
        {
            _interaction = interaction;
            _accountService = accountService;
            _interactionService = interactionService;
            _clientStore = clientStore;
            _signInManager = singinManager;
        }

        [HttpGet("check-session")]
        public IActionResult GetAuthStatus()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Ok(new { isAuthenticated = true, userName = User.Identity.Name }); // Adapt userName if needed
            }
            else
            {
                return Ok(new { isAuthenticated = false });
            }
        }

        [HttpGet("context")]
        public async Task<IActionResult> Context(string returnUrl)
        {
            var authzContext = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (authzContext != null)
            {
                return Ok(new
                {
                    loginHint = authzContext.LoginHint,
                    idp = authzContext.IdP,
                    tenant = authzContext.Tenant,
                    scopes = authzContext.ValidatedResources.RawScopeValues,
                    client = authzContext.Client.ClientName ?? authzContext.Client.ClientId
                });
            }

            return BadRequest();
        }

        [HttpGet("error")]
        public async Task<IActionResult> Error(string errorId)
        {
            var errorInfo = await _interaction.GetErrorContextAsync(errorId);
            if (errorInfo != null)
            {
                return Ok(new
                {
                    errorInfo.Error,
                    errorInfo.ErrorDescription
                });
            }

            return BadRequest();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            // check if we are in the context of an authorization request
            var context = await _interaction.GetAuthorizationContextAsync(request.ReturnUrl);

            // Reject possible malicious return url
            if (!(context != null
                  || string.IsNullOrWhiteSpace(request.ReturnUrl)
                  || Url.IsLocalUrl(request.ReturnUrl)))
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid arguments");
            }

            var result = await _accountService.PasswordSignInAsync(request.Email, request.Password);

            if (result.Succeeded)
            {
                // If successful, you might want to handle the OIDC flow continuation
                // based on the context if this login was initiated by an OIDC request.
                // For simplicity, we'll just return a success status for now.
                return Ok(new { message = "Login successful" });
            }

            // Handle different sign-in failure cases (e.g., locked out, requires verification)
            if (result.IsLockedOut)
            {
                return Unauthorized(new { message = "Account locked out." });
            }
            // Add other failure checks as needed

            return Unauthorized(new { message = "Invalid login attempt." });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            var result = await _accountService.RegisterUserAsync(model);

            if (result.Succeeded)
            {
                // If registration is successful, you might want to automatically sign in the user
                // or redirect them to a confirmation page.
                // For simplicity, we'll just return a success status for now.
                return Ok(new { message = "Registration successful" });
            }

            // Handle registration errors
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { errors = errors });
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var logoutInfo = await _interaction.GetLogoutContextAsync(logoutId);

            if (logoutInfo != null && !string.IsNullOrEmpty(logoutId))
            {
                if (string.IsNullOrEmpty(logoutInfo.ClientId))
                {
                    return BadRequest("Unable to get logout info.");
                }

                if (User.Identity.IsAuthenticated)
                {
                    await _signInManager.SignOutAsync();

                    return Ok(new
                    {
                        iframeUrl = logoutInfo.SignOutIFrameUrl,
                        postLogoutRedirectUri = logoutInfo.PostLogoutRedirectUri
                    });
                }

            }
            await _signInManager.SignOutAsync();
            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> PostLogout(string? logoutId="")
        {
            var logoutInfo = await _interaction.GetLogoutContextAsync(logoutId);

            if (User.Identity.IsAuthenticated)
            {
                await _signInManager.SignOutAsync();

                return Ok(new
                {
                    iframeUrl = logoutInfo?.SignOutIFrameUrl,
                    postLogoutRedirectUri = logoutInfo?.PostLogoutRedirectUri
                });
            }

            return BadRequest();
        }
    }
}
