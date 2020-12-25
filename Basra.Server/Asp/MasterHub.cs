using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Basra.Server.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Basra.Server.Extensions;
using System.Linq;
using Basra.Server.Structure;
using Basra.Server.Exceptions;
using System.Threading;

//todo learn about thread safety
namespace Basra.Server
{
    [Authorize]
    public class MasterHub : Hub
    {
        // public static MasterHub Current;//I don't know if this is thread safe
        //hub life time is not even per connection, it's per request!

        private readonly SignInManager<BasraIdentityUser> _signInManager;

        private static List<User> ConnectedUsers { get; } = new List<User>();

        // public RuntimeUser GetUser(string connectionId) => RuntimeUsers.First(u => u.ConnectionId == connectionId);
        public User GetCurrentUser() => ConnectedUsers.First(u => u.ConnectionId == Context.ConnectionId);
        //the system will allow one connections per user

        public MasterHub(SignInManager<BasraIdentityUser> signInManager, MasterContext masterContext)
        {
            _signInManager = signInManager;
        }

        public override async Task OnConnectedAsync()
        {
            System.Console.WriteLine($"connection established: {Context.ConnectionId} {Context.UserIdentifier}");

            var user = new User
            {
                Id = Context.UserIdentifier,
                ConnectionId = Context.ConnectionId,
                Name = Context.User.Identity.Name,
            };

            ConnectedUsers.Add(user);
            //the claims principle shoud pass the id here

            await base.OnConnectedAsync();

            // Context.Abort(); //this hiw to close connection
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            System.Console.WriteLine($"{Context.ConnectionId} Disconnected");

            var currentUser = GetCurrentUser();
            currentUser.Disconncted = true;
            ConnectedUsers.Remove(currentUser);
            //removed from groups automatically
            await base.OnDisconnectedAsync(exception);
        }

        //sends the message to other contacts
        //public async Task SendMessageAsync(string message)
        //{
        //    var messageObject = JsonConvert.DeserializeObject<Message>(message);
        //    // JsonConvert.DeserializeObject<dynamic>(message); //you clould use it like this without strict types

        //    System.Console.WriteLine("message from: " + Context.ConnectionId);

        //    if (string.IsNullOrEmpty(messageObject.To))
        //    {
        //        await Clients.All.SendAsync("ShowMessage", messageObject.Value);
        //        //what's the method
        //    }
        //    else
        //    {
        //        await Clients.Client(messageObject.To).SendAsync("ShowMessage", messageObject.Value);
        //    }
        //}

        #region rpc

        public async Task AskForRoom(int roomGenre, int roomPlayerCount)
        {
            await Structure.Room.Pending.AskForRoom(this, roomGenre, roomPlayerCount);
        }

        public async Task Ready()
        {
            await GetCurrentUser().RUser.Ready();
        }

        public async Task Throw(int indexInHand)
        {
            return;
            await GetCurrentUser().RUser.Play(indexInHand);
        }//automatic actions happen from serevr side and the client knows this overrides his action and do the revert 
        public async Task InformTurnTimeout()
        {
            await GetCurrentUser().RUser.RandomPlay();
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