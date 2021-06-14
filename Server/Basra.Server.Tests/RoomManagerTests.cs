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

            mi.Invoke(roomManager, new object[] { room });

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(roomDataUsers));
            if (winner == 0) Assert.True(roomDataUsers[0].Money > roomDataUsers[1].Money);
            else if (winner == 1) Assert.True(roomDataUsers[1].Money > roomDataUsers[0].Money);
            else if (winner == -1) Assert.True(roomDataUsers[1].Money == roomDataUsers[0].Money);
        }

        [Theory]
        [InlineData(new int[] { 0, 0 }, new int[] { 22, 30 }, new int[] { 1, 3 }, new int[] { 0, 0 }, -1, new int[] { 10, 60 })]
        [InlineData(new int[] { 0, 0 }, new int[] { 15, 52 - 15 }, new int[] { 1, 2 }, new int[] { 2, 0 }, -1, new int[] { 70, 50 })]
        [InlineData(new int[] { 0, 0 }, new int[] { 40, 12 }, new int[] { 1, 5 }, new int[] { 2, 9 }, 1, new int[] { 100, -1 })]
        [InlineData(new int[] { 0, 0 }, new int[] { 40, 12 }, new int[] { 1, 1 }, new int[] { 2, 0 }, 0, new int[] { -1, 40 })]
        [InlineData(new int[] { 0, 0 }, new int[] { 27, 27 }, new int[] { 1, 1 }, new int[] { 1, 1 }, -1, new int[] { 70, 70 })]
        [InlineData(new int[] { 0, 0 }, new int[] { 27, 27 }, new int[] { 0, 0 }, new int[] { 0, 0 }, 0, new int[] { -1, 30 })]
        [InlineData(new int[] { 0, 0, 1 }, new int[] { 27, 10, 17 }, new int[] { 0, 0, 1 }, new int[] { 0, 0, 0 }, -1, new int[] { 30, 0, 10 })]
        [InlineData(new int[] { 0, 0, 1 }, new int[] { 20, 12, 20 }, new int[] { 0, 0, 1 }, new int[] { 0, 0, 0 }, -1, new int[] { 30, 0, 40 })]
        public void CalcScore(int[] types, int[] eatenCounts, int[] basraCounts, int[] bigBasraCounts, int resignedUserInd, int[] scores)
        {
            var roomActors = new List<RoomActor>();
            for (int i = 0; i < eatenCounts.Length; i++)
            {
                if (types[i] == 0)
                    roomActors.Add(new RoomUser
                    {
                        EatenCardsCount = eatenCounts[i],
                        BasraCount = basraCounts[i],
                        BigBasraCount = bigBasraCounts[i],
                    });
                else
                    roomActors.Add(new RoomBot
                    {
                        EatenCardsCount = eatenCounts[i],
                        BasraCount = basraCounts[i],
                        BigBasraCount = bigBasraCounts[i],
                    });
            }

            var resignedUser = resignedUserInd == -1 ? null : roomActors[resignedUserInd];

            var mi = typeof(RoomManager).GetMethod("CalcScores", BindingFlags.Instance | BindingFlags.NonPublic);
            var roomManager = GetMockedRoomManager();

            var actualScores = (List<int>)mi.Invoke(roomManager, new Object[] { roomActors, resignedUser });

            Assert.Equal(scores, actualScores);

        }

        [Theory]
        [InlineData(0, 0, new int[] { 30, 10 }, new int[] { 30, 22 }, new int[] { 0, 1 }, new int[] { 0, 0 },
        new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 100, 0 }, new int[] { 100, 0 }, new int[] { 1, 0 })]
        [InlineData(1, 0, new int[] { 30, 10 }, new int[] { 30, 22 }, new int[] { 0, 1 }, new int[] { 0, 0 },
        new int[] { 0, 0 }, new int[] { 1, 0 }, new int[] { 1, 1 }, new int[] { 200, 0 }, new int[] { 200, 0 }, new int[] { 1, 0 })]
        [InlineData(1, 0, new int[] { -1, 10 }, new int[] { 30, 22 }, new int[] { 0, 1 }, new int[] { 1, 0 },
        new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 1 }, new int[] { 0, 200 }, new int[] { 0, 200 }, new int[] { 0, 1 })]
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

            var actualScores = (List<int>)mi.Invoke(roomManager, new Object[] { room, dUsers, scores.ToList() });

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

            await (Task)mi.Invoke(roomManager, new Object[] { user });

            Assert.Equal(user.Level, newLevel);
        } //testing by manual debugging
    }
}