using AccountService.Server.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AccountService.Server.Repositories.User
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

        public async Task<ApplicationUser?> FindUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<Dictionary<string, object>> GetUserClaimsAsync(string userId, List<string> claimTypes)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new Dictionary<string, object>();

            var claims = await _userManager.GetClaimsAsync(user);

            return claims
                .Where(claim => claimTypes.Contains(claim.Type))
                .ToDictionary(claim => claim.Type, claim => (object)claim.Value);
        }
    }
}
