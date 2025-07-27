using AccountService.Server.Models;
using AccountService.Server.Repositories.User;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Validation;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public class PinResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;

    public PinResourceOwnerPasswordValidator(IUserRepository userRepository, IPasswordHasher<ApplicationUser> passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
    {
        var user = await _userRepository.FindByPhoneNumberAsync(context.UserName);
        if (user == null)
        {
            context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_username_or_password");
            return;
        }

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PinHash, context.Password);
        if (passwordVerificationResult == PasswordVerificationResult.Success)
        {
            context.Result = new GrantValidationResult(user.Id, "password", claims: new List<Claim>
            {
                new("phone_number", user.PhoneNumber ?? ""),
                new("PhoneNumberConfirmed", user.PhoneNumberConfirmed.ToString())
            });
            return;
        }

        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid_username_or_password");
    }
}
