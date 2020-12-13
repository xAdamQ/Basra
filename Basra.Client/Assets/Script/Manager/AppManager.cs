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


//action is function instances(require object)
//methodInfo is not
//so for every scoped life object we fetch it's actions with reflections (methodInfo to action with an object)
//the id for a method in server side(sendAsync) is a single string, so function name must be unique acreoss all types


namespace Basra.Client
{
    public class AppManager : MonoBehaviour
    {
        #region props
        public static AppManager I;

        public List<object> Managers = new List<object>();
        public List<MethodInfo> Rpcs = new List<MethodInfo>();

        public HubConnection HubConnection;
        public int Money;
        public User User;

        public GameObject TestLoginUI;
        public InputField TestFbigInf;

        public Room.RoomManager Room;
        public LobbyManager Lobby;
        #endregion

        private void Awake()
        {
            I = this;

            DontDestroyOnLoad(this);

            FetchManagersRpcs();


#if UNITY_EDITOR
            // SceneManager.UnloadSceneAsync(1);
            Connect("5");
#endif

        }
        private void Start()
        {
            Managers.Add(this);
        }
        public void TestConnect()
        {
            Connect(TestFbigInf.text);
        }
        public void Connect(string fbigToken)
        {
            var protocol = new JsonProtocol(new LitJsonEncoder());

            // var fbigToken = "To1b7XND62yJ_mUCX2emTc8lzdeIUy-Uor95jWAPzcY.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZF9hdCI6MTU5NjUyMjcwMCwicGxheWVyX2lkIjoiMzYzMjgxNTU2MDA5NDI3MyIsInJlcXVlc3RfcGF5bG9hZCI6bnVsbH0";

            var uriBuilder = new UriBuilder("http://localhost:5000/connect");
            uriBuilder.Query += $"access_token={fbigToken}";

            HubConnection = new HubConnection(uriBuilder.Uri, protocol)
            {
                ReconnectPolicy = new ReconnectPolicy(),
            };

            //I don't have this term "authentication" despite I make token authentication
            // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

            HubConnection.OnConnected += OnConntected;
            HubConnection.OnError += OnError;
            HubConnection.OnClosed += OnClosed;
            HubConnection.OnMessage += OnMessage;

            // hubConnection.On<int, int>(nameof(Lobby.EnterRoom), (genre, playerCount) => Lobby.EnterRoom(genre, playerCount)/*will make it runtime evaluated*/);
            // HubConnection.On(nameof(Lobby.RoomIsFilling), () => Lobby.RoomIsFilling());
            // hubConnection.On<int[]>(nameof(Room.Distribute), (hand) => Room.Distribute(hand));
            // hubConnection.On<int[], int[]>(nameof(Room.InitialDistribute), (hand, ground) => Room.InitialDistribute(hand, ground));
            // hubConnection.On(nameof(Room.InitialDistribute), () => Lobby.RoomIsFilling());



            User = new User
            {
                FbId = fbigToken,
            };

            HubConnection.ConnectAsync();
        }

        #region events
        private void OnConntected(HubConnection obj)
        {
            HubConnection.Send("TestAsync");
            SceneManager.LoadScene(1);
        }
        private bool OnMessage(HubConnection arg1, Message message)
        {
            Debug.Log($"OnMessage {message}");
            //call the rpcs using reflection and lobby, room objects
            // [Invocation Id: , Target: 'EnterRoom', Argument count: 2, Stream Ids: 0]

            if (message.type == MessageTypes.Invocation)
            {
                var method = Rpcs.Find(m => m.Name == message.target);

                var realArgs = HubConnection.Protocol.GetRealArguments(method.GetParameterTypes(), message.arguments);

                if (realArgs != null)
                {
                    var debugMessage = "args are:  ";
                    foreach (var item in realArgs)
                    {
                        debugMessage += item.ToString() + "  --  ";
                    }
                    Debug.Log(debugMessage);
                }
                //debug args

                var manager = Managers.First(obj => obj.GetType() == method.DeclaringType);

                if (method.Name.StartsWith("Override"))
                    InstantRpcRecord.Current?.RevertVisuals();

                method.Invoke(manager, realArgs);//the only problem

                return false;
            }

            return true;
            //if i returned false the message will be discarded internally, (no action of besthttp)
        }
        private void OnClosed(HubConnection obj)
        {
            Debug.Log("OnClosed");
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

        private void FetchManagersRpcs()
        {
            //you need instance of each object of the fun when server calls
            //get the type of each one and pass the right object

            var namespaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && (t.Namespace == "Basra.Client" || t.Namespace == "Basra.Client.Room"));

            foreach (var type in namespaceTypes)
            {
                if (!type.Name.EndsWith("Manager")) continue;
                Debug.Log("we picked: " + type.Name);
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute(typeof(RpcAttribute));

                    if (attribute == null) continue;

                    Rpcs.Add(method);
                }
            }
        }
        #endregion

        public void SendUnconfirmed(string method, params object[] args)
        {
            HubConnection.Send(method, args)
            .OnSuccess(future =>
            {
                Debug.Log(method + " confimred");
                InstantRpcRecord.Current?.Confirm();
            })
            .OnError(exc =>
            {
                Debug.Log("error happened on serevr: " + exc);
            });
        }

        [Rpc]
        public void TestCall()
        {
            Debug.Log("TestCall *******************************************8");
        }
    }
}