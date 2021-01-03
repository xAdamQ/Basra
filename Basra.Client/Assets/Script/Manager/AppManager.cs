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
using BestHTTP;
using BestHTTP.Logger;
using Cysharp.Threading.Tasks;

namespace Basra.Client.Components
{
    public class AppManager : MonoBehaviour
    {
        public Client.AppManager Logical;

        public static AppManager I;

        public Room.Components.RoomManager RoomManager;
        public LobbyManager LobbyManager;

        public Action LastRevertAction;

        private void Awake()
        {
            Logical = new Client.AppManager();

            Logical.HubConnection.OnConnected += OnConntected;

            I = this;

            DontDestroyOnLoad(this);

            Logical.FetchManagersRpcs("Basra.Client.Components", "Basra.Client.Components.Room");
        }

        private async void Start()
        {
            await UniTask.WhenAll(new UniTask[]
            {
                Room.Components.User.StaticInit(),
                Room.Components.Ground.StaticInit(),
                Room.Components.Card.StaticInit(),
                Room.Components.Front.StaticInit(),
            });
        }

        //automated, so its injected to the logic
        //but other types call the logic, like test units do
        private void OnConntected(HubConnection obj)
        {
            SceneManager.LoadScene(1);
        }

        public LoadingPanel LaodingPanel;

        public GameObject ManualLoginUI;
        public InputField FbigInputField;
        public void ManualIdConnect()
        {
            Logical.Connect(FbigInputField.text);
        }//button
    }
}

namespace Basra.Client
{
    public class AppManager
    {
        //public static AppManager I;

        public List<object> Managers = new List<object>();
        public List<MethodInfo> Rpcs = new List<MethodInfo>();

        public HubConnection HubConnection;
        public User User;

        public Room.RoomManager RoomManager;
        public LobbyManager LobbyManager;

        public Action LastRevertAction;

        public string TestFbigInf;

        public AppManager()
        {
            //I = this;

            FetchManagersRpcs("Basra.Client", "Basra.Client.Room");

            Managers.Add(this);
            HTTPManager.Logger = new MyBestHttpLogger();

#if UNITY_EDITOR
            Connect("5");
#endif

        }

        public void Connect(string fbigToken)
        {
            Debug.Log("connecting with id " + fbigToken);

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

        private void OnConntected(HubConnection obj)
        {
            //HubConnection.Send("TestAsync");
            //SceneManager.LoadScene(1);
            var roomManager = new LobbyManager(this);
        }
        private bool OnMessage(HubConnection arg1, Message message)
        {
            Debug.Log($"OnMessage {message}");
            //call the rpcs using reflection and lobby, room objects
            // [Invocation Id: , Target: 'EnterRoom', Argument count: 2, Stream Ids: 0]

            if (message.type == MessageTypes.Invocation)
            {
                HandleInvocationMessage(message);
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

        public void FetchManagersRpcs(params string[] namespaces)
        {
            var namespaceTypes = AppDomain.CurrentDomain.GetAssemblies()
           .SelectMany(a => a.GetTypes())
           .Where(t => t.IsClass && (namespaces.Contains(t.Namespace)));

            foreach (var type in namespaceTypes)
            {
                if (!type.Name.EndsWith("Manager")) continue;
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute(typeof(RpcAttribute));

                    if (attribute == null) continue;

                    Rpcs.Add(method);
                }
            }
        }

        private void HandleInvocationMessage(Message message)
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
            {
                LastRevertAction?.Invoke();
                LastRevertAction = null;
            }

            // InstantRpcRecord.Current?.Revert();

            method.Invoke(manager, realArgs);//the only problem
        }

        public void SendUnconfirmed(string method, Action onSuccess, Action revert = null, params object[] args)
        {
            LastRevertAction = revert;
            //is cleaned after server response
            //override or pt2 is called before any next unconfirmed action
            HubConnection.Send(method, args).OnSuccess((future) =>
            {
                LastRevertAction = null;
                onSuccess.Invoke();
            });

        }
    }
}

#region general instant feedback with reflection, currenlty deprecated
// public void SendUnconfirmed(string method, params object[] args)
// {
//     HubConnection.Send(method, args)
//     .OnSuccess(future =>
//     {
//         Debug.Log(method + " confimred");
//         InstantRpcRecord.Current?.Confirm();
//     })
//     .OnError(exc =>
//     {
//         Debug.Log("error happened on serevr: " + exc);
//     });
// }
#endregion