//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Basra.Server
//{
//    public class RoomManager
//    {
//        private readonly IMasterRepo _masterRepo;

//        public RoomManager(IMasterRepo masterRepo)
//        {
//            _masterRepo = masterRepo;
//        }

//        public async Task AskForRoom(int genre, int playerCount)
//        {
//            var pRoom = await _masterRepo.GetPendingRoomWithSpecs(genre, playerCount);
//            if (pRoom == null)
//            {
//                pRoom = await _masterRepo.CreatePendingRoom(genre, playerCount);
//                Console.WriteLine("a new room is made");
//            }

//            var roomUser = new RoomUser
//            {
//                ConnectionId = hub.Context.ConnectionId,
//                UserId = hub.Context.UserIdentifier,
//            };

//            await pRoom.AddUser(hub, masterRepo, roomUser);
//        }
//    }
//}
