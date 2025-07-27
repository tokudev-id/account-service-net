#nullable enable
using static Duende.IdentityServer.Models.IdentityResources;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;


namespace AccountService.Server.Models
{
    public class ApplicationUser: IdentityUser<string>
    {
        [MaxLength(101)]
        public string Name { get; set; }

        [Column(TypeName = "varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci")]
        [MaxLength(32)]
        public string? LoginProvider { get; set; }
        public ApplicationUser? LinkedUser { get; set; }
        public string? PinHash { get; set; } = "";

        public bool? IsOolean { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? Nonce { get; set; }
        public DateTime? PasswordLastUpdate { get; set; }

        [Column(TypeName = "varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci")]
        public override string UserName { get; set; }

        [Column(TypeName = "varchar(256) CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci")]
        public override string NormalizedUserName { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser" />.
        /// </summary>
        public ApplicationUser()
        {
            Id = $"{Guid.NewGuid():N}"; // 36 chars long
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of <see cref="T:Microsoft.AspNetCore.Identity.EntityFrameworkCore.IdentityUser" />.
        /// </summary>
        /// <param name="userName">The user name.</param>
        public ApplicationUser(string userName) : this()
        {
            UserName = userName;
        }

        /// <inheritdoc />
        /// <summary>
        /// Provide created entry date information.
        /// Auto-filled with on first data entry.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "datetime(6)")]
        public DateTime CreateDate { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Provide update entry date information.
        /// Auto-filled with on every update on the entry.
        /// </summary>
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(TypeName = "datetime(6)")]
        public DateTime UpdateDate { get; set; }
    }
}
