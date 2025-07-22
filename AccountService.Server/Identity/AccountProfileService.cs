using AccountService.Server.Models;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AccountService.Server.Identity
{
    /// <summary>
    /// This service is responsible for adding user claims to tokens and determining if a user is active.
    /// It's called by IdentityServer during the token creation process.
    /// </summary>
    public class AccountProfileService : IProfileService
    {
        private readonly ILogger<AccountProfileService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountProfileService(UserManager<ApplicationUser> userManager, ILogger<AccountProfileService> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g., for an ID token or access token).
        /// </summary>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            _logger.LogInformation("Get profile data called for subject {SubjectId}", subjectId);

            var user = await _userManager.FindByIdAsync(subjectId);
            if (user == null)
            {
                _logger.LogWarning("No user found for subject {SubjectId}", subjectId);
                return;
            }

            var claims = new List<Claim>
        {
            new Claim(JwtClaimTypes.Name, user.UserName),
            new Claim(JwtClaimTypes.Email, user.Email),
            // Add other standard claims as needed
        };

            // Add user roles as 'role' claims
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(JwtClaimTypes.Role, role)));

            // Add custom claims from your ApplicationUser model if you have any
            // if (!string.IsNullOrEmpty(user.FullName))
            // {
            //     claims.Add(new Claim("full_name", user.FullName));
            // }

            // Only add the claims that were actually requested
            context.IssuedClaims.AddRange(claims);
        }

        /// <summary>
        /// This method is called whenever IdentityServer needs to determine if a user is still allowed to receive tokens.
        /// </summary>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            _logger.LogInformation("IsActiveAsync called for subject {SubjectId}", subjectId);

            var user = await _userManager.FindByIdAsync(subjectId);

            // A user is active if they exist and are not locked out.
            context.IsActive = (user != null) && !await _userManager.IsLockedOutAsync(user);
        }
    }
}
