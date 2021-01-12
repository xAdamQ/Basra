using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Basra.Server.Identity
{
    public class IdentityConetxt : IdentityUserContext<User>
    {
        public IdentityConetxt(DbContextOptions<IdentityConetxt> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // modelBuilder.Entity<User>(etb =>
            // {
            //     // Each User can have many UserClaims
            //     // etb.HasMany<IdentityUserClaim<int>>()
            //     // .WithOne()
            //     // .HasForeignKey(uc => uc.UserId)
            //     // .IsRequired();

            //     // Each User can have many UserClaims
            //     etb.HasMany(user => user.Claims)
            //    .WithOne()
            //    .HasForeignKey(uc => uc.UserId)
            //    .IsRequired();
            // });

            modelBuilder.Entity<User>().Property(x => x.Id).HasMaxLength(64);
            modelBuilder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.Name).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(64);

        }
    }
}