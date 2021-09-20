using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Basra.Server.Exceptions;
using Basra.Common;
using Basra.Server.Helpers;
using Basra.Server.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Basra.Server
{
    public class MasterHub : Hub
    {
        #region services

        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IRoomManager _roomManager;
        private readonly IMatchMaker _matchMaker;
        private readonly ILogger<MasterHub> _logger;
        private readonly ILobbyManager _lobbyManager;


        public MasterHub(IMasterRepo masterRepo, ILobbyManager lobbyManager,
            ISessionRepo sessionRepo, IRoomManager roomManager, IMatchMaker matchMaker,
            ILogger<MasterHub> logger)
        {
            _masterRepo = masterRepo;
            _lobbyManager = lobbyManager;
            _sessionRepo = sessionRepo;
            _roomManager = roomManager;
            _matchMaker = matchMaker;
            _logger = logger;
        }

        #endregion

        #region on dis/connceted

        public override async Task OnConnectedAsync()
        {
            _logger.LogInformation($"connection established: {Context.UserIdentifier}");

            ActiveRoomState activeRoomState = null;

            if (_sessionRepo.IsUserActive(Context.UserIdentifier))
            {
                if (ActiveUser.IsDisconnected == false)
                    throw new BadUserInputException(
                        "user is connected already and trying to connect again");
                //todo i think the auth handler should be responsible for this part, should be terminated faster

                activeRoomState = await _roomManager.GetFullRoomState(RoomUser);

                ActiveUser.IsDisconnected = false;
            }
            else
            {
                CreateActiveUser();
            }

            await InitClientGame(activeRoomState);

            await base.OnConnectedAsync();
        }
        private void CreateActiveUser()
        {
            _sessionRepo.AddActiveUser(new ActiveUser(Context.UserIdentifier, Context.ConnectionId,
                typeof(UserDomain.App.Lobby.Idle)));
        }
        private async Task InitClientGame(ActiveRoomState activeRoomState)
        {
            var user = await _masterRepo.GetUserByIdAsyc(Context.UserIdentifier);
            var clientPersonalInfo = Mapper.ConvertUserDataToClient(user);
            //you travel to db 2 more times
            clientPersonalInfo.Followers =
                await _masterRepo.GetFollowersAsync(Context.UserIdentifier);
            clientPersonalInfo.Followings =
                await _masterRepo.GetFollowingsAsync(Context.UserIdentifier);

            await Clients.Caller.SendAsync("InitGame", ++ActiveUser.MessageIndex, clientPersonalInfo, activeRoomState, ActiveUser.MessageIndex);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"{Context.UserIdentifier} Disconnected");

            ActiveUser.Disconnect();

            //remove pending room user
            if (RoomUser != null && !RoomUser.Room.IsFull)
                _matchMaker.RemovePendingDisconnectedUser(RoomUser);

            //RoomUser.Room is null when he was the last player in pending room and disconnected

            //mark user in room as disconnected
            if (RoomUser is { Room: { } }) //todo test get non existing user
                //this means the room user is not null it's room is not also
                ActiveUser.IsDisconnected = true;
            else
                _sessionRepo.RemoveActiveUser(Context.UserIdentifier);

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        private RoomUser roomUser;
        private RoomUser RoomUser =>
            roomUser ??= _sessionRepo.GetRoomUserWithId(Context.UserIdentifier);
        private ActiveUser activeUser;
        private ActiveUser ActiveUser =>
            activeUser ??= _sessionRepo.GetActiveUser(Context.UserIdentifier);

        #region general

        [RpcDomain(typeof(UserDomain.App))]
        public async Task<PersonalFullUserInfo> GetPersonalUserData()
        {
            return Mapper.ConvertUserDataToClient(
                await _masterRepo.GetUserByIdAsyc(Context.UserIdentifier));
        }

        /// <summary>
        /// get public user data by his id
        /// </summary>
        [RpcDomain(typeof(UserDomain.App))]
        public async Task<FullUserInfo> GetUserData(string id)
        {
            var data = await _masterRepo.GetFullUserInfoAsync(id);
            data.Friendship = (int)_masterRepo.GetFriendship(Context.UserIdentifier, id);
            return data;
        }

        [RpcDomain(typeof(UserDomain.App))]
        public async Task ToggleFollow(string targetId)
        {
            _masterRepo.ToggleFollow(Context.UserIdentifier, targetId);
            await _masterRepo.SaveChangesAsync();
        }

        [RpcDomain(typeof(UserDomain.App))]
        public async Task ToggleOpenMatches()
        {
            var user = await _masterRepo.GetUserByIdAsyc(Context.UserIdentifier);
            user.EnableOpenMatches = !user.EnableOpenMatches;
            await _masterRepo.SaveChangesAsync();
        }

        #endregion

        #region lobby

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task MakePurchase(string purchaseData, string sign)
        {
            await _lobbyManager.MakePurchase(ActiveUser, purchaseData, sign);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task RequestRandomRoom(int betChoice, int capacityChoice)
        {
            await _matchMaker.RequestRandomRoom(betChoice, capacityChoice, ActiveUser);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task<MatchMaker.MatchRequestResult> RequestMatch(string oppoId)
        {
            return await _matchMaker.RequestMatch(ActiveUser, oppoId);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Pending))]
        public void CancelChallengeRequest(string oppoId)
        {
            _matchMaker.CancelChallengeRequest(ActiveUser);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task<MatchMaker.ChallengeResponseResult> RespondChallengeRequest
            (string senderId, bool response)
        {
            return await _matchMaker.RespondChallengeRequest(ActiveUser, response, senderId);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.GettingReady))]
        public async Task Ready()
        {
            await _matchMaker.MakeRoomUserReadyRpc(ActiveUser, RoomUser);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task AskForMoneyAid()
        {
            await _lobbyManager.RequestMoneyAid(ActiveUser);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task ClaimMoneyAid()
        {
            await _lobbyManager.ClaimMoneyAim(ActiveUser);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task BuyCardback(int cardbackId)
        {
            await _lobbyManager.BuyCardBack(cardbackId, ActiveUser.Id);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task BuyBackground(int backgroundId)
        {
            await _lobbyManager.BuyBackground(backgroundId, ActiveUser.Id);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task SelectCardback(int cardbackId)
        {
            await _lobbyManager.SelectCardback(cardbackId, ActiveUser.Id);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task SelectBackground(int backgroundId)
        {
            await _lobbyManager.SelectBackground(backgroundId, ActiveUser.Id);
        }

        #endregion

        #region room

        [RpcDomain(typeof(UserDomain.App.Room.Active))]
        public async Task Throw(int indexInHand)
        {
            await _roomManager.UserPlayRpc(RoomUser, indexInHand);
        }

        //custom validation?
        [RpcDomain(typeof(UserDomain.App.Room.Active))]
        public async Task MissTurn()
        {
            await _roomManager.MissTurnRpc(RoomUser);
        }

        /// <summary>
        /// get what makes up the room for reconnected users
        /// except for the turn remaining time
        /// </summary>
        [RpcDomain(typeof(UserDomain.App.Room.Active))]
        public async Task<ActiveRoomState> GetFullRoomState()
        {
            return await _roomManager.GetFullRoomState(RoomUser);
        }

        //todo currently there's no validation on the message id, so it can be any string
        //how this could be misused? 1- sending very big message to the receiving client to stop his game!
        [RpcDomain(typeof(UserDomain.App.Room.Active))]
        public async Task ShowMessage(string msgId)
        {
            await _roomManager.ShowMessage(RoomUser, msgId);
        }


        // [RpcDomain(typeof(UserDomain.App.Room.Active))]
        // public async Task Surrender()
        // {
        //     await _roomManager.Surrender(RoomUser);
        // }

        #endregion


        [RpcDomain(typeof(UserDomain.App))]
        public void BuieTest()
        {
            throw new BadUserInputException("this the exc message");
        }

        [RpcDomain(typeof(UserDomain.App))]
        public void ThrowExc()
        {
            throw new Exception("a test general exc is thrown");
        }

        [RpcDomain(typeof(UserDomain.App))]
        public async Task<MinUserInfo> TestReturnObject()
        {
            await Task.Delay(5000);
            return new MinUserInfo { Name = "some data to test" };
        }

        [RpcDomain(typeof(UserDomain.App))]
        public async Task TestWaitAlot()
        {
            await Task.Delay(5000);
        }

        public class MethodDomains
        {
            public MethodDomains()
            {
                var rpcs =
                    typeof(MasterHub).GetMethods(BindingFlags.Public | BindingFlags.Instance);

                foreach (var rpc in rpcs)
                {
                    var attribute = rpc.GetCustomAttribute<RpcDomainAttribute>();

                    if (attribute == null) continue;

                    Domains.Add(rpc.Name, attribute.Domain);
                }
            }

            private Dictionary<string, Type> Domains { get; } = new();
            public Type GetDomain(string method)
            {
                return !Domains.ContainsKey(method) ? null : Domains[method];
                // throw new Exception("the request function is not listed in the hub public methods");
            }
        }

        [AttributeUsage(AttributeTargets.Method, Inherited = false)]
        private sealed class RpcDomainAttribute : Attribute
        {
            public Type Domain { get; }
            public RpcDomainAttribute(Type domain)
            {
                Domain = domain;
            }
        }
    }
}