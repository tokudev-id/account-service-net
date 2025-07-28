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
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

        public AccountService(
            IUserRepository userRepository,
            SignInManager<ApplicationUser> signInManager,
            IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _userRepository = userRepository;
            _signInManager = signInManager;
            _passwordHasher = passwordHasher;
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

        public async Task<bool> ValidateUserWithPinAsync(string phoneNumber, string pin)
        {
            var user = await _userRepository.FindByPhoneNumberAsync(phoneNumber);
            if (user == null || string.IsNullOrEmpty(user.PinHash))
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.PinHash, pin);
            return result == PasswordVerificationResult.Success;
        }

        public async Task<bool> SetUserPinAsync(string userId, string pin)
        {
            var user = await _userRepository.FindUserByIdAsync(userId);
            if (user == null)
                return false;

            user.PinHash = _passwordHasher.HashPassword(user, pin);
            return await _userRepository.UpdateUserAsync(user);
        }

        public async Task<(bool Success, string? ErrorMessage, object? Data)>
            UpdateUserProfileAsync(string userId, UpdateProfileDto dto)
        {
            var user = await _userRepository.FindUserByIdAsync(userId);
            if (user == null)
                return (false, "User not found.", null);

            // Update basic fields
            if (!string.IsNullOrEmpty(dto.Name))
                user.Name = dto.Name;

            // Claims dictionary
            var claimUpdates = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(dto.Gender)) claimUpdates["gender"] = dto.Gender;
            if (!string.IsNullOrEmpty(dto.Country)) claimUpdates["country"] = dto.Country;
            if (!string.IsNullOrEmpty(dto.Birthdate))
            {
                if (DateTime.TryParse(dto.Birthdate, out var birthdate))
                    claimUpdates["birthdate"] = birthdate.ToString("yyyy-MM-dd");
                else
                    return (false, "Invalid birthdate format.", null);
            }
            if (!string.IsNullOrEmpty(dto.Timezone)) claimUpdates["timezone"] = dto.Timezone;

            // Save user updates
            var updateResult = await _userRepository.UpdateUserAsync(user);
            if (!updateResult)
                return (false, "Failed to update user profile.", null);

            // Save claim updates
            if (claimUpdates.Any())
            {
                var claimResult = await _userRepository.UpdateUserClaimsAsync(user.Id, claimUpdates);
                if (!claimResult)
                    return (false, "Failed to update user claims.", null);
            }

            var safeUser = new
            {
                user.Id,
                user.Name,
                user.Email,
                Claims = claimUpdates
            };

            return (true, null, safeUser);
        }
    }
}
