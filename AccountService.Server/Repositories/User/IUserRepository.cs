using AccountService.Server.Dto;
using AccountService.Server.Models;
using System.Security.Claims;

namespace AccountService.Server.Repositories.User {
    public interface IUserRepository
    {
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<bool> CreateUserAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> FindUserByIdAsync(string id);
        Task<Dictionary<string, object>> GetUserClaimsAsync(string userId, List<string> claimTypes);
        Task<bool> UpdateUserAsync(ApplicationUser user);

        Task<ApplicationUser?> FindByPhoneNumberAsync(string phoneNumber);
    }
}