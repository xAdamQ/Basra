using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

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
    public class RoomUserManager
    {
        private readonly IHubContext<MasterHub> _masterHub;
        private readonly IMasterRepo _masterRepo;
        private readonly IRoomManager _roomManager;

        public RoomUserManager(IHubContext<MasterHub> masterHub, IMasterRepo masterRepo, IRoomManager roomManager)
        {
            _masterHub = masterHub;
            _masterRepo = masterRepo;
            _roomManager = roomManager;
        }

        // public async Task StartRoom(RoomUser roomUser, Room room, int id, string[] playerNames)
        // {
        //     roomUser.IdInRoom = id;
        //     roomUser.ActiveRoom = room;
        //     //////////////big issue, saving in db

        //     // IdInRoom = id;
        //     // ActiveRoom = room;
        //
        //     await _masterHub.Clients.User(roomUser.UserId).SendAsync("StartRoom", roomUser.IdInRoom, playerNames);
        // }
        //
        // //rpc
        // /// <summary>
        // /// get ready for the room to start distribute cards
        // /// </summary>
        // public async Task Ready(RoomUser roomUser)
        // {
        //     roomUser.IsReady = true;
        //     //////////////big issue, saving in db

        //     await roomUser.ActiveRoom.CheckPlayersReady();
        // }

    }
}