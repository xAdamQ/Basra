using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Basra.Server
{
    public class MasterContext : IdentityUserContext<BasraIdentityUser>
    {
        public MasterContext(DbContextOptions options) : base(options) { }

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

            modelBuilder.Entity<BasraIdentityUser>().Property(x => x.Id).HasMaxLength(64);
            modelBuilder.Entity<IdentityRole>().Property(x => x.Id).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.ProviderKey).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserLogin<string>>().Property(x => x.LoginProvider).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.Name).HasMaxLength(64);
            modelBuilder.Entity<IdentityUserToken<string>>().Property(x => x.LoginProvider).HasMaxLength(64);

        }
    }
}