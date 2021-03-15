using System.Threading.Tasks;

namespace Basra.Server.Services
{
    public interface IServerLoop
    {
        Task SetupTurnTimout(RoomUser roomUser);
        void CutTurnTimout(RoomUser roomUser);
    }
}