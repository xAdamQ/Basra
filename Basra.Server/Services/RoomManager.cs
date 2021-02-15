using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Basra.Server.Services
{
    public interface IRoomManager
    {
        Task AskForRoom(int genre, int playerCount, string userId, string connId);
        Task AddUser(PendingRoom pRoom, string userId, string connId);
    }

    public class RoomManager : IRoomManager
    {
        private readonly ILogger _logger;
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IMasterRepo _masterRepo;
        private readonly ServerLoop _serverLoop;

        public RoomManager(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo, ServerLoop serverLoop)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _serverLoop = serverLoop;
            // _logger = logger;
        }

        public async Task AskForRoom(int genre, int playerCount, string userId, string connId)
        {
            var room = _masterRepo.GetPendingRoomWithSpecs(genre, playerCount);
            if (room == null)
            {
                room = _masterRepo.MakeRoom(genre, playerCount); //should save
                _logger.LogInformation("a new room is made");
            }

            await AddUser(room, userId, connId);
        }

        public async Task AddUser(PendingRoom pRoom, string userId, string connId)
        {
            var rUser = _masterRepo.AddRoomUser(userId, connId, pRoom); //should save

            if (pRoom.UserCount == pRoom.EnteredUsers)
            {
                _logger.LogInformation("a room is ready and will start");
                // await Start(pRoom);
            }
            else
            {
                await _masterHub.Clients.User(rUser.UserId).SendAsync("RoomIsFilling");
            }
        }

        // public async Task Start(PendingRoom pRoom)
        // {
        //     var dUsers = _masterRepo.GetRoomDisplayUsersAsync(pRoom);
        //     var rUsers = new List<RoomUser>();
        //     //room user
        //     //display user
        //
        //     // var userNames = new string[pRoom.UserCount];
        //     // for (int i = 0; i < userNames.Length; i++)
        //     // {
        //     //     
        //     //     userNames[i] = await _masterRepo.GetNameOfUserAsync(Users[i].UserId);
        //     // }
        //     //var userNames = Users.Select(u => u.ActiveUser.Name).ToArray();
        //
        //     //start game
        //     //dinsplay users
        //
        //     var tasks = new List<Task>();
        //     for (int i = 0; i < pRoom.UserCount; i++)
        //     {
        //         tasks.Add(_masterHub.Groups.AddToGroupAsync(rUsers[i].ConnectionId, "room" + pRoom.RoomId));
        //
        //         _masterRepo.StartRoomUser(rUsers[i], i, pRoom.RoomId);
        //
        //         rUsers[i].IdInRoom = i;
        //         rUsers[i].RoomId = pRoom.RoomId;
        //
        //         tasks.Add(_masterHub.Clients.User(rUsers[i].UserId).SendAsync("StartRoom", i, rUsers));
        //
        //         // tasks.Add(Users[i].StartRoom(this, i, userNames));
        //     }
        //
        //     _masterRepo.SaveChanges();
        //
        //     await Task.WhenAll(tasks);
        //
        //     //server weq3
        //     // Task.Run()
        //     
        //     Users[0].StartTurn();
        //
        //     _masterRepo.RemovePendingRoom(pRoom);
        // }

        //mapping them to users


        // public async Task StartRoom(Room room, int id, string[] playerNames)
        // {
        //     IdInRoom = id;
        //     ActiveRoom = room;
        //
        //     await Program.HubContext.Clients.User(UserId).SendAsync("StartRoom", IdInRoom, playerNames);
        // }

        // public void StartTurn()
        // {
        //     TurnTimoutCancelation = new CancellationTokenSource();
        //     Task.Delay(HandTime * 1000).ContinueWith(t => RandomPlay(), TurnTimoutCancelation.Token);
        // }
    }
}