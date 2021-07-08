using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Moq;
using System.Text.Json;
using System.Threading;
using Basra.Models.Client;
using Basra.Server.Extensions;
using Basra.Server.Tests;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;


namespace Basra.Server.Services
{
    public class FinalizeManagerTests
    {
        private readonly ITestOutputHelper _testOutputHelper;
        public FinalizeManagerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Theory]
        [InlineData(30, 22, 1, 4, 0, 0, -1)]
        [InlineData(30, 22, 0, 4, 0, 0, 1)]
        [InlineData(30, 22, 0, 1, 0, 0, 0)]
        [InlineData(20, 32, 1, 1, 1, 0, -1)]
        [InlineData(20, 32, 1, 0, 1, 0, 0)]
        public async Task FinalizeGame_RightWinner(int eaten1, int eaten2, int basra1, int basra2, int bbasra1, int bbasra2, int winner)
        {
            var roomDataUsers = new List<User>()
            {
                new(),
                new(),
            };
            //every int is 0

            var roomUsers = new List<RoomUser>
            {
                new()
                {
                    EatenCardsCount = eaten1,
                    BasraCount = basra1,
                    BigBasraCount = bbasra1,
                    ActiveUser = new("0", "00", typeof(UserDomain.App)),
                },
                new()
                {
                    EatenCardsCount = eaten2,
                    BasraCount = basra2,
                    BigBasraCount = bbasra2,
                    ActiveUser = new("0", "00", typeof(UserDomain.App)),
                }
            };

            var room = new Room(0, 0);
            room.RoomUsers.AddRange(roomUsers);
            room.RoomActors.AddRange(roomUsers);

            room.LastEater = roomUsers[0];
            room.GroundCards = new List<int>();

            var masterRepoMock = new Mock<IMasterRepo>();
            masterRepoMock.Setup(mr => mr.GetUsersByIds(It.IsAny<List<string>>()))
                .Returns(() => Task.FromResult(roomDataUsers));

            var finMan = new FinalizeManager(MasterHubTests.GetMockWithSendFuns().Object, masterRepoMock.Object, new Mock<ISessionRepo>()
                .Object, new Mock<ILogger<FinalizeManager>>().Object);

            await finMan.FinalizeRoom(room);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(roomDataUsers));

            if (winner == 0) Assert.True(roomDataUsers[0].Money > roomDataUsers[1].Money);
            else if (winner == 1) Assert.True(roomDataUsers[1].Money > roomDataUsers[0].Money);
            else if (winner == -1) Assert.Equal(roomDataUsers[1].Money, roomDataUsers[0].Money);
        }

        [Theory]
        [InlineData(new int[] {0, 0}, new int[] {22, 30}, new int[] {1, 3}, new int[] {0, 0}, -1, new int[] {10, 60})]
        [InlineData(new int[] {0, 0}, new int[] {15, 52 - 15}, new int[] {1, 2}, new int[] {2, 0}, -1, new int[] {70, 50})]
        [InlineData(new int[] {0, 0}, new int[] {40, 12}, new int[] {1, 5}, new int[] {2, 9}, 1, new int[] {100, -1})]
        [InlineData(new int[] {0, 0}, new int[] {40, 12}, new int[] {1, 1}, new int[] {2, 0}, 0, new int[] {-1, 40})]
        [InlineData(new int[] {0, 0}, new int[] {27, 27}, new int[] {1, 1}, new int[] {1, 1}, -1, new int[] {70, 70})]
        [InlineData(new int[] {0, 0}, new int[] {27, 27}, new int[] {0, 0}, new int[] {0, 0}, 0, new int[] {-1, 30})]
        [InlineData(new int[] {0, 0, 1}, new int[] {27, 10, 17}, new int[] {0, 0, 1}, new int[] {0, 0, 0}, -1, new int[] {30, 0, 10})]
        [InlineData(new int[] {0, 0, 1}, new int[] {20, 12, 20}, new int[] {0, 0, 1}, new int[] {0, 0, 0}, -1, new int[] {30, 0, 40})]
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

            var finMan = new FinalizeManager(MasterHubTests.GetMockWithSendFuns().Object, new Mock<IMasterRepo>().Object, new
                Mock<ISessionRepo>().Object, new Mock<ILogger<FinalizeManager>>().Object);

            var actualScores = (List<int>) TestHelper.CallPrivateMethod("CalcScores", finMan, new Object[] {roomActors, resignedUser});

            Assert.Equal(scores, actualScores);
        }

        [Fact]
        public async Task SendResult()
        {
            var roomUsers = new List<RoomUser>()
            {
                new()
                {
                    Id = "4",
                    TurnId = 0,
                },
                new()
                {
                    Id = "9",
                    TurnId = 1,
                },
            };

            var dataUsers = new List<User>()
            {
                new()
                {
                    Id = "9",
                    Name = "nine",
                },
                new()
                {
                    Id = "7",
                    Name = "seven",
                },
                new()
                {
                    Id = "4",
                    Name = "four",
                }
            };

            var xpRep = new List<RoomXpReport>()
            {
                new()
                {
                    Competition = 44
                },
                new()
                {
                    Competition = 99
                }
            };

            var passedArgs = new List<FinalizeResult>();

            var hub = new Mock<IHubContext<MasterHub>>();
            var hubClient = new Mock<IHubClients>();
            var clientProxy = new Mock<IClientProxy>();

            clientProxy.Setup(_ => _.SendCoreAsync("FinalizeRoom", It.IsAny<object[]>(), CancellationToken.None))
                .Callback<string, object[], CancellationToken>((_, args, _) => passedArgs.Add((FinalizeResult) args[0]));
            hubClient.Setup(_ => _.GroupExcept(It.IsAny<string>(), It.IsAny<IReadOnlyList<string>>()))
                .Returns(clientProxy.Object);
            hubClient.Setup(_ => _.User(It.IsAny<string>())).Returns(clientProxy.Object);

            hub.Setup(_ => _.Clients).Returns(hubClient.Object);


            var fm = new FinalizeManager(hub.Object, new Mock<IMasterRepo>().Object, new
                Mock<ISessionRepo>().Object, new Mock<ILogger<FinalizeManager>>().Object);

            await TestHelper.CallAsyncPrivateAction("SendFinalizeResult", fm, new object[] {roomUsers, dataUsers, xpRep, 0});


            Assert.Equal(xpRep[0], passedArgs[0].RoomXpReport);
            Assert.Equal(roomUsers[0].Id, passedArgs[0].PersonalFullUserInfo.Id);

            Assert.Equal(xpRep[1], passedArgs[1].RoomXpReport);
            Assert.Equal(roomUsers[1].Id, passedArgs[1].PersonalFullUserInfo.Id);
        }
    }
}