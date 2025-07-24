using AccountService.Server.Repositories.User;
using AccountService.Server.Models;
using AccountService.Server.Dto;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AccountService.Server.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public AccountService(IUserRepository userRepository, SignInManager<ApplicationUser> signInManager)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser?> FindUserByIdAsync(string id)
        {
            return await _userRepository.FindUserByIdAsync(id);
        }

        public async Task<Dictionary<string, object>> GetUserClaimsAsync(string userId, List<string> claimTypes)
        {
            return await _userRepository.GetUserClaimsAsync(userId, claimTypes);
        }

        public async Task<SignInResult> PasswordSignInAsync(string email, string password)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed;
            }

            return await _signInManager.PasswordSignInAsync(user, password, isPersistent: true, lockoutOnFailure: false);
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDto model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            var result = await _userRepository.CreateUserAsync(user, model.Password);

            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User creation failed." });
        }

    }
}
