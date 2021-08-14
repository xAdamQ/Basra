using Basra.Common;
using Basra.Server.Exceptions;
using Basra.Server.Extensions;
using Basra.Server.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basra.Server.Services
{
    public interface IMatchMaker
    {
        Task RequestRandomRoom(int betChoice, int capacityChoice, ActiveUser activeUser);
        /// <summary>
        /// called by timeout
        /// </summary>
        Task FillPendingRoomWithBots(Room room);
        Task MakeRoomUserReadyRpc(ActiveUser activeUser, RoomUser roomUser);
        void RemovePendingDisconnectedUser(RoomUser roomUser);
    }

    public class MatchMaker : IMatchMaker
    {
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IRoomManager _roomManager;
        private readonly IServerLoop _serverLoop;
        private readonly ILogger<MatchMaker> _logger;
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;

        public MatchMaker(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo, ISessionRepo sessionRepo,
            IRoomManager roomManager, IServerLoop serverLoop, ILogger<MatchMaker> logger)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _roomManager = roomManager;
            _serverLoop = serverLoop;
            _logger = logger;
        }

        public async Task RequestRandomRoom(int betChoice, int capacityChoice, ActiveUser activeUser)
        {
            if (!betChoice.IsInRange(Room.Bets.Length) || !capacityChoice.IsInRange(Room.Capacities.Length))
                throw new BadUserInputException();

            var dUser = await _masterRepo.GetUserByIdAsyc(activeUser.Id);

            if (dUser.Money < Room.MinBet)
                throw new BadUserInputException();

            var room = TakeOrCreateAppropriateRoom(betChoice, capacityChoice);
            var roomUser = CreateRoomUser(activeUser, room);
            room.RoomUsers.Add(roomUser);
            room.RoomActors.Add(roomUser);

            RemoveDisconnectedUsers(room);

            if (room.IsFull)
            {
                _serverLoop.CancelPendingRoomTimeout(room);
                await PrepareRoom(room);
            }
            else
            {
                activeUser.Domain = typeof(UserDomain.App.Lobby.Pending);
                _serverLoop.SetupPendingRoomTimeoutIfNotExist(room);
                _sessionRepo.KeepPendingRoom(room);
            }
        }
        private void RemoveDisconnectedUsers(Room room)
        {
            var disconnectedUsers = room.RoomUsers.Where(ru => ru.ActiveUser.Disconnected).ToList();

            disconnectedUsers.ForEach(_ => RemovePendingDisconnectedUser(_));
        }
        /// <summary>
        /// called by timeout
        /// </summary>
        public async Task FillPendingRoomWithBots(Room room)
        {
            room.RoomBots = new();
            var botsCount = room.Capacity - room.RoomUsers.Count;

            var botIds = new List<string> { "999", "9999", "99999" };
            for (int i = 0; i < botsCount; i++)
            {
                var botId = botIds.Cut(StaticRandom.GetRandom(botIds.Count));
                room.RoomBots.Add(new RoomBot { Id = botId, Room = room });
            }

            room.RoomActors.AddRange(room.RoomBots);

            await PrepareRoom(room);
        }
        public async Task MakeRoomUserReadyRpc(ActiveUser activeUser, RoomUser roomUser)
        {
            activeUser.Domain = typeof(UserDomain.App.Lobby.WaitingForOthers);
            roomUser.IsReady = true;

            await StartRoomIfAllReady(roomUser.Room);
        } //doesn't fit into unit testing

        private async Task PrepareRoom(Room room)
        {
            room.SetUsersDomains(typeof(UserDomain.App.Lobby.GettingReady));
            _serverLoop.SetForceStartRoomTimeout(room);

            var userIds = room.RoomActors.Select(ru => ru.Id).ToList();
            var users = await _masterRepo.GetUsersByIdsAsync(userIds);

            users.ForEach(u => u.Money -= room.Bet);

            for (int i = 0; i < room.RoomActors.Count; i++) room.RoomActors[i].TurnId = i;

            var fullUsersInfo = users.Select(Mapper.UserToFullUserInfoFunc).ToList();

            var turnSortedUsersInfo = room.RoomActors.Join(fullUsersInfo, actor => actor.Id, info => info.Id,
                (_, info) => info).ToList();


            await _masterRepo.SaveChangesAsync();

            await SendPrepareRoom(room.BetChoice, room.CapacityChoice, turnSortedUsersInfo);
        }

        private async Task SendPrepareRoom(int betChoice, int capacityChoice, List<FullUserInfo> turnSortedUsersInfo)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < turnSortedUsersInfo.Count; i++)
            {
                var task = _masterHub.Clients.User(turnSortedUsersInfo[i].Id)
                    //todo -farther investigation- I use user rather than client because conn id changes in the same room when he disconnect
                    .SendAsync("PrepareRequestedRoomRpc", betChoice, capacityChoice, turnSortedUsersInfo, i);

                tasks.Add(task);
            }

            await Task.WhenAll(tasks);
        }

        private Room TakeOrCreateAppropriateRoom(int betChoice, int capacityChoice)
        {
            return _sessionRepo.TakePendingRoom(betChoice, capacityChoice) ??
                   _sessionRepo.MakeRoom(betChoice, capacityChoice);
        }

        private RoomUser CreateRoomUser(ActiveUser activeUser, Room room)
        {
            var roomUser = new RoomUser
            {
                ActiveUser = activeUser,
                Id = activeUser.Id,
                ConnectionId = activeUser.ConnectionId,
                Room = room
            };
            _sessionRepo.AddRoomUser(roomUser);
            return roomUser;
        }

        private async Task StartRoomIfAllReady(Room room)
        {
            var readyUsersCount = room.RoomUsers.Count(u => u.IsReady);
            if (readyUsersCount == room.RoomUsers.Count) //bots doesn't have ready prop
            {
                _serverLoop.CancelForceStart(room);
                await _roomManager.StartRoom(room);
            }
        }

        public void RemovePendingDisconnectedUser(RoomUser roomUser)
        {
            _logger.LogInformation($"removing pending room user {roomUser.Id}");

            roomUser.Room.RoomActors.Remove(roomUser);
            roomUser.Room.RoomUsers.Remove(roomUser);

            if (roomUser.Room.RoomUsers.Count == 0) //maybe the remaining are bots, or non
            {
                _serverLoop.CancelPendingRoomTimeout(roomUser.Room);
                _sessionRepo.DeleteRoom(roomUser.Room);
            }

            _sessionRepo.DeleteRoomUser(roomUser);

            roomUser.Room = null;
        }

        //grouping is canceled, I don't send to many players at once
        // private async Task GroupRoomUsers(Room room)
        // {
        //     await _masterHub.Groups.AddToGroupAsync(room.RoomUsers.ConnectionId, "room" + room.Id));
        //     //todo user disconnects temporarily it would be removed from the group?
        //     //not mine but possible to have errors
        // }
    }
}