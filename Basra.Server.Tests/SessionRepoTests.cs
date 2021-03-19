// using System.Collections.Concurrent;
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
// using NuGet.Frameworks;
//
// namespace Basra.Server.Tests
// {
//     public class SessionRepoTests
//     {
//         [Fact]
//         public void MakeAndGetPendingRoom()
//         {
//             var newRoomsData = new[]
//             {
//                 new[] {0, 0},
//                 new[] {1, 1},
//                 new[] {1, 1},
//                 new[] {0, 0},
//             };
//
//             var sessionRepo = new SessionRepo();
//
//             foreach (var roomData in newRoomsData)
//             {
//                 sessionRepo.MakeRoom(roomData[0], roomData[0]);
//             }
//
//
//             // var cb = new ConcurrentBag<int>();
//             // cb.Add(1);
//             // cb.Add(2);
//             // cb.Add(3);
//
//             // cb.TryTake(out int res);
//             // Assert.NotEqual(res, 0);
//             // Assert.Equal(res, 3);
//             var r1 = sessionRepo.GetPendingRoom(0, 0);
//             Assert.NotEqual(r1, null);
//             Assert.NotEqual(sessionRepo.GetPendingRoom(0, 0), null);
//
//             sessionRepo.KeepRoom(r1);
//             Assert.NotEqual(sessionRepo.GetPendingRoom(0, 0), null);
//
//             // var PendingRooms = new ConcurrentDictionary<(int, int), ConcurrentBag<int>>();
//             // for (int i = 0; i < 4; i++)
//             // {
//             //     for (int j = 0; j < 3; j++)
//             //     {
//             //         PendingRooms.TryAdd((i, j), new ConcurrentBag<int>());
//             //     }
//             // }
//
//             // PendingRooms[(0,0)]
//         }
//     }
// }