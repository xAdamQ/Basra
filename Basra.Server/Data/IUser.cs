using System.Threading.Tasks;

namespace Basra.Server.Data
{
    public interface IUser
    {
        string ConnectionId { get; set; }
        bool Disconncted { get; set; }
        Server.Room.IUser RoomUser { get; set; }

        string IdentityUserId { get; set; }
        int Money { get; set; }
        string Name { get; set; }
        int PlayedGames { get; set; }
        int Draws { get; set; }
        int Wins { get; set; }

        Task SendAsync(string method, params object[] args);
    }
}