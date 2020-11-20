using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Basra.Server.Structure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

//learn about thread safety

namespace Basra.Server
{
    [Authorize]
    public class MasterHub : Hub
    {
        //hub life time is not even per connection, it's per request!

        private readonly SignInManager<BasraIdentityUser> _signInManager;
        private readonly MasterContext _masterContext;

        private static List<BasraIdentityUser> ConnectedUsers { get; } = new List<BasraIdentityUser>();//filled using the conyext, isn't it loaded by default?
        private static List<Room> WaitingRooms { get; } = new List<Room>();

        public MasterHub(SignInManager<BasraIdentityUser> signInManager, MasterContext masterContext)
        {
            _signInManager = signInManager;
            _masterContext = masterContext;
        }

        public override async Task OnConnectedAsync()
        {
            System.Console.WriteLine($"connection established: {Context.ConnectionId} {Context.UserIdentifier}");

            ConnectedUsers.Add(_masterContext.Users.FirstOrDefault(u => u.Id == Context.UserIdentifier));
            //the claims principle shoud pass the id here

            await base.OnConnectedAsync();

            // Context.Abort(); //this hiw to close connection
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            System.Console.WriteLine($"{Context.ConnectionId} Disconnected");
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

        public async Task AskForRoom(int roomGenre, int roomPlayerCount)
        {
            Room? nullableRoom = null;
            try //find a room with this specs
            {
                nullableRoom = WaitingRooms.First(r => r.Genre == roomGenre && r.PlayerCount == roomPlayerCount);
            } //otherwise make new one
            catch (InvalidOperationException) //this wins
            {
                nullableRoom = MakeRoom(roomPlayerCount, roomGenre);
                Debug.WriteLine("the exc for empty room is (InvalidOperationException)");
            }

            var room = nullableRoom.Value;

            room.Players.Add(Context.UserIdentifier);
            await Groups.AddToGroupAsync(Context.ConnectionId, "room" + room.Id);
            System.Console.WriteLine($"player {Context.UserIdentifier} has entered room");

            if (room.PlayerCount == room.Players.Count)
            {
                WaitingRooms.Remove(room);
                // await Clients.Caller.SendAsync("EnterRoom");
                await Clients.Group("room" + room.Id).SendAsync("EnterRoom", roomGenre, roomPlayerCount);
            }
            else
            {
                await Clients.Caller.SendAsync("RoomIsFilling");
            }

        }

        private Room MakeRoom(int playerCount, int genre)
        {
            var room = new Room
            {
                Id = Room.LastId++,
                Genre = genre,
                Players = new List<string>(),
                PlayerCount = playerCount,
            };

            WaitingRooms.Add(room);
            System.Console.WriteLine($"a new room is made");

            return room;
        }

    }
}