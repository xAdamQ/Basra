using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Basra.Server.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Basra.Server.Tests
{
    public class EfQueryTests
    {
        private MasterContext GetMasterContext()
        {
            var options2 = new DbContextOptionsBuilder<MasterContext>().UseMySql(
                "Server=localhost; Port=3306; Uid=root; Pwd=23437075; Database=Basra;",
                new MySqlServerVersion(new Version(8, 0, 21)),
                mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)
            ).Options;

            return new MasterContext(options2);
        }

        [Fact]
        public async Task projection()
        {
            //arrange
            var options = new DbContextOptionsBuilder<MasterContext>()
                .UseInMemoryDatabase(databaseName: "the_name_of_in_memory_db")
                .Options;

            var masterContext = GetMasterContext();

            //act
            var q = masterContext.Users.Select(u => new ProjectedUser
            {
                Id = u.Id,
                Email = u.Email,
                IsActive = u.IsActive,
            });
////////////////this was projection

            var str = q.ToQueryString();
            //assert
        }

        [Fact]
        public async Task selectselect()
        {
            // var q = trackedRoom.RoomUsers.Select(rs => rs.User).Select(DisplayUser.Projection);

            var mc = GetMasterContext();

            var trackedRoom = mc.Rooms.First(r => r.Id == "0");

            var a = mc.Add(trackedRoom);
            var b = a.Collection(r => r.RoomUsers).Query();
            var c = b.Select(ru => ru.User);

            // var q2 = mc.Entry(trackedRoom).Collection(r => r.RoomUsers).Query().Select(ru => ru.User);
            // var postCount = context.Entry(blog)
            //     .CollectUsion(b => b.Posts)
            //     .Query()
            //     .Count();

            var users = c.ToList();
            var str = c.ToQueryString();

            var users2 = mc.Users.ToList();
        }


        [Fact]
        public async Task GetDisplayUsers()
        {
            var mc = GetMasterContext();
            // var room = mc.Rooms.First(r => r.Id == "0");
            // var repo = new MasterRepo(mc);
            //
            // var du = repo.GetRoomDisplayUsersAsync(room);
            //
            // var a = mc.Entry(room).Collection(c => c.RoomUsers).Query().Select(ru => ru.User)
            //     .Select(DisplayUser.Projection).ToQueryString();
            // // .Select(DisplayUser.Projection).AsQueryable().ToQueryString();
            // ;

            var pRoom = mc.PendingRooms.First(pr => pr.Id == 1);

            var b = mc.Entry(pRoom)
                .Reference(pr => pr.Room)
                .Query()
                .SelectMany(c => c.RoomUsers)   
                .Select(ru => ru.User)
                .Select(DisplayUser.Projection).ToQueryString();
        }

        class A
        {
            public List<B> RoomUsers { get; set; }
        }

        class B
        {
            public List<C> Users { get; set; }
        }

        class C
        {
        }

        public class ProjectedUser
        {
            public string Id { get; set; }

            public string Email { get; set; }

            public bool IsActive { get; set; }
        }
    }
}