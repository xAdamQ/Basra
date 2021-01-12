using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Room
{
    public class Pending : IPending
    {
        public int Genre { get; }
        public List<IUser> Users { get; } = new List<IUser>();
        public int PlayerCount { get; }
        public int Id { get; }
        public static int LastId { get; set; }

        private static List<Pending> All { get; } = new List<Pending>();

        public Pending(int genre, int playerCount)
        {
            Id = LastId++;
            Genre = genre;
            PlayerCount = playerCount;

            All.Add(this);

            System.Console.WriteLine($"a new room is made with genre {Genre} and playerCount {PlayerCount}");
        }

        public async static Task AskForRoom(MasterHub hub, int genre, int playerCount)
        {
            Pending pRoom = null;
            try //find a room with this specs
            {
                pRoom = All.First(r => r.Genre == genre && r.PlayerCount == playerCount);
            } //otherwise make new one
            catch (InvalidOperationException)
            {
                pRoom = new Pending(genre, playerCount);
                Console.WriteLine("the exc for empty room is (InvalidOperationException)");
            }

            var initiator = hub.GetCurrentUser();

            initiator.RoomUser = new User
            {
                Structure = initiator,
            };

            pRoom.Users.Add(initiator.RoomUser);

            pRoom.Users.RemoveAll(u => u.Structure.Disconncted);

            if (pRoom.PlayerCount == pRoom.Users.Count)
            {
                All.Remove(pRoom);

                var active = new Active(pRoom);
                await active.Start();
            }
            else
            {
                await hub.Clients.Caller.SendAsync("RoomIsFilling");
            }
        }
    }
}