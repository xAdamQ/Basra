using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Data;
using Microsoft.AspNetCore.SignalR;

namespace Basra.Server
{
    public class RoomManager
    {
        private readonly IMasterRepo _masterRepo;
        private readonly IHubContext<MasterHub> _hubContext;

        public RoomManager(IMasterRepo masterRepo, IHubContext<MasterHub> hubContext)
        {
            _masterRepo = masterRepo;
            _hubContext = hubContext;
        }

        public async Task AskForRoom(int genre, int userCount, string userId, string connId)
        {
            var room = await _masterRepo.GetRoomWithSpecs(genre, userCount);
            if (room == null)
            {
                room = await _masterRepo.CreateRoom(genre, userCount);
                Console.WriteLine("a new room is made");
            }

            await _masterRepo.CreateRoomUserAsync(userId, connId, room.Id);

            room.EnteredUserCount++;

            if (room.EnteredUserCount == userCount)
            {
                await StartRoom(room);
            }
            else
            {
                await _hubContext.Clients.User(userId).SendAsync("RoomIsFilling");
            }

            _masterRepo.SaveChanges();

            //public async Task AddUserToRoom()
            //{
            //    Users.RemoveAll(u => !masterRepo.GetUserActiveState(u.UserId));
            //    Users.Add(rUser);

            //    if (PlayerCount == Users.Count)
            //    {
            //        Console.WriteLine("congrates, a room is ready");

            //        All.Remove(this);

            //        var active = new ActiveRoom(this, masterRepo);
            //        await active.Start();
            //    }
            //    else
            //    {
            //        await hub.Clients.Caller.SendAsync("RoomIsFilling");
            //    }
            //}


            //rpc
        }

        public async Task Start(Room room)
        {
            var roomUsers = new RoomUser[room.UserCount];
            var userNames = new string[room.UserCount];
            for (int i = 0; i < room.UserCount; i++)
            {
                userNames[i] = await _masterRepo.GetUserNameAsync(room.UsersIds[i]);
            }

            var usersConnIds = await _masterRepo.GetRoomUserConnIds(room.Id);
            var tasks = new List<Task>();
            for (int i = 0; i < room.UserCount; i++)
            {
                tasks.Add(Program.HubContext.Groups.AddToGroupAsync(usersConnIds[i], "room" + room.Id));

                ///***should set TurnId
                //TurnId = id;
                //await Program.HubContext.Clients.Client(ConnectionId).SendAsync("StartRoom", TurnId, playerNames);

                //tasks.Add(Users[i].StartRoom(this, i, userNames));
            }

            await Task.WhenAll(tasks);

            Users[0].StartTurn();

            _masterRepo.SaveChanges();
        }

        //public async Task StartRoom(Room room, int id, string[] playerNames)
        //{
        //    //TurnId = id;

        //    //await Program.HubContext.Clients.Client(ConnectionId).SendAsync("StartRoom", TurnId, playerNames);
        //}

        public void StartTurn()
        {
            TurnTimoutCancelation = new CancellationTokenSource();
            Task.Delay(HandTime * 1000).ContinueWith(t => RandomPlay(), TurnTimoutCancelation.Token);
        }

        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        public async Task Ready(string userId)
        {
            await _masterRepo.SetRoomUserReadyState(userId);
            _masterRepo.SaveChanges();
            await CheckPlayersAreReady();
        }

        //public async Task CheckPlayersAreReady(Room activeRoom)
        //{
        //    var allReady = true;
        //    foreach (var userId in activeRoom.UsersIds)
        //    {
        //        if (!_masterRepo.GetUserActiveState(userId))
        //        {
        //            allReady = false;
        //            break;
        //        }
        //    }

        //    if (allReady)
        //    {
        //        await InitialDistribute(activeRoom);
        //    }
        //}

    }
}
