using System.Collections.Generic;
using Basra.Server.Data;
using Microsoft.EntityFrameworkCore;

namespace Basra.Server
{
    public class MasterContext : DbContext
    {
        public MasterContext(DbContextOptions<MasterContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<RoomUser> RoomUsers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<PendingRoom> PendingRooms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasMany<RoomUser>(g => g.RoomUsers)
                .WithOne(s => s.Room)
                .HasForeignKey(s => s.RoomId);

            modelBuilder.Entity<User>().HasData(
                new List<User>()
                {
                    new User
                    {
                        Id = "0",
                        Fbid = "0",
                        PlayedGames = 3,
                        Wins = 4,
                        Name = "hany"
                    },
                    new User
                    {
                        Id = "1",
                        Fbid = "1",
                        PlayedGames = 7,
                        Wins = 11,
                        Name = "samy"
                    },
                    new User
                    {
                        Id = "2",
                        Fbid = "2",
                        PlayedGames = 9,
                        Wins = 1,
                        Name = "anni"
                    },
                    new User
                    {
                        Id = "3",
                        Fbid = "3",
                        PlayedGames = 2,
                        Wins = 0,
                        Name = "ali"
                    },
                }
            );
            modelBuilder.Entity<RoomUser>().HasData
            (
                new List<RoomUser>()
                {
                    new RoomUser
                    {
                        Id = "0",
                        RoomId = "0",
                        UserId = "3" //ali
                    },
                    new RoomUser
                    {
                        Id = "1",
                        RoomId = "1",
                        UserId = "0" //hany
                    },
                }
            );
            modelBuilder.Entity<Room>().HasData(
                new List<Room>
                {
                    new Room
                    {
                        Id = "0",
                    },
                    new Room
                    {
                        Id = "1",
                    },
                }
            );
            modelBuilder.Entity<PendingRoom>().HasData(
                new List<PendingRoom>
                {
                    new PendingRoom
                    {
                        Id = 1,
                        RoomId = "1"
                    }
                }
            );
        }
    }
}