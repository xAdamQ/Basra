using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Basra.Server.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Basra.Server.Tests
{
    public class RoomTests
    {
        // many users come in and ask for a room
        // //they require different rooom specs
        // //they can disconnect during
        // //1- call ask for room serveral times
        // //2- setup the scenario yourslef

        //
        // public static IEnumerable<object[]> AskForRoom_ShouldWork_Data => new[]
        // {
        //     new object[] {"tstConnId2", "tstUserId2", 0, 2},
        // };

        // All list >> gerner and player count
        //connected users active state in the selected room
        //users count
        // [Theory]
        // [MemberData(nameof(AskForRoom_ShouldWork_Data))]
        // public async Task AskForRoom_ShouldWork(string connId, string uId, int targetRoomGenre, int targetRoomUCount,
        //     List<IRoom> rooms)
        // {
        //     //arrange
        //     var options = new DbContextOptionsBuilder<MasterContext>()
        //         .UseInMemoryDatabase(databaseName: "the_name_of_in_memory_db")
        //         .Options;
        //
        //     var masterContext = new MasterContext(options);
        //     masterContext.AddRange(rooms);
        //
        //     var masterRepo = new MasterRepo(masterContext);
        //
        //     var hub = new Mock<IHubContext<MasterHub>>(); //you mock what you don;t test, like sendAsync of hub context!
        //     var roomManager = new RoomManager(hub.Object, masterRepo, new Mock<ILogger>().Object);
        //
        //     //act
        //     await roomManager.AskForRoom(targetRoomGenre, targetRoomUCount, uId, connId);
        //
        //     //assert
        //     // Assert.Equal(masterRepo., 1);
        // }
        //
        // [Theory]
        // [MemberData(nameof(AskForRoom_ShouldWork_Data))]
        // public void Ask_ShouldMakeNew(int genre, int capacity, string userId, string connId,
        //     List<(int, int)> pendingRooms)
        // {
        //     // var sessionRepo = new SessionRepo();
        //     // pendingRooms.ForEach(room => sessionRepo.MakeRoom(room.Item1, room.Item2));
        //     //
        //     // var roomManager =
        //     //     new RoomManager(new Mock<IHubContext<MasterHub>>().Object, new Mock<IMasterRepo>().Object,
        //     //         sessionRepo, new Mock<IServerLoop>().Object);
        //     //
        //     //
        //     // roomManager.Ask(genre, capacity, userId, connId);
        //     //
        //     //
        //     // roomManager.
        // }

        public static IRoomManager GetRoomManager() => new RoomManager(new Mock<ILogger<RoomManager>>().Object,
            new Mock<IHubContext<MasterHub>>().Object, new Mock<IMasterRepo>().Object, new Mock<ISessionRepo>().Object,
            new Mock<IServerLoop>().Object);

        [Fact]
        public void Scenario1()
        {
            var roomManager = new RoomManager(new Mock<ILogger<RoomManager>>().Object,
                new Mock<IHubContext<MasterHub>>().Object, new Mock<IMasterRepo>().Object,
                new Mock<ISessionRepo>().Object,
                new Mock<IServerLoop>().Object);

            // roomManager.RequestRoom();
        }

        [Fact]
        public void RequestRoom()
        {
            var rm = GetRoomManager();

            rm.RequestRoom(0, 0, 0, "uId", "connId");
        }

        public void StartTurn()
        {
        }

        //why in memory db is very good in this situation!
        //because you test queries which makes sense, you don't mock the repo and miss the chance to test it!
    }
}