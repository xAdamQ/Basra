using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Antlr.Runtime.Tree;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore.Messages;
using Cysharp.Threading.Tasks;
using MiscUtil.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = System.Object;

namespace Basra.Client
{
    public interface INetManager
    {
        void Connect(string fbigToken);
        event Action Conncted;
        BestHTTP.Futures.IFuture<object> Send(string method, params object[] args);
    }

    /// <summary>
    /// connection related stuff
    /// 
    /// </summary>
    public class NetManager : INetManager
    {
        public static NetManager I;

        private List<Type> RpcContainers { get; } = new List<Type>();
        private List<MethodInfo> Rpcs { get; } = new List<MethodInfo>();

        private HubConnection HubConnection { get; set; }

        public NetManager(Type[] rpcContainers)
        {
            I = this;
            RpcContainers.AddRange(rpcContainers);
            FetchContainersRpcs();
        }

        private const string ConnectionUrl = "http://localhost:5000/connect";
        private readonly IProtocol Protocol = new JsonProtocol(new LitJsonEncoder());
        private readonly IRetryPolicy RetryPolicy = new MyReconnectPolicy();
        private const string TokenQuery = "access_token";

        //skip for now, too advanced but manually tested
        public void Connect(string fbigToken)
        {
            // var fbigToken = "To1b7XND62yJ_mUCX2emTc8lzdeIUy-Uor95jWAPzcY.eyJhbGdvcml0aG0iOiJITUFDLVNIQTI1NiIsImlzc3VlZF9hdCI6MTU5NjUyMjcwMCwicGxheWVyX2lkIjoiMzYzMjgxNTU2MDA5NDI3MyIsInJlcXVlc3RfcGF5bG9hZCI6bnVsbH0";

            //I don't have this term "authentication" despite I make token authentication
            // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

            var uriBuilder = new UriBuilder(ConnectionUrl)
            {
                Query = $"{TokenQuery}={fbigToken}"
            };
            HubConnection = new HubConnection(uriBuilder.Uri, Protocol)
            {
                ReconnectPolicy = RetryPolicy,
            };

            HubConnection.OnConnected += OnConnected;
            HubConnection.OnError += OnError;
            HubConnection.OnClosed += OnClosed;
            HubConnection.OnMessage += OnMessage;

            // hubConnection.On<int, int>(nameof(Lobby.EnterRoom), (genre, playerCount) => Lobby.EnterRoom(genre, playerCount)/*will make it runtime evaluated*/);
            // HubConnection.On(nameof(Lobby.RoomIsFilling), () => Lobby.RoomIsFilling());
            // hubConnection.On<int[]>(nameof(Room.Distribute), (hand) => Room.Distribute(hand));
            // hubConnection.On<int[], int[]>(nameof(Room.InitialDistribute), (hand, ground) => Room.InitialDistribute(hand, ground));
            // hubConnection.On(nameof(Room.InitialDistribute), () => Lobby.RoomIsFilling());

            HubConnection.ConnectAsync();
        }

        public event Action Conncted;

        #region events

        private void OnConnected(HubConnection obj)
        {
            Conncted?.Invoke();
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

        public Action LastRevertAction;

        //manually tested
        private void HandleInvocationMessage(Message message)
        {
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

            var manager = RpcContainers.First(obj => obj.GetType() == method.DeclaringType);

            if (method.Name.StartsWith("Override"))
            {
                LastRevertAction?.Invoke();
                LastRevertAction = null;
            }

            method.Invoke(manager, realArgs); //the only problem
        }

        //tested manually
        private void FetchContainersRpcs()
        {
            //you need instance of each object of the fun when server calls
            //get the type of each one and pass the right object

            foreach (var type in RpcContainers)
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

        public BestHTTP.Futures.IFuture<object> Send(string method, params object[] args)
        {
            return HubConnection.Send(method, args);
        }
    }
}