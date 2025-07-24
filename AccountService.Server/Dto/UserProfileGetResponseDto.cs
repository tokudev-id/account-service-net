using Microsoft.AspNetCore.Identity;

#nullable enable
namespace AccountService.Server.Dto
{
    public class UserProfileGetResponseDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string BirthPlace { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ProfilePictureUrl { get; set; }
        public bool HasPassword { get; set; }
        public bool IsEmailVerified { get; set; }
        public DateTime? PasswordLastUpdate { get; set; }

        public IList<UserLoginInfo> CurrentLogins { get; set; }
    }
}
