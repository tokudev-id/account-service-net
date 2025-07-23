using AccountService.Server.Models;

namespace AccountService.Server.Repositories.User {
    public interface IUserRepository
    {
        Task<ApplicationUser> FindByEmailAsync(string email);
        Task<bool> CreateUserAsync(ApplicationUser user, string password);
        // Add other user-related methods as needed (e.g., FindByIdAsync)
    }
}