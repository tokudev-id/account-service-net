using AccountService.Server.Dto;
using Microsoft.AspNetCore.Identity;

namespace AccountService.Server.Services.Account
{
    public interface IAccountService
    {
        Task<SignInResult> PasswordSignInAsync(string email, string password);
        Task<IdentityResult> RegisterUserAsync(RegisterDto model);
        // Add other account-related methods as needed (e.g., SignOutAsync)
    }
}