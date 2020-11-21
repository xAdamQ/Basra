using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Structure
{
    public class PendingRoom
    {
        public int Genre { get; }
        public List<User> Users { get; } = new List<User>(); //maybe removed if the groups is utilized
        public int PlayerCount { get; }
        public int Id { get; }
        public static int LastId { get; set; }

        private static List<PendingRoom> All { get; } = new List<PendingRoom>();

        public PendingRoom(int genre, int playerCount)
        {
            Id = LastId++;
            Genre = genre;
            PlayerCount = playerCount;

            All.Add(this);

            System.Console.WriteLine($"a new room is made with genre {Genre} and playerCount {PlayerCount}");
        }

        public async static Task AskForRoom(MasterHub hub, int genre, int playerCount)
        {
            PendingRoom pRoom = null;
            try //find a room with this specs
            {
                pRoom = All.First(r => r.Genre == genre && r.PlayerCount == playerCount);
            } //otherwise make new one
            catch (InvalidOperationException)
            {
                pRoom = new PendingRoom(genre, playerCount);

                Console.WriteLine("the exc for empty room is (InvalidOperationException)");
            }

            var initiator = hub.GetCurrentUser();

            pRoom.Users.Add(initiator);
            await hub.Groups.AddToGroupAsync(initiator.ConnectionId, "room" + pRoom.Id);
            System.Console.WriteLine($"player {initiator.Id} has entered room");

            pRoom.Users.RemoveAll(u => u.Disconncted);

            if (pRoom.PlayerCount == pRoom.Users.Count)
            {
                All.Remove(pRoom);

                var hubContext = hub.Context.GetHttpContext().RequestServices.GetService(typeof(IHubContext<MasterHub>)) as IHubContext<MasterHub>;
                var room = new Room(hubContext, pRoom);

                await hub.Clients.Group("room" + pRoom.Id).SendAsync("EnterRoom", genre, playerCount);
            }
            else
            {
                await hub.Clients.Caller.SendAsync("RoomIsFilling");
            }
        }
    }
}