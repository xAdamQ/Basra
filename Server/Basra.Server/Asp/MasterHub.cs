using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Reflection;
using Basra.Server.Exceptions;
using Basra.Models.Client;
using Basra.Server.Helpers;
using Basra.Server.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Basra.Server
{
    [Authorize]
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
            ISessionRepo sessionRepo, IRoomManager roomManager, IMatchMaker matchMaker, ILogger<MasterHub> logger)
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
                if (ActiveUser.Disconnected == false)
                    throw new BadUserInputException("user is connected already and trying to connect again");
                //todo i think the auth handler should be responsible for this part, should be terminated faster

                activeRoomState = await _roomManager.GetFullRoomState(RoomUser);

                ActiveUser.Disconnected = false;
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
            _sessionRepo.AddActiveUser(new ActiveUser(Context.UserIdentifier, Context.ConnectionId, typeof(UserDomain.App.Lobby.Idle)));
        }
        private async Task InitClientGame(ActiveRoomState activeRoomState)
        {
            var user = await _masterRepo.GetUserByIdAsyc(Context.UserIdentifier);
            var clientPersonalInfo = Mapper.ConvertUserDataToClient(user);

            var yesterdayChampions = new MinUserInfo[]
            {
                new MinUserInfo {Id = "tstId1", Name = "champ1", Level = 1, SelectedTitleId = 0},
                new MinUserInfo {Id = "tstId2", Name = "champ2", Level = 2, SelectedTitleId = 1},
                new MinUserInfo {Id = "tstId3", Name = "champ3", Level = 3, SelectedTitleId = 2},
            };
            var topFriends = new MinUserInfo[]
            {
                new MinUserInfo {Id = "tstId5", Name = "friend2", Level = 2, SelectedTitleId = 3},
                new MinUserInfo {Id = "tstId6", Name = "friend3", Level = 3, SelectedTitleId = 4},
            };

            await Clients.Caller.SendAsync("InitGame", clientPersonalInfo, yesterdayChampions, topFriends, activeRoomState);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            _logger.LogInformation($"{Context.UserIdentifier} Disconnected");

            if (RoomUser != null)
                _matchMaker.RemovePendingDisconnectedUser(RoomUser);

            //RoomUser.Room is null when he was the last player in pending room and disconnected

            if (RoomUser != null && RoomUser.Room != null) //todo test get non existing user
                ActiveUser.Disconnected = true;
            else
                _sessionRepo.RemoveActiveUser(Context.UserIdentifier);

            await base.OnDisconnectedAsync(exception);
        }

        #endregion

        private RoomUser roomUser;
        private RoomUser RoomUser => roomUser ??= _sessionRepo.GetRoomUserWithId(Context.UserIdentifier);
        private ActiveUser activeUser;
        private ActiveUser ActiveUser => activeUser ??= _sessionRepo.GetActiveUser(Context.UserIdentifier);

        #region general

        [RpcDomain(typeof(UserDomain.App))]
        public async Task<PersonalFullUserInfo> GetPersonalUserData()
        {
            return Mapper.ConvertUserDataToClient(await _masterRepo.GetUserByIdAsyc(Context.UserIdentifier));
        }

        /// <summary>
        /// get public user data by his id
        /// </summary>
        [RpcDomain(typeof(UserDomain.App))]
        public async Task<FullUserInfo> GetUserData(string id)
        {
            return await _masterRepo.GetFullUserInfoAsync(id);
        }

        #endregion

        #region lobby

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task RequestRandomRoom(int betChoice, int capacityChoice)
        {
            await _matchMaker.RequestRandomRoom(betChoice, capacityChoice, ActiveUser);
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
            await _lobbyManager.BuyCardBack(cardbackId);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task BuyBackground(int backgroundId)
        {
            await _lobbyManager.BuyCardBack(backgroundId);
        }

        [RpcDomain(typeof(UserDomain.App.Lobby.Idle))]
        public async Task SelectCardback(int cardbackId)
        {
            await _lobbyManager.SelectCardback(cardbackId);
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
            return new MinUserInfo {Name = "some data to test"};
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
                var rpcs = typeof(MasterHub).GetMethods(BindingFlags.Public | BindingFlags.Instance);

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