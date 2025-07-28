using AccountService.Server.Dto;
using AccountService.Server.Models;
using Microsoft.AspNetCore.Identity;

namespace AccountService.Server.Services.Account
{
    public interface IAccountService
    {
        Task<SignInResult> PasswordSignInAsync(string email, string password);
        Task<IdentityResult> RegisterUserAsync(RegisterDto model);
        Task<ApplicationUser?> FindUserByIdAsync(string id);
        Task<Dictionary<string, object>> GetUserClaimsAsync(string userId, List<string> claimTypes);
        Task<bool> ValidateUserWithPinAsync(string phoneNumber, string pin);
        public Task<bool> SetUserPinAsync(string userId, string pin);

        Task<(bool Success, string? ErrorMessage, object? Data)> UpdateUserProfileAsync(string userId, UpdateProfileDto dto);
    }
}