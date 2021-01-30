//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.SignalR;

//namespace Basra.Server
//{
//    public class PendingRoom : IPendingRoom
//    {
//        public int Genre { get; }
//        public List<IRoomUser> Users { get; } = new List<IRoomUser>();
//        public int PlayerCount { get; }
//        public int Id { get; }
//        public static int LastId { get; set; }

//        public static List<PendingRoom> All { get; } = new List<PendingRoom>();

//        public PendingRoom(int genre, int playerCount)
//        {
//            Id = LastId++;
//            Genre = genre;
//            PlayerCount = playerCount;
//            All.Add(this);

//            System.Console.WriteLine($"a new room is made with genre {Genre} and playerCount {PlayerCount}");
//        }

//        public async static Task AskForRoom(MasterHub hub, int genre, int playerCount, IMasterRepo masterRepo)
//        {
//            PendingRoom pRoom = null;
//            try //find a room with this specs
//            {
//                pRoom = All.First(r => r.Genre == genre && r.PlayerCount == playerCount);
//            } //otherwise make new one
//            catch (InvalidOperationException)
//            {
//                pRoom = new PendingRoom(genre, playerCount);
//                Console.WriteLine("a new room is made");
//            }

//            var roomUser = new RoomUser
//            {
//                ConnectionId = hub.Context.ConnectionId,
//                UserId = hub.Context.UserIdentifier,
//            };

//            await pRoom.AddUser(hub, masterRepo, roomUser);
//        }

//        public async Task AddUser(MasterHub hub, IMasterRepo masterRepo, RoomUser rUser)
//        {
//            Users.RemoveAll(u => !masterRepo.GetUserActiveState(u.UserId));
//            Users.Add(rUser);

//            if (PlayerCount == Users.Count)
//            {
//                Console.WriteLine("congrates, a room is ready");

//                All.Remove(this);

//                var active = new ActiveRoom(this, masterRepo);
//                await active.Start();
//            }
//            else
//            {
//                await hub.Clients.Caller.SendAsync("RoomIsFilling");
//            }
//        }


//    }
//}