using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using QuickServe.Infrastructure.Identity.Models;
using System;

namespace QuickServe.Infrastructure.Identity.Contexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("Identity");
            // Cấu hình bảng ApplicationUser
            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable(name: "User");

                // Cấu hình mối quan hệ một-một với ApplicationRole
                entity.HasOne(user => user.ApplicationRole)
                    .WithOne(role => role.ApplicationUser)
                    .HasForeignKey<ApplicationRole>(role => role.ApplicationUserId);
            });

            /*builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "Role");
            });*/

            // Cấu hình bảng ApplicationRole
            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable(name: "Role");
                entity.HasIndex(role => role.ApplicationUserId).IsUnique(); // Tạo chỉ mục duy nhất trên ApplicationUserId
            });
            builder.Entity<IdentityUserRole<Guid>>(entity =>
            {
                entity.ToTable("UserRoles");
            });

            builder.Entity<IdentityUserClaim<Guid>>(entity =>
            {
                entity.ToTable("UserClaims");
            });

            builder.Entity<IdentityUserLogin<Guid>>(entity =>
            {
                entity.ToTable("UserLogins");
            });

            builder.Entity<IdentityRoleClaim<Guid>>(entity =>
            {
                entity.ToTable("RoleClaims");

            });

            builder.Entity<IdentityUserToken<Guid>>(entity =>
            {
                entity.ToTable("UserTokens");
            });

        }
    }
}
