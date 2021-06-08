using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Basra.Server.Data;
using Basra.Server.Exceptions;
using Basra.Server.Services;
using Castle.Components.DictionaryAdapter;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Basra.Server.Extensions;
using Xunit;
using Xunit.Abstractions;


namespace Basra.Server.Tests
{
    public class RoomManagerTests
    {
        private RoomManager GetMockedRoomManager(Mock<ISessionRepo> sessionRepo = null, Mock<IMasterRepo> masterRepo =
                null,
            Mock<IRequestCache> requestCache = null)
        {
            sessionRepo ??= new Mock<ISessionRepo>();
            masterRepo ??= new Mock<IMasterRepo>();

            sessionRepo.Setup(_ => _.DoesRoomUserExist(It.IsAny<string>())).Returns(false);
            sessionRepo.SetReturnsDefault(new Room(0, 0));
            sessionRepo.SetReturnsDefault(new RoomUser());

            return new RoomManager(MasterHubTests.GetMockWithSendFuns().Object, masterRepo.Object, sessionRepo.Object,
                new Mock<IServerLoop>().Object, new Mock<ILogger<RoomManager>>().Object);
        }

        private readonly ITestOutputHelper _testOutputHelper;
        public RoomManagerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(30, 22, 1, 4, 0, 0, -1)]
        [InlineData(30, 22, 0, 4, 0, 0, 1)]
        [InlineData(30, 22, 0, 1, 0, 0, 0)]
        [InlineData(20, 32, 1, 1, 1, 0, -1)]
        [InlineData(20, 32, 1, 0, 1, 0, 0)]
        public void FinalizeGame(int eaten1, int eaten2, int basra1, int basra2, int bbasra1, int bbasra2, int winner)
        {
            var roomDataUsers = new List<User>()
            {
                new User(),
                new User(),
            };
            //every int is 0

            var roomUsers = new List<RoomUser>
            {
                new RoomUser
                {
                    EatenCardsCount = eaten1,
                    BasraCount = basra1,
                    BigBasraCount = bbasra1,
                },
                new RoomUser
                {
                    EatenCardsCount = eaten2,
                    BasraCount = basra2,
                    BigBasraCount = bbasra2,
                }
            };

            var room = new Room(0, 0);
            room.RoomUsers.AddRange(roomUsers);

            var masterRepoMock = new Mock<IMasterRepo>();
            masterRepoMock.Setup(mr => mr.GetUsersByIds(It.IsAny<List<string>>()))
                .Returns(() => Task.FromResult(roomDataUsers));

            var mi = typeof(RoomManager).GetMethod("FinalizeGame", BindingFlags.Instance | BindingFlags.NonPublic);

            var roomManager = GetMockedRoomManager(masterRepo: masterRepoMock);

            mi.Invoke(roomManager, new object[] {room});

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(roomDataUsers));
            if (winner == 0) Assert.True(roomDataUsers[0].Money > roomDataUsers[1].Money);
            else if (winner == 1) Assert.True(roomDataUsers[1].Money > roomDataUsers[0].Money);
            else if (winner == -1) Assert.True(roomDataUsers[1].Money == roomDataUsers[0].Money);
        }

        [Theory]
        [InlineData(3000, 15, 16)]
        [InlineData(20000, 20, 16)]
        public void LevelWorks(int newXp, int level, int newLevel)
        {
            var user = new User()
            {
                XP = newXp,
                Level = level,
            };

            var mi = typeof(RoomManager).GetMethod("LevelWorks", BindingFlags.Instance | BindingFlags.NonPublic);

            var roomManager = GetMockedRoomManager();

            mi.InvokeAsync(roomManager, new object[] {user});

            // Assert.Equal(user.Level, newLevel);
        } //testing by manual debugging
    }
}