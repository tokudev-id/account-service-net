using System.ComponentModel.DataAnnotations;

namespace AccountService.Server.Dto
{
    public class RegisterDto
    {
        [Required]
        public string Name { get; set; } // Assuming you want to collect name
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        // Optional for OIDC flow persistence
        public string? OauthContextReturnTo { get; set; }
    }
}