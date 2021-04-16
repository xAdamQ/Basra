using BestHTTP.SecureProtocol.Org.BouncyCastle.Ocsp;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;

namespace Basra.Client
{
    public class Lobby : IInitializable
    {
        private readonly ZenjectSceneLoader _sceneLoader;
        private readonly IAppManager _appManager;
        private readonly NetManager _netManager;
        private readonly ILobbyInterface _lobbyInterface;

        [Inject]
        public Lobby(ZenjectSceneLoader sceneLoader, IAppManager appManager, NetManager netManager,
            ILobbyInterface lobbyInterface)
        {
            _sceneLoader = sceneLoader;
            _appManager = appManager;
            _netManager = netManager;
            _lobbyInterface = lobbyInterface;
        }

        public void Initialize()
        {
            InitVisuals();
        }

        void InitVisuals()
        {
            _lobbyInterface.UserName = _appManager.User.FbId;
        }

        [Rpc]
        public async UniTask StartRoom(int myTurnId, string[] userNames)
        {
            await _sceneLoader.LoadSceneAsync("Room", LoadSceneMode.Additive,
                extraBindings: container =>
                {
                    container.BindInstance(new Room.Room.Settings
                    {
                        BetChoice = RequestedRoom.Item1,
                        CapacityChoice = RequestedRoom.Item2,
                        MyTurnId = myTurnId,
                        UserNames = userNames
                    }).WhenInjectedInto<RoomInstaller>();
                });

            _appManager.Interface.LoadingFeedback.Hide();
        }

        [Rpc]
        public void RoomIsFilling()
        {
            _appManager.Interface.LoadingFeedback.Show("Filling The Room");
        }

        private (int, int) RequestedRoom;

        //button
        public void RequestRoom(int betChoice, int capacityChoice)
        {
            _appManager.Interface.LoadingFeedback.Show("requesting room");
            RequestedRoom = (betChoice, capacityChoice);
            _netManager.Send("RequestRoom", 0, betChoice, capacityChoice);
            _appManager.Interface.LoadingFeedback.Hide();
        }
    }
}