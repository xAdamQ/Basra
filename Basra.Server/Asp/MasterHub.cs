using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Basra.Server.Structure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Basra.Server.Extensions;
using System.Linq;

//todo learn about thread safety
namespace Basra.Server
{
    [Authorize]
    public class MasterHub : Hub
    {
        // public static MasterHub Current;//I don't know if this is thread safe
        //hub life time is not even per connection, it's per request!

        private readonly SignInManager<BasraIdentityUser> _signInManager;
        private readonly MasterContext _masterContext;

        private static List<BasraIdentityUser> ConnectedUsersIdentities { get; } = new List<BasraIdentityUser>();//filled using the conyext, isn't it loaded by default?
        private static List<User> ConnectedUsers { get; } = new List<User>();

        // public RuntimeUser GetUser(string connectionId) => RuntimeUsers.First(u => u.ConnectionId == connectionId);
        public User GetCurrentUser() => ConnectedUsers.First(u => u.ConnectionId == Context.ConnectionId);
        //the system will allow one connections per user

        public MasterHub(SignInManager<BasraIdentityUser> signInManager, MasterContext masterContext)
        {
            _signInManager = signInManager;
            _masterContext = masterContext;
        }

        public override async Task OnConnectedAsync()
        {
            System.Console.WriteLine($"connection established: {Context.ConnectionId} {Context.UserIdentifier}");

            // Context.GetHttpContext().RequestServices()

            ConnectedUsersIdentities.Add(_masterContext.Users.First(u => u.Id == Context.UserIdentifier));
            var user = new User
            {
                Id = Context.UserIdentifier,
                ConnectionId = Context.ConnectionId,
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

            ConnectedUsersIdentities.Remove(u => u.Id == Context.UserIdentifier);

            await base.OnDisconnectedAsync(exception);
        }

        //sends the message to other contacts
        public async Task SendMessageAsync(string message)
        {
            var messageObject = JsonConvert.DeserializeObject<Message>(message);
            // JsonConvert.DeserializeObject<dynamic>(message); //you clould use it like this without strict types

            System.Console.WriteLine("message from: " + Context.ConnectionId);

            if (string.IsNullOrEmpty(messageObject.To))
            {
                await Clients.All.SendAsync("ShowMessage", messageObject.Value);
                //what's the method
            }
            else
            {
                await Clients.Client(messageObject.To).SendAsync("ShowMessage", messageObject.Value);
            }
        }

        #region rpc
        public async Task AskForRoom(int roomGenre, int roomPlayerCount)
        {
            await PendingRoom.AskForRoom(this, roomGenre, roomPlayerCount);

        }
        public void Ready()
        {
            GetCurrentUser().Ready();
        }
        #endregion

    }
}