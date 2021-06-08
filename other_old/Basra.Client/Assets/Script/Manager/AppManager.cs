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
using Random = UnityEngine.Random;

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

        public Room.RoomManager RoomManager;
        public LobbyManager LobbyManager;

        #endregion

        public Action LastRevertAction;

        private void Awake()
        {
            I = this;

            DontDestroyOnLoad(this);

            FetchManagersRpcs();

#if UNITY_EDITOR
            // SceneManager.UnloadSceneAsync(1);
            Connect(Random.Range(0, 1000).ToString());
#endif
        }

        private async void Start()
        {
            Managers.Add(this);

            //HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.;
            HTTPManager.Logger = new MyBestHttpLogger();
            //HTTPManager.Logger.Output = new MyBestHttpOutput();

            await UniTask.WhenAll(new UniTask[]
            {
                Room.User.StaticInit(),
                Room.Ground.StaticInit(),
                Room.Card.StaticInit(),
                Room.Front.StaticInit(),
            });
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
            //HubConnection.Send("TestAsync");
            SceneManager.LoadScene(1);
        }

        private bool OnMessage(HubConnection arg1, Message message)
        {
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

        #endregion

        public LoadingPanel LaodingPanel;

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
                //Debug.Log("we picked: " + type.Name);
                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute(typeof(RpcAttribute));

                    if (attribute == null) continue;

                    Rpcs.Add(method);
                }
            }
        }

        #region testing

        [Rpc]
        public void TestCall()
        {
            Debug.Log("TestCall *******************************************8");
        }

        #endregion

        private void HandleInvocationMessage(Message message)
        {
            if (message.target == "StartRoom")
            {

            }
            var method = Rpcs.Find(m => m.Name == message.target);

            var realArgs = HubConnection.Protocol.GetRealArguments(method.GetParameterTypes(), message.arguments);

            var debugMessage = "HandleInvocationMessage " + message.target;
            if (realArgs != null)
            {
                debugMessage += "  with args:  ";
                foreach (var item in realArgs)
                {
                    debugMessage += item.ToString() + "  --  ";
                }
            }

            Debug.Log(debugMessage);

            var manager = Managers.First(obj => obj.GetType() == method.DeclaringType);

            if (method.Name.StartsWith("Override"))
            {
                LastRevertAction?.Invoke();
                LastRevertAction = null;
            }

            method.Invoke(manager, realArgs); //the only problem
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