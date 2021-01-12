using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Basra.Server.Data
{
    public class GameContext : DbContext
    {
        public GameContext(DbContextOptions<GameContext> options) : base(options) { }

        public DbSet<User> Users;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().Ignore(u => u.ConnectionId);
            modelBuilder.Entity<User>().Ignore(u => u.Disconncted);
            modelBuilder.Entity<User>().Ignore(u => u.RoomUser);

            modelBuilder.Entity<User>().HasKey(du => du.IdentityUserId);
            modelBuilder.Entity<Identity.User>()
            .HasOne(iu => iu.DataUser)
            .WithOne(du => du.IdentityUser)
            .HasForeignKey<User>(du => du.IdentityUserId);

        }
    }
}
