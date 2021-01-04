using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SignalRCore;
using UnityEngine.SceneManagement;
using System.Reflection;
using Cysharp.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace Basra.Client
{
    public class LobbyManager : MonoBehaviour
    {
        private Client.LobbyManager Logical;
        private Lobby Lobby;

        [SerializeField] Image UserPic;
        [SerializeField] Text UserName;

        private void Awake()
        {
            Lobby = new Lobby(AppManager.I.ServerManager);

            //AppManager.I.LobbyManager = this;
        }

        void Start()
        {
            UserName.text = Logical.AppManager.User.FbId.ToString();
        }

        /*
         
        pick rpcs from logical managers
        fire them on OnMessage
         
        thus, how we call the upper level, which by the way may have args

        logical layer, the calls happen by prototyping and scenarios

        unity layer, calls happen by user interactions, or automated

        so the second layer behaves like test units with user ineractions
        but automated task, like server response?

        1- another implementation by namespaces
        2- extending their functionality
            but how to call them?
        3- saperated functions    

        by this design, you have to expose every usable method?

         */

        [Rpc]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void StartRoom(int myTurnId, string[] userNames)
        {
            SceneManager.LoadScene(2);
            AppManager.I.LaodingPanel.Hide();
        }

        [Rpc]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "<Pending>")]
        private void RoomIsFilling()
        {
            AppManager.I.LaodingPanel.Show("Filling The Room");
        }

        //button
        public void AskForRoom(int genre, int playerCount)
        {
            Lobby.AskForRoom(genre, playerCount);
            //Logical.AskForRoom(genre, playerCount);
        }

    }
}

namespace Basra.Client
{
    public class Lobby
    {
        public Room.RoomManager PendingRoom;

        public ServerManager ServerManager;

        public Lobby(ServerManager serverManager)
        {
            ServerManager = serverManager;
        }

        public void AskForRoom(int genre, int playerCount)
        {
            PendingRoom = new Room.RoomManager(AppManager, genre, playerCount);

            AppManager.HubConnection.Send("AskForRoom", genre, playerCount);
        }
    }
}

namespace Basra.Client.LogicSystem
{
    public class LobbyManager
    {
        public AppManager AppManager;
        public Room.RoomManager PendingRoom;

        public LobbyManager(AppManager appManager)
        {
            AppManager = appManager;

            AppManager.LobbyManager = this;
            AppManager.Managers.Add(this);
        }

        ~LobbyManager()
        {
            AppManager.Managers.Remove(this);
        }

        //[Rpc]
        //private void StartRoom(int myTurnId, string[] userNames)
        //{
        //    PendingRoom.Start(myTurnId, userNames);

        //    Debug.Log("entered room");
        //}

        //[Rpc]
        //private void RoomIsFilling()
        //{
        //    Debug.Log("room is filling");
        //}

        public void AskForRoom(int genre, int playerCount)
        {
            PendingRoom = new Room.RoomManager(AppManager, genre, playerCount);

            AppManager.HubConnection.Send("AskForRoom", genre, playerCount);
        }
    }
}
