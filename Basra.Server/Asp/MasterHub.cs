using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Basra.Server.Extensions;
using System.Linq;
using Basra.Server.Exceptions;
using System.Threading;
using Basra.Server.Services;

namespace Basra.Server
{
    public interface IRoomHub
    {
        Task StartRoom(int genre, int bet, int playerCount);

        Task BuyCardback(int id);
        Task BuyBackground(int id);
    }

    [Authorize]
    public class MasterHub : Hub
    {
        private readonly IMasterRepo _masterRepo;
        private readonly ISessionRepo _sessionRepo;
        private readonly IRoomManager _roomManager;

        public MasterHub(IMasterRepo masterRepo, IRoomManager roomManager, ISessionRepo sessionRepo)
        {
            _masterRepo = masterRepo;
            _roomManager = roomManager;
            _sessionRepo = sessionRepo;
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"connection established: {Context.UserIdentifier} {Context.UserIdentifier}");

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"{Context.UserIdentifier} Disconnected");

            _sessionRepo.RemoveActiveUser(Context.UserIdentifier);

            await base.OnDisconnectedAsync(exception);
        }

        private RoomUser GetRoomUser() => _sessionRepo.GetRoomUserWithId(Context.UserIdentifier);


        public async Task RequestRoom(int genre, int betChoice, int capacityChoice)
        {
            await _roomManager.RequestRoom(genre, betChoice, capacityChoice, Context.UserIdentifier, Context.ConnectionId);
        }

        public async Task Ready()
        {
            await _roomManager.Ready(GetRoomUser());
        }

        public async Task Throw(int indexInHand)
        {
            await _roomManager.Play(GetRoomUser(), indexInHand);
        } //automatic actions happen from server side and the client knows this overrides his action and do the revert 

        public async Task InformTurnTimeout()
        {
            await _roomManager.RandomPlay(GetRoomUser());
        }

    }
}