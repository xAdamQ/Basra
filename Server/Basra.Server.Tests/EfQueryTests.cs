using System;
using System.Collections.Generic;
using System.Linq;
using Basra.Server.Data;
using Basra.Common;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Xunit.Abstractions;

namespace Basra.Server.Tests
{
    public class EfQueryTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public EfQueryTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        // private MasterContext GetMasterContext2()
        // {
        //     var options2 = new DbContextOptionsBuilder<MasterContext>().UseMySql(
        //         "Server=localhost; Port=3306; Uid=root; Pwd=23437075; Database=Basra;",
        //         new MySqlServerVersion(new Version(8, 0, 21)),
        //         mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)
        //     ).Options;
        //
        //     return new MasterContext(options2);
        // }

        private MasterContext GetMasterContext()
        {
            // var options2 = new DbContextOptionsBuilder<MasterContext>().UseMySql(
            //     "Server=localhost; Port=3306; Uid=root; Pwd=23437075; Database=Basra;",
            //     new MySqlServerVersion(new Version(8, 0, 21)),
            //     mySqlOptions => mySqlOptions.CharSetBehavior(CharSetBehavior.NeverAppend)
            // ).Options;

            var op = new DbContextOptionsBuilder<MasterContext>()
                .UseSqlServer("Server=localhost; Database=Basra; User Id=SA; Password=Mm23437075;")
                .Options;

            return new MasterContext(op);
        }

//         [Fact]
//         public async Task projection()
//         {
//             //arrange
//             var options = new DbContextOptionsBuilder<MasterContext>()
//                 .UseInMemoryDatabase(databaseName: "the_name_of_in_memory_db")
//                 .Options;
//
//             var masterContext = GetMasterContext();
//
//             //act
//             var q = masterContext.Users.Select(u => new ProjectedUser
//             {
//                 Id = u.Id,
//                 Email = u.Email,
//                 IsActive = u.IsActive,
//             });
// ////////////////this was projection
//
//             var str = q.ToQueryString();
//             //assert
//         }
//
//         [Fact]
//         public async Task selectselect()
//         {
//             // var q = trackedRoom.RoomUsers.Select(rs => rs.User).Select(DisplayUser.Projection);
//
//             var mc = GetMasterContext();
//
//             var trackedRoom = mc.Rooms.First(r => r.Id == "0");
//
//             var a = mc.Add(trackedRoom);
//             var b = a.Collection(r => r.RoomUsers).Query();
//             var c = b.Select(ru => ru.User);
//
//             // var q2 = mc.Entry(trackedRoom).Collection(r => r.RoomUsers).Query().Select(ru => ru.User);
//             // var postCount = context.Entry(blog)
//             //     .CollectUsion(b => b.Posts)
//             //     .Query()
//             //     .Count();
//
//             var users = c.ToList();
//             var str = c.ToQueryString();
//
//             var users2 = mc.Users.ToList();
//         }
//
//
//         [Fact]
//         public async Task GetDisplayUsers()
//         {
//             var mc = GetMasterContext();
//             // var room = mc.Rooms.First(r => r.Id == "0");
//             // var repo = new MasterRepo(mc);
//             //
//             // var du = repo.GetRoomDisplayUsersAsync(room);
//             //
//             // var a = mc.Entry(room).Collection(c => c.RoomUsers).Query().Select(ru => ru.User)
//             //     .Select(DisplayUser.Projection).ToQueryString();
//             // // .Select(DisplayUser.Projection).AsQueryable().ToQueryString();
//             // ;
//
//             var pRoom = mc.PendingRooms.First(pr => pr.Id == 1);
//
//             var b = mc.Entry(pRoom)
//                 .Reference(pr => pr.Room)
//                 .Query()
//                 .SelectMany(c => c.RoomUsers)
//                 .Select(ru => ru.User)
//                 .Select(DisplayUser.Projection).ToQueryString();
//         }
//
//         class A
//         {
//             public List<B> RoomUsers { get; set; }
//         }
//
//         class B
//         {
//             public List<C> Users { get; set; }
//         }
//
//         class C
//         {
//         }
//
//         public class ProjectedUser
//         {
//             public string Id { get; set; }
//
//             public string Email { get; set; }
//
//             public bool IsActive { get; set; }
//         }

        [Fact]
        public void TestQuery0012()
        {
            var q = GetMasterContext().Users.Where(_ => _.Id == "0").Select(DisplayUser.Projection);
            _testOutputHelper.WriteLine(q.ToQueryString());
        }

        [Fact]
        public void TestQuery0013()
        {
            var ids = new List<RoomUser>
            {
                new RoomUser
                {
                    Id = "0",
                    ConnectionId = "tstCoon"
                },
                new RoomUser
                {
                    Id = "0",
                    ConnectionId = "tstCoon"
                },
            };
            var ids2 = new List<string> {"1", "2", "3"};

            // var q4 = GetMasterContext().Users.Where(u => u.Id == "0").ToList();

            // var q2 = GetMasterContext().Users.GroupJoin(ids, u => u.Id, id => id, (user, s) => new {resId = s})
            // var b = GetMasterContext2().Users.Join(ids, u => u.Id, ru => ru.Id, (u, id) => new {u, id}).ToList();
            // var c = GetMasterContext().Users.Where(u => ids2.Contains(u.Id)).Take(ids2.Count)
            //     .Select(FullUserInfo.Projection);


            // .ToList();
            // .Take(1);
            // .ToList();

            // var l = ids2.Join(ids, u => u, id => id, (user, s) => new {resId = s}).ToList();

            // _testOutputHelper.WriteLine(String.Join(", ", q4));

            // .Select(FullUserInfo.Projection);

            // var q = GetMasterContext().Users.Where(_ => _.Id == "0").Select(DisplayUser.Projection);
            // _testOutputHelper.WriteLine(c.ToQueryString());
        }
    }
}