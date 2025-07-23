using AccountService.Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AccountService.Server.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity => {
                entity.ToTable(name: "Users");
                entity.HasIndex(it => it.Id);
                entity.Property(e => e.Id).HasMaxLength(50);
                entity.Property(e => e.UserName).HasCharSet("utf8mb4").UseCollation("utf8mb4_general_ci");
                entity.Property(e => e.NormalizedUserName).HasCharSet("utf8mb4").UseCollation("utf8mb4_general_ci");
            });

            builder.Entity<ApplicationRole>(entity => {
                entity.ToTable(name: "Roles");
                entity.Property(e => e.Id).HasMaxLength(50);
            });

            builder.Entity<IdentityUserClaim<string>>(entity => {
                entity.ToTable(name: "UserClaims");
                entity.HasIndex(it => it.ClaimType);
                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            builder.Entity<IdentityRoleClaim<string>>(entity => {
                entity.ToTable(name: "RoleClaims");
                entity.Property(e => e.RoleId).HasMaxLength(50);
            });

            builder.Entity<IdentityUserRole<string>>(entity => {
                entity.ToTable(name: "UserRoles");
                entity.Property(e => e.RoleId).HasMaxLength(50);
                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            builder.Entity<IdentityUserLogin<string>>(entity => {
                entity.ToTable(name: "Logins");
                entity.Property(e => e.LoginProvider).HasMaxLength(256);
                entity.Property(e => e.ProviderKey).HasMaxLength(256);
                entity.Property(e => e.UserId).HasMaxLength(50);
            });

            builder.Entity<IdentityUserToken<string>>(entity => {
                entity.ToTable(name: "UserTokens");
                entity.Property(e => e.UserId).HasMaxLength(50);
                entity.Property(e => e.LoginProvider).HasMaxLength(256);
                entity.Property(e => e.Name).HasMaxLength(256);
            });
        }
    }
}
