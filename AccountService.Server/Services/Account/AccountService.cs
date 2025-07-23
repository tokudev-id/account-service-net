
using AccountService.Server.Repositories.User;
using AccountService.Server.Models;
using AccountService.Server.Dto;
using Microsoft.AspNetCore.Identity;
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

        public async Task<SignInResult> PasswordSignInAsync(string email, string password)
        {
            var user = await _userRepository.FindByEmailAsync(email);
            if (user == null)
            {
                return SignInResult.Failed; // User not found
            }

            // This will attempt to sign in the user and create a cookie
            var result = await _signInManager.PasswordSignInAsync(user, password, isPersistent: false, lockoutOnFailure: false);

            return result;
        }

        public async Task<IdentityResult> RegisterUserAsync(RegisterDto model)
        {
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
            // Add other user properties from model if needed

            var result = await _userRepository.CreateUserAsync(user, model.Password);

            return result ? IdentityResult.Success : IdentityResult.Failed(new IdentityError { Description = "User creation failed." }); // Return IdentityResult
        }

        // Implement other methods from IAccountService
    }
}
