using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Data;
using Basra.Server.Exceptions;
using Basra.Server.Services;
using Castle.Components.DictionaryAdapter;
using Hangfire;
using Microsoft.Extensions.Logging;
using Moq;
using System.Text.Json;
using Xunit;
using Xunit.Abstractions;

namespace Basra.Server.Tests
{
    public class LobbyManagerTests
    {
        //create editable instance int terms of mocking
        private LobbyManager GetMockedLobby(Mock<ISessionRepo> sessionRepo = null, Mock<IMasterRepo> masterRepo = null,
            Mock<IRequestCache> requestCache = null)
        {
            sessionRepo ??= new Mock<ISessionRepo>();
            masterRepo ??= new Mock<IMasterRepo>();
            requestCache ??= new Mock<IRequestCache>();

            sessionRepo.Setup(_ => _.DoesRoomUserExist(It.IsAny<string>())).Returns(false);
            sessionRepo.SetReturnsDefault(new Room(0, 0));
            sessionRepo.SetReturnsDefault(new RoomUser());

            return new LobbyManager(masterRepo.Object, new Mock<IBackgroundJobClient>().Object, requestCache.Object);
        }

        private readonly ITestOutputHelper _testOutputHelper;
        public LobbyManagerTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        //tested as public, now they're private
        // [InlineData(0, 0)]
        // [InlineData(0, 1)]
        // [InlineData(2, 1)]
        // [InlineData(2, 0)]
        // [Theory]
        // public void ValidateRoomChoice_ShouldPass(int betChoice, int capacityChoice)
        // {
        //     var lobbyManager = GetMockedLobby();
        //
        //     lobbyManager.TakeOrCreateAppropriateRoom(betChoice, capacityChoice);
        // }
        //
        // [InlineData(2, -1)]
        // [InlineData(-2, 0)]
        // [InlineData(3, 0)]
        // [InlineData(3, 9)]
        // [Theory]
        // public void ValidateRoomChoice_ShouldFail(int betChoice, int capacityChoice)
        // {
        //     var sessionRepo = new Mock<ISessionRepo>();
        //     sessionRepo.Setup(_ => _.DoesRoomUserExist(It.IsAny<string>())).Returns(false);
        //
        //     var lobbyManager = GetMockedLobby();
        //
        //     Assert.Throws<BadUserInputException>(
        //         () => lobbyManager.TakeOrCreateAppropriateRoom(betChoice, capacityChoice));
        // }
        //
        // [Fact]
        // public void GenerateDeck()
        // {
        //     var lobbyManager = GetMockedLobby();
        //
        //     var room = new Room(0, 0);
        //
        //     lobbyManager.GenerateRoomDeck(room);
        //     _testOutputHelper.WriteLine($"deck values are: {string.Join(", ", room.Deck)}");
        //
        //     Assert.Equal(52, room.Deck.Count);
        //     Assert.NotEqual(room.Deck[0], room.Deck[1]);
        // }
        //
        // [Fact]
        // public void SetRoomUsersDomainGettingReady()
        // {
        //     var lobbyManager = GetMockedLobby();
        //     var room = new Room(0, 0)
        //     {
        //         RoomUsers = new List<RoomUser>
        //         {
        //             new RoomUser {Id = "tstId1"},
        //             new RoomUser {Id = "tstId2"},
        //         }
        //     };
        // }
        //
        // [Fact]
        // public async Task GetRoomDisplayUsers()
        // {
        //     var masterRepo = new Mock<IMasterRepo>();
        //     masterRepo.Setup(_ => _.GetDisplayUserAsync(It.IsAny<String>())).Returns<string>((id) =>
        //         Task.FromResult(new DisplayUser
        //         {
        //             Name = "tstName of id " + id,
        //             PlayedRooms = 13,
        //             Wins = 11,
        //         }));
        //     var lobbyManager = GetMockedLobby(masterRepo: masterRepo);
        //
        //     var room = new Room(0, 0)
        //     {
        //         RoomUsers = new List<RoomUser>
        //         {
        //             new RoomUser
        //             {
        //                 Id = "tstId 1"
        //             },
        //             new RoomUser
        //             {
        //                 Id = "tstId 2"
        //             },
        //         }
        //     };
        //     var dUsers = await lobbyManager.GetRoomDisplayUsers(room);
        //
        //     _testOutputHelper.WriteLine(string.Join(", ", dUsers.Select(user => user.Name)));
        //
        //     Assert.NotNull(dUsers);
        //     Assert.NotNull(dUsers[0]);
        //     Assert.NotNull(dUsers[1]);
        //     Assert.Equal("tstName of id " + room.RoomUsers[0].Id, dUsers[0].Name);
        // }

        [Fact]
        public async Task BuyCardback_ShouldFailNoMoney()
        {
            var requestCacheMock = new Mock<IRequestCache>();
            requestCacheMock.Setup(m => m.GetUser()).Returns(Task.FromResult(new User
            {
                Id = "tstId",
                Money = 7,
            }));

            var lobbyManager = GetMockedLobby(requestCache: requestCacheMock);

            await Assert.ThrowsAsync<BadUserInputException>(() => lobbyManager.BuyCardBack(0));
        }
        [Fact]
        public async Task BuyCardback_ShouldFailAlreadyBought()
        {
            var requestCacheMock = new Mock<IRequestCache>();
            requestCacheMock.Setup(m => m.GetUser()).Returns(Task.FromResult(new User
            {
                Id = "tstId",
                Money = 999999,
                OwnedCardBackIds = new List<int>() {0, 1, 2}
            }));

            var lobbyManager = GetMockedLobby(requestCache: requestCacheMock);

            await Assert.ThrowsAsync<BadUserInputException>(() => lobbyManager.BuyCardBack(1));
        }
        [Fact]
        public async Task BuyCardback_ShouldSucceedAndTakeMoney()
        {
            var requestCacheMock = new Mock<IRequestCache>();
            var user = new User
            {
                Id = "tstId",
                Money = 250,
                OwnedCardBackIds = new List<int>() {0, 2}
            };
            requestCacheMock.Setup(m => m.GetUser()).Returns(Task.FromResult(user));

            var lobbyManager = GetMockedLobby(requestCache: requestCacheMock);

            await lobbyManager.BuyCardBack(1);

            _testOutputHelper.WriteLine(JsonSerializer.Serialize(user));
            Assert.Equal(185, user.Money);
            Assert.Equal(new List<int> {0, 2, 1}, user.OwnedCardBackIds);
        }

        [Fact]
        public void DomainTest()
        {
            var t1 = typeof(UserDomain.App);
            var t2 = typeof(UserDomain.App.Lobby.Idle);

            Assert.True(t1.IsSubclassOf(t2));
        }
    }
}