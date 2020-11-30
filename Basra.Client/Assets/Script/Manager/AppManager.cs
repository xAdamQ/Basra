using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SignalRCore.Messages;
using BestHTTP.SignalRCore.Authentication;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using BestHTTP.Futures;
using System.Reflection;
using System.Linq;

namespace Basra.Client
{
    public class AppManager : MonoBehaviour
    {
        public static AppManager I;

        public HubConnection HubConnection;
        public int Money;
        public User User;
        public ActionRecord LastAction;

        private async Task Awake()
        {
            I = this;

            DontDestroyOnLoad(this);

            FetchRpcs();

#if UNITY_EDITOR
            // SceneManager.UnloadSceneAsync(1);
            HubConnection = await Connect("5");
            Debug.Log("reached so far");
#endif
        }

        public GameObject TestLoginUI;
        public InputField TestFbigInf;

        public RoomManager Room;
        public LobbyManger Lobby;

        //iterate over the assmbly for the methods, but I need object instances

        public List<MethodInfo> Rpcs = new List<MethodInfo>();

        public async Task TestConnect()
        {
            HubConnection = await Connect(TestFbigInf.text);
        }

        public async Task<HubConnection> Connect(string fbigToken)
        {
            var protocol = new JsonProtocol(new LitJsonEncoder());

            // var fbigToken = "To1b7XND62yJ_mUCX2emTc8lzdeIUy-Uor95jWAPzcY.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZF9hdCI6MTU5NjUyMjcwMCwicGxheWVyX2lkIjoiMzYzMjgxNTU2MDA5NDI3MyIsInJlcXVlc3RfcGF5bG9hZCI6bnVsbH0";

            var uriBuilder = new UriBuilder("http://localhost:5000/connect");
            uriBuilder.Query += $"access_token={fbigToken}";

            var hubConnection = new HubConnection(uriBuilder.Uri, protocol)
            {
                ReconnectPolicy = new ReconnectPolicy(),
            };

            //I don't have this term "authentication" despite I make token authentication
            // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

            hubConnection.OnConnected += OnConntected;
            hubConnection.OnError += OnError;
            hubConnection.OnClosed += OnClosed;
            hubConnection.OnMessage += OnMessage;

            // hubConnection.On<int, int>(nameof(Lobby.EnterRoom), (genre, playerCount) => Lobby.EnterRoom(genre, playerCount)/*will make it runtime evaluated*/);
            // hubConnection.On(nameof(Lobby.RoomIsFilling), () => Lobby.RoomIsFilling());
            // hubConnection.On<int[]>(nameof(Room.Distribute), (hand) => Room.Distribute(hand));
            // hubConnection.On<int[], int[]>(nameof(Room.InitialDistribute), (hand, ground) => Room.InitialDistribute(hand, ground));
            // hubConnection.On(nameof(Room.InitialDistribute), () => Lobby.RoomIsFilling());

            User = new User
            {
                FbId = fbigToken,
            };

            return await hubConnection.ConnectAsync();
        }

        public void Disconnect()
        {
            HubConnection.CloseAsync();
        }

        #region events
        private bool OnMessage(HubConnection arg1, Message message)
        {
            Debug.Log($"OnMessage {message}");
            //call the rpcs using reflection and lobby, room objects
            // [Invocation Id: , Target: 'EnterRoom', Argument count: 2, Stream Ids: 0]
            if (!string.IsNullOrEmpty(message.target))
            {
                var method = Rpcs.Find(m => m.Name == message.target);
                if (method.DeclaringType == typeof(LobbyManger))
                    method.Invoke(Lobby, message.arguments);
                else if (method.DeclaringType == typeof(RoomManager))
                    method.Invoke(Room, message.arguments);
            }

            return true;
        }
        private void OnClosed(HubConnection obj)
        {
            Debug.Log("OnClosed");
        }
        private void OnConntected(HubConnection obj)
        {
            Debug.Log("OnConntected");
            TestLoginUI.SetActive(false);
            SceneManager.LoadScene(1);
        }
        private void OnError(HubConnection arg1, string arg2)
        {
            Debug.Log($"OnError: {arg2}");
        }
        #endregion

        #region helpers
        [SerializeField]
        private LoadingPanel LaodingPanel;
        public void ShowLoadingPanel(string message = "")
        {
            LaodingPanel.MessageText.text = message;
            LaodingPanel.gameObject.SetActive(true);
        }
        public void StopLoadingPanel()
        {
            LaodingPanel.gameObject.SetActive(false);
        }
        #endregion

        private void FetchRpcs()
        {
            //you need instance of each object of the fun when server calls
            //get the type of each one and pass the right object

            var namespaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && t.Namespace == "Basra.Client");

            foreach (var type in namespaceTypes)
            {
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute(typeof(RpcAttribute));

                    if (attribute == null) continue;

                    Rpcs.Add(method);
                }
            }
        }
    }
}