#nullable enable
using System.ComponentModel.DataAnnotations;

namespace AccountService.Server.Dto
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [MaxLength(2000)]
        public string? ReturnUrl { get; set; }

    }
}
