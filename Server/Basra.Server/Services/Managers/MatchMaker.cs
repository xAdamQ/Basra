using Basra.Common;
using Basra.Server.Exceptions;
using Basra.Server.Extensions;
using Basra.Server.Helpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

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
        Task<MatchMaker.MatchRequestResult> RequestMatch(ActiveUser activeUser, string oppoId);
        void CancelChallengeRequest(ActiveUser activeUser);
        Task<MatchMaker.ChallengeResponseResult> RespondChallengeRequest(ActiveUser activeUser,
            bool response, string sender);
    }

    public class MatchMaker : IMatchMaker
    {
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IRoomManager _roomManager;
        private readonly IServerLoop _serverLoop;
        private readonly ILogger<MatchMaker> _logger;
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;

        public MatchMaker(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo,
            ISessionRepo sessionRepo,
            IRoomManager roomManager, IServerLoop serverLoop, ILogger<MatchMaker> logger)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _sessionRepo = sessionRepo;
            _roomManager = roomManager;
            _serverLoop = serverLoop;
            _logger = logger;
        }

        public async Task RequestRandomRoom(int betChoice, int capacityChoice,
            ActiveUser activeUser)
        {
            if (!betChoice.IsInRange(Room.Bets.Length) ||
                !capacityChoice.IsInRange(Room.Capacities.Length))
                throw new BadUserInputException();

            var dUser = await _masterRepo.GetUserByIdAsyc(activeUser.Id);

            if (dUser.Money < Room.Bets[betChoice])
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
            var disconnectedUsers =
                room.RoomUsers.Where(ru => ru.ActiveUser.IsDisconnected).ToList();

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

        public enum MatchRequestResult
        {
            Offline,
            Playing,
            NoMoney,
            Available,
        }

        public async Task<MatchRequestResult> RequestMatch(ActiveUser activeUser,
            string oppoId)
        {
            var dUser = await _masterRepo.GetUserByIdAsyc(activeUser.Id);

            if (dUser.Money < Room.MinBet)
                throw new BadUserInputException();

            //BadUserInputException is thrown when something is wrong but should've been
            //validated by the client 

            var oppoUser = await _masterRepo.GetUserByIdAsyc(oppoId);
            var friendship = _masterRepo.GetFriendship(activeUser.Id, oppoId);

            if (friendship is FriendShip.None or FriendShip.Follower && !oppoUser.EnableOpenMatches)
                throw new BadUserInputException();

            if (!_sessionRepo.IsUserActive(oppoId))
                return MatchRequestResult.Offline;

            if (_sessionRepo.DoesRoomUserExist(oppoId))
                return MatchRequestResult.Playing;

            if (oppoUser.Money < Room.MinBet)
                return MatchRequestResult.NoMoney;

            //can't call again because this fun domain is lobby.idle only
            activeUser.Domain = typeof(UserDomain.App.Lobby.Pending);

            activeUser.ChallengeRequestTarget = oppoId;

            var oppoAU = _sessionRepo.GetActiveUser(oppoId);
            //oppo is 100% active at this satage

            await _masterHub.SendOrderedAsync(oppoAU, "ChallengeRequest",
                Mapper.UserToMinUserInfoFunc(dUser));

            return MatchRequestResult.Available;
        }

        public void CancelChallengeRequest(ActiveUser activeUser)
        {
            activeUser.ChallengeRequestTarget = null;
            activeUser.Domain = typeof(UserDomain.App.Lobby.Idle);
        }

        public enum ChallengeResponseResult
        {
            Offline, //player is offline whatever the response
            Canceled, //player is not interested anymore
            Success, //successful whatever the response
        }

        public async Task<ChallengeResponseResult> RespondChallengeRequest(ActiveUser activeUser,
            bool response, string sender)
        {
            if (!_sessionRepo.IsUserActive(sender))
                return ChallengeResponseResult.Offline;

            var senderActiveUser = _sessionRepo.GetActiveUser(sender);

            if (senderActiveUser.ChallengeRequestTarget != activeUser.Id)
                //can be null or he sent to another user after
                return ChallengeResponseResult.Canceled;

            if (!response)
            {
                await _masterHub.SendOrderedAsync(_sessionRepo.GetActiveUser(sender),
                    "RespondChallenge", false);
                //otherwise start the room

                CancelChallengeRequest(senderActiveUser);

                return ChallengeResponseResult.Success;
            }

            //user domains are changed when prepare is called
            senderActiveUser.ChallengeRequestTarget = null;

            var room = _sessionRepo.MakeRoom(0, 0);

            var roomUser = CreateRoomUser(activeUser, room);
            var senderRoomUser = CreateRoomUser(senderActiveUser, room);

            room.RoomUsers.Add(senderRoomUser);
            room.RoomActors.Add(senderRoomUser);
            room.RoomUsers.Add(roomUser);
            room.RoomActors.Add(roomUser);

            await PrepareRoom(room);

            return ChallengeResponseResult.Success;
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

            var fullUsersInfos = users.Select(Mapper.UserToFullUserInfoFunc).ToList();

            var turnSortedUsersInfo = room.RoomActors.Join(fullUsersInfos, actor => actor.Id,
                info => info.Id, (_, info) => info).ToList();

            await _masterRepo.SaveChangesAsync();

            await SendPrepareRoom(room, turnSortedUsersInfo);
        }

        private async Task SendPrepareRoom(Room room, List<FullUserInfo> turnSortedUsersInfo)
        {
            var tasks = new List<Task>();

            for (int i = 0; i < turnSortedUsersInfo.Count; i++)
            {
                var userInfo = turnSortedUsersInfo[i];
                foreach (var otherUser in turnSortedUsersInfo.Where(u => u != userInfo))
                    otherUser.Friendship =
                        (int)_masterRepo.GetFriendship(userInfo.Id, otherUser.Id);


                if (room.RoomActors[i] is RoomUser ru)
                {
                    var task = _masterHub.SendOrderedAsync(ru.ActiveUser, "PrepareRequestedRoomRpc",
                        room.BetChoice, room.CapacityChoice, turnSortedUsersInfo, i);
                    //changes in the same room when he disconnect

                    tasks.Add(task);
                }
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
    }
}