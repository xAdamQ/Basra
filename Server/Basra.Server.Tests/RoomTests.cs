// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.SignalR;
// using Microsoft.EntityFrameworkCore;
// using Moq;
// using Xunit;
// using Basra.Server.Services;
// using Microsoft.Extensions.Caching.Memory;
// using Microsoft.Extensions.Logging;
//
// namespace Basra.Server.Tests
// {
//     public class RoomTests
//     {
//         public static IRoomManager GetRoomManager()
//         {
//             return new RoomManager(new Mock<ILogger<RoomManager>>().Object, MasterHubTests.GetMockWithSendFuns().Object,
//                 new Mock<IMasterRepo>().Object, new Mock<ISessionRepo>().Object, new Mock<IServerLoop>().Object);
//         }
//
//         public static IEnumerable<object[]> Play_ShouldEatRight_Data => new[]
//         {
//             new object[] {48, new List<int> {40, 42, 44, 47}, new List<int> {42, 44}, false, false}, //normal
//             new object[] {18, new List<int> {40, 42, 44, 47}, new List<int> {40, 42}, false, false}, //normal
//             new object[] {36, new List<int> {40, 42, 44, 47}, new List<int> {40, 42, 44, 47}, false, false}, //boy
//             new object[] {19, new List<int> {47}, new List<int> {47}, true, false}, //basra komi
//             new object[] {19, new List<int> { }, new List<int> { }, false, false}, //empty ground
//         };
//
//         [Theory]
//         [MemberData(nameof(Play_ShouldEatRight_Data))]
//         public async Task Play_ShouldEatRight(int cardId, List<int> ground, List<int> eaten, bool basra, bool bigBasra)
//         {
//             var rm = GetRoomManager();
//             var ru = new RoomUser
//             {
//                 ConnectionId = "0",
//                 Hand = new List<int> { cardId },
//                 TurnId = 0,
//                 Room = new Room(0, 0)
//                 {
//                     GroundCards = ground,
//                     CurrentTurn = 0,
//                     Id = 0,
//                 },
//             };
//             ru.Room.RoomUsers = new List<RoomUser> { ru, ru };
//
//             await rm.Play(ru, 0);
//
//             Assert.True(!eaten.Any(c => ru.Room.GroundCards.Contains(c)));
//         }
//
//         public static IEnumerable<object[]> FinalizeGame_Data => new object[][]
//         {
//             new object[]
//             {
//                 0, 0, new int[] {20, 32}, new int[] {1, 0}, new int[] {0, 0}, new int[] {0, 0}, new int[] {0, 1},
//                 new int[] {0, 100}
//             },
//             new object[]
//             {
//                 0, 0, new int[] {31, 31}, new int[] {0, 0}, new int[] {0, 0}, new int[] {1, 1}, new int[] {0, 0},
//                 new int[] {50, 50}
//             },
//             new object[]
//             {
//                 0, 0, new int[] {12, 40}, new int[] {4, 0}, new int[] {0, 0}, new int[] {0, 0}, new int[] {1, 0},
//                 new int[] {100, 0}
//             },
//         };
//
//         [Theory]
//         [MemberData(nameof(FinalizeGame_Data))]
//         public async Task FinalizeGame_ScoreAndWinnersCheck(int betChoice, int capacityChoice, int[] eatenCounts,
//             int[] basraCounts, int[] bigBasraCounts, int[] addedDraws, int[] addedWins, int[] addedMoney)
//         {
//             //declare data
//             var capacity = Room.Capacities[capacityChoice];
//             var room = new Room(0, capacityChoice);
//             room.RoomUsers = new List<RoomUser>();
//             var dbUsers = new Dictionary<string, User>();
//             for (var i = 0; i < capacity; i++)
//             {
//                 dbUsers.Add(i.ToString(), new User());
//
//                 room.RoomUsers.Add(new RoomUser
//                 {
//                     UserId = i.ToString(),
//                     EatenCardsCount = eatenCounts[i],
//                     BasraCount = basraCounts[i],
//                     BigBasraCount = bigBasraCounts[i],
//                 });
//             }
//
//             var mr = new Mock<IMasterRepo>();
//             mr.Setup(_ => _.GetUserByIdAsyc(It.IsAny<string>())).Returns<string>(_ => Task.FromResult(dbUsers[_]));
//
//             var rm = new RoomManager(new Mock<ILogger<RoomManager>>().Object,
//                 MasterHubTests.GetMockWithSendFuns().Object,
//                 mr.Object, new Mock<ISessionRepo>().Object, new Mock<IServerLoop>().Object);
//
//             await rm.FinalizeGame(room);
//
//             Assert.Equal(addedDraws, dbUsers.Select(u => u.Value.Draws).ToArray());
//             Assert.Equal(addedWins, dbUsers.Select(u => u.Value.Wins).ToArray());
//             Assert.Equal(addedMoney, dbUsers.Select(u => u.Value.Money).ToArray());
//         }
//
//
//         // public static IEnumerable<object[]> RequestRoom_ShouldStart_Data => new object[][]
//         // {
//         //     new object[] { },
//         // };
//
//
//         // [Theory]
//         // [MemberData(nameof(RequestRoom_ShouldStart_Data))]
//         // public async Task RequestRoom_ShouldStart(int player1Id, int player2Fbid, int betChoice, int capacityChoice)
//         // {
//         //     //declare data
//         //     var capacity = Room.Capacities[capacityChoice];
//         //     var room = new Room(0, capacityChoice);
//         //     room.RoomUsers = new List<RoomUser>();
//         //     var dbUsers = new Dictionary<string, User>();
//         //     for (var i = 0; i < capacity; i++)
//         //     {
//         //         dbUsers.Add(i.ToString(), new User());
//
//         //         room.RoomUsers.Add(new RoomUser
//         //         {
//         //             UserId = i.ToString(),
//         //             EatenCardsCount = eatenCounts[i],
//         //             BasraCount = basraCounts[i],
//         //             BigBasraCount = bigBasraCounts[i],
//         //         });
//         //     }
//
//         //     var mr = new Mock<IMasterRepo>();
//         //     mr.Setup(_ => _.GetUserByIdAsyc(It.IsAny<string>())).Returns<string>(_ => Task.FromResult(dbUsers[_]));
//
//         //     var rm = new RoomManager(new Mock<ILogger<RoomManager>>().Object,
//         //         MasterHubTests.GetMockWithSendFuns().Object,
//         //         mr.Object, new Mock<ISessionRepo>().Object, new Mock<IServerLoop>().Object);
//
//         //     await rm.FinalizeGame(room);
//
//         //     Assert.Equal(addedDraws, dbUsers.Select(u => u.Value.Draws).ToArray());
//         //     Assert.Equal(addedWins, dbUsers.Select(u => u.Value.Wins).ToArray());
//         //     Assert.Equal(addedMoney, dbUsers.Select(u => u.Value.Money).ToArray());
//         // }
//
//     }
// }

