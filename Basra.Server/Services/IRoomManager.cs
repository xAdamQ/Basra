using System.Threading.Tasks;

namespace Basra.Server.Services
{
    public interface IRoomManager
    {
        Task RequestRoom(int genre, int bet, int capacity, string userId, string connId);
        Task FinalizeGame(Room room);

        /// <summary>
        /// get ready for the room to start distribute cards
        /// </summary>
        Task Ready(RoomUser roomUser);

        Task RandomPlay(RoomUser roomUser);
        Task Play(RoomUser roomUser, int cardIndexInHand);
    }
}