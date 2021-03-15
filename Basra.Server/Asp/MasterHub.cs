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

        //public ActiveUser GetCurrentUser() => ActiveUsers.First(u => u.ConnectionId == Context.ConnectionId);
        //the system will allow one connections per user

        public MasterHub(IMasterRepo masterRepo, IRoomManager roomManager, ISessionRepo sessionRepo)
        {
            _masterRepo = masterRepo;
            _roomManager = roomManager;
            _sessionRepo = sessionRepo;
        }

        public override async Task OnConnectedAsync()
        {
            Console.WriteLine($"connection established: {Context.UserIdentifier} {Context.UserIdentifier}");

            //var user = new ActiveUser
            //{
            //    Id = Context.UserIdentifier,
            //    ConnectionId = Context.ConnectionId,
            //    //Name = Context.User.
            //    Name = Context.User.Identity.Name,
            //};

            //ActiveUsers.Add(user);
            //the claims principle shoud pass the id here

            // var user = await _masterRepo.GetUserByIdAsyc(Context.UserIdentifier);

            // await _masterRepo.GetNameOfUserAsync(Context.UserIdentifier);
            //
            // user.IsActive = true;
            // _masterRepo.SaveChanges();

            await base.OnConnectedAsync();
            // Context.Abort(); //this how to close connection
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            Console.WriteLine($"{Context.UserIdentifier} Disconnected");

            _sessionRepo.RemoveActiveUser(Context.UserIdentifier);

            //var currentUser = GetCurrentUser();
            //currentUser.Disconncted = true;
            //ActiveUsers.Remove(currentUser);
            //
            // var user = await _masterRepo.GetUserByIdAsyc(Context.UserIdentifier);
            // user.IsActive = false;
            // _masterRepo.SaveChanges();

            // _sessionRepo

            //removed from groups automatically
            await base.OnDisconnectedAsync(exception);
        }

        private RoomUser GetRoomUser() => _sessionRepo.GetRoomUserWithId(Context.UserIdentifier);

        #region rpc

        public async Task AskForRoom(int genre, int bet, int capacity)
        {
            await _roomManager.RequestRoom(genre, bet, capacity, Context.UserIdentifier, Context.ConnectionId);
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

        #region testing

        public void MakeBadUserInputException()
        {
            throw new BadUserInputException();
        }

        public void DummyFunction()
        {
            System.Console.WriteLine("dummy called");
        }

        //public static CancellationTokenSource testSource;
        //public async Task TestAsync()
        //{
        //testSource = new CancellationTokenSource();

        //testSource.CancelAfter(3500);

        //try
        //{
        //    await Task.Delay(3000, testSource.Token);
        //}
        //catch (TaskCanceledException)
        //{
        //    System.Console.WriteLine("Task cancelled");
        //    return;
        //}

        //System.Console.WriteLine("success 7985255555555");
        //}

        #endregion

        #endregion
    }
}