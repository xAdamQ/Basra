using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Basra.Server.Structure
{
    public class User
    {
        public string Id;
        public string ConnectionId { get; set; }
        public bool Disconncted { get; set; }

        public string Name;

        public Room.User RUser;

        public async Task SendAsync(string method, params object[] args)
        {
            await Program.HubContext.Clients.Client(ConnectionId).SendAsync(method, args);
        }

    }
}