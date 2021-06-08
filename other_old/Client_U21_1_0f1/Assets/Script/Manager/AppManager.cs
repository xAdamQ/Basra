using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SignalRCore.Authentication;
using System.Threading.Tasks;
using BestHTTP.Futures;
using System.Linq;
using BestHTTP;
using BestHTTP.Logger;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using Zenject;
using Random = UnityEngine.Random;

//action is function instances(require object)
//methodInfo is not
//so for every scoped life object we fetch it's actions with reflections (methodInfo to action with an object)
//the id for a method in server side(sendAsync) is a single string, so function name must be unique acreoss all types

namespace Basra.Client
{
    public interface IAppManager : IInitializable
    {
        public IAppInterface Interface { get; }
        public User User { get; }
    }

    public class AppManager : IAppManager
    {
        private readonly IAppInterface _interface;
        public IAppInterface Interface => _interface;

        private readonly ZenjectSceneLoader _sceneLoader;
        private readonly INetManager _netManager;

        private readonly User user;
        public User User => user;
        public List<Room.Card> Eaten;

        [Inject]
        public AppManager(ZenjectSceneLoader sceneLoader, INetManager netManager, IAppInterface appInterface)
        {
            _sceneLoader = sceneLoader;
            _netManager = netManager;
            _interface = appInterface;
            user = new User();
        }

        //trivial
        private void Connect(string fbigToken)
        {
            _netManager.Conncted += OnConnected;
            user.FbId = fbigToken;
            NetManager.I.Connect(fbigToken);
        }

        private void OnConnected()
        {
            _sceneLoader.LoadScene("Lobby", LoadSceneMode.Additive);
        }

        public void Initialize()
        {
            //HTTPManager.Logger.Level = BestHTTP.Logger.Loglevels.;
            HTTPManager.Logger = new MyBestHttpLogger();
            //HTTPManager.Logger.Output = new MyBestHttpOutput();

            UniTask.WhenAll
            (
                Room.Ground.StaticInit(),
                Room.Card.StaticInit(),
                Room.Front.StaticInit()
            ).ContinueWith(() =>
            {
                new NetManager(new[] {typeof(Room.Room), typeof(Lobby)});
                Connect(Random.Range(0, 9999999).ToString());
            }).Forget(e => throw e);
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
//
// //manually tested, should be moved to non mono class        
// private Type[] FetchManagersRpcs()
// {
//     //you need instance of each object of the fun when server calls
//     //get the type of each one and pass the right object
//
//     var namespaceTypes = AppDomain.CurrentDomain.GetAssemblies()
//         .SelectMany(a => a.GetTypes())
//         .Where(t => t.IsClass && (t.Namespace == "Basra.Client" || t.Namespace == "Basra.Client.Room"));
//
//     return namespaceTypes.Where(t => t.Name.EndsWith("Manager")).ToArray();
// }

#endregion