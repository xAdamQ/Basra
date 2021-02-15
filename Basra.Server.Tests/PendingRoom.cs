// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Text;
// using System.Threading;
// using System.Threading.Tasks;
// using Microsoft.AspNetCore.SignalR;
// using Moq;
// using Xunit;
//
// namespace Basra.Server.Tests
// {
//     public class PendingRoom
//     {
//         //many users come in and ask for a room
//         //they require different rooom specs
//         //they can disconnect during
//         //1- call ask for room serveral times
//         //2- setup the scenario yourslef
//         public static IEnumerable<object[]> AskForRoom_ShouldWork_Data => new[]
//         {
//             new object[] {"tstConnId1", "tstUserId1", 0, 2},
//             new object[] {"tstConnId2", "tstUserId2", 0, 2},
//         };
//         //All list >> gerner and player count
//         //connected users active state in the selected room
//         //users count
//
//         [Theory]
//         [MemberData(nameof(AskForRoom_ShouldWork_Data))]
//         public async Task AskForRoom_ShouldWork(string connId, string uId, int targetRoomGenre, int targetRoomUCount)//, int[] currentRoomsGenres, int[] curretnRoomsUCounts, bool[] activeState, int userCount)
//         {
//             var context = new Mock<HubCallerContext>();
//             context.Setup(c => c.ConnectionId).Returns(connId);
//             context.Setup(c => c.UserIdentifier).Returns(uId);
//
//             //send async is called on a mocked proxy and returned a successful task, is that a default behaviour?
//             //caller.Setup(c => c.SendAsync("RoomIsFilling", CancellationToken.None)).Returns(Task.CompletedTask);
//
//             var clients = new Mock<IHubCallerClients>();
//             var caller = new Mock<IClientProxy>();
//             clients.Setup(hcc => hcc.Caller).Returns(caller.Object);
//
//             var masterRepo = new Mock<IMasterRepo>();
//             masterRepo.Setup(mr => mr.GetUserActiveState(connId)).Returns(true);
//
//             var hub = new MasterHub(masterRepo.Object, new RoomFactory())
//             {
//                 Context = context.Object,
//                 Clients = clients.Object,
//             };
//
//             await Server.Room.AskForRoom(hub, new RoomFactory(), masterRepo.Object, targetRoomGenre, targetRoomUCount);
//
// #pragma warning disable xUnit2000 
//             // Constants and literals should be the expected argument
//             Assert.Equal(Server.Room.All.Count, 1);
//             //        System.NotSupportedException : Unsupported expression:
//             //Extension methods(here: ClientProxyExtensions.SendAsync) may not be used in setup / verification expressions.
//             // Constants and literals should be the expected argument
// #pragma warning restore xUnit2000 
//         }
//     }
// }
