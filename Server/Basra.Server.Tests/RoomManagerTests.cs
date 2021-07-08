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

            return new RoomManager(MasterHubTests.GetMockWithSendFuns().Object, masterRepo.Object,
                new Mock<IServerLoop>().Object, new Mock<ILogger<RoomManager>>().Object, new Mock<IFinalizeManager>().Object);
        }

        private readonly ITestOutputHelper _testOutputHelper;
        public RoomManagerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

   
     
 
        [Theory]
        [InlineData(0, 0, new int[] {30, 10}, new int[] {30, 22}, new int[] {0, 1}, new int[] {0, 0},
            new int[] {0, 0}, new int[] {1, 0}, new int[] {1, 1}, new int[] {100, 0}, new int[] {100, 0}, new int[] {1, 0})]
        [InlineData(1, 0, new int[] {30, 10}, new int[] {30, 22}, new int[] {0, 1}, new int[] {0, 0},
            new int[] {0, 0}, new int[] {1, 0}, new int[] {1, 1}, new int[] {200, 0}, new int[] {200, 0}, new int[] {1, 0})]
        [InlineData(1, 0, new int[] {-1, 10}, new int[] {30, 22}, new int[] {0, 1}, new int[] {1, 0},
            new int[] {0, 0}, new int[] {0, 1}, new int[] {1, 1}, new int[] {0, 200}, new int[] {0, 200}, new int[] {0, 1})]
        public void SetUserState(int betC, int capC, int[] scores, int[] eatens, int[] basras, int[] bbasras,
            int[] draws, int[] wins, int[] played, int[] moneys, int[] totalEarned, int[] winStreak)
        {
            var room = new Room(betC, capC);
            var cap = Room.Capacities[capC];

            for (int i = 0; i < cap; i++)
            {
                room.RoomActors.Add(new RoomUser
                {
                    EatenCardsCount = eatens[i],
                    BasraCount = basras[i],
                    BigBasraCount = bbasras[i],
                });
            }


            var dUsers = new List<User>();
            for (int i = 0; i < cap; i++) dUsers.Add(new User());


            var mi = typeof(RoomManager).GetMethod("UpdateUserStates", BindingFlags.Instance | BindingFlags.NonPublic);
            var roomManager = GetMockedRoomManager();

            var actualScores = (List<int>) mi.Invoke(roomManager, new Object[] {room, dUsers, scores.ToList()});

            // Assert.Equal(xps, dUsers.Select(u => u.XP));
            Assert.Equal(draws, dUsers.Select(u => u.Draws));
            Assert.Equal(wins, dUsers.Select(u => u.WonRoomsCount));
            Assert.Equal(played, dUsers.Select(u => u.PlayedRoomsCount));
            Assert.Equal(moneys, dUsers.Select(u => u.Money));
            Assert.Equal(totalEarned, dUsers.Select(u => u.TotalEarnedMoney));
            Assert.Equal(winStreak, dUsers.Select(u => u.WinStreak));
        }

        [Theory]
        [InlineData(3000, 6, 8)]
        [InlineData(20000, 20, 23)]
        public async Task LevelWorks(int newXp, int level, int newLevel)
        {
            var user = new User()
            {
                XP = newXp,
                Level = level,
            };

            var mi = typeof(RoomManager).GetMethod("LevelWorks", BindingFlags.Instance | BindingFlags.NonPublic);

            var roomManager = GetMockedRoomManager();

            await (Task) mi.Invoke(roomManager, new Object[] {user});

            Assert.Equal(user.Level, newLevel);
        } //testing by manual debugging
    }
}