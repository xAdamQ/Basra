using System.Threading.Tasks;

namespace Basra.Server.Services
{
    public interface IRoomUserManager
    {
        Task Distribute();
        Task InitialDistribute();
        bool IsMyTurn();
        Task Play(int cardIndexInHand);
        Task RandomPlay();
        Task Ready();
        Task StartRoom(Room room, int id, string[] playerNames);
        void StartTurn();
    }
}