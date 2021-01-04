using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Basra.Client
{
    public class ServerManager
    {
        public const string ADDRESS = "http://localhost:5000/connect";

        public HubConnection HubConnection;
        private IProtocol Protocol = new JsonProtocol(new LitJsonEncoder());
        private ReconnectPolicy ReconnectPolicy = new ReconnectPolicy();

        public List<MethodInfo> Rpcs = new List<MethodInfo>();
        public List<object> RpcTypeInstances = new List<object>();
        public Type[] RpcTypes;

        public ServerManager()
        {
        }

        public void Connect(string fbigToken)
        {
            Debug.Log("connecting with id " + fbigToken);

            var uriBuilder = new UriBuilder(ADDRESS);
            uriBuilder.Query += $"access_token={fbigToken}";

            HubConnection = new HubConnection(uriBuilder.Uri, Protocol)
            {
                ReconnectPolicy = ReconnectPolicy,
            };

            //I don't have this term "authentication" despite I make token authentication
            // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

            HubConnection.OnConnected += OnConntected;
            HubConnection.OnError += OnError;
            HubConnection.OnClosed += OnClosed;
            HubConnection.OnMessage += OnMessage;

            //User = new User
            //{
            //    FbId = fbigToken,
            //};

            HubConnection.ConnectAsync();
        }

        public void FetchTypesRpcs(params Type[] types)
        {
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance);
                foreach (var method in methods)
                {
                    var attribute = method.GetCustomAttribute(typeof(RpcAttribute));

                    if (attribute == null) continue;

                    Rpcs.Add(method);
                }
            }

            RpcTypes = types;
        }

        private void OnConntected(HubConnection obj)
        {
            Debug.Log("connected to server");
            //HubConnection.Send("TestAsync");
            //SceneManager.LoadScene(1);
            //var roomManager = new LobbyManager(this);
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

            var manager = RpcTypeInstances.First(obj => obj.GetType() == method.DeclaringType);

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
