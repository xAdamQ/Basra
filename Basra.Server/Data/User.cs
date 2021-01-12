using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Data
{
    public class User : IUser
    {
        [System.ComponentModel.DataAnnotations.Schema.ForeignKey("Id")]
        public string ConnectionId { get; set; }
        public bool Disconncted { get; set; }
        public Room.IUser RoomUser { get; set; }

        //how to save -- on singup
        //how to load -- on signin
        //how to access -- 
        //how to update --

        public Identity.User IdentityUser;

        public string IdentityUserId { get; set; }

        public string Name { get; set; }

        public int Money { get; set; }

        public int PlayedGames { get; set; }
        public int Wins { get; set; }
        public int Draws { get; set; }

        public async Task SendAsync(string method, params object[] args)
        {
            await Program.HubContext.Clients.Client(ConnectionId).SendAsync(method, args);
        }

    }
}