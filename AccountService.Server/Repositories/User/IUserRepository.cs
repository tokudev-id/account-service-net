using AccountService.Server.Dto;
using AccountService.Server.Models;
using System.Security.Claims;

namespace AccountService.Server.Repositories.User {
    public interface IUserRepository
    {
        // QUERY
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<ApplicationUser?> FindUserByIdAsync(string id);
        Task<Dictionary<string, object>> GetUserClaimsAsync(string userId, List<string> claimTypes);
        Task<ApplicationUser?> FindByPhoneNumberAsync(string phoneNumber);

        //COMMAND
        Task<bool> CreateUserAsync(ApplicationUser user, string password);
        Task<bool> UpdateUserAsync(ApplicationUser user);
        Task<bool> VerifyUserPasswordAsync(ApplicationUser user, string password);
        Task<bool> UpdateUserClaimsAsync(string userId, Dictionary<string, string> claims);

    }
}