using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SignalRCore;
using UnityEngine.SceneManagement;
using System.Reflection;

namespace Basra.Client
{
    public class LobbyManger : MonoBehaviour
    {
        [SerializeField] Image UserPic;
        [SerializeField] Text UserName;

        private void Awake()
        {
            AppManager.I.Lobby = this;
            AppManager.I.Managers.Add(this);
        }
        private void OnDestroy()
        {
            AppManager.I.Managers.Remove(this);
        }

        void Start()
        {
            UserName.text = AppManager.I.User.FbId.ToString();
            // UserName.text = AppManger.I.FirebaseAuth.CurrentUser.DisplayName;
        }

        [Rpc]
        public void StartRoom(int myTurnId, string[] userNames)
        {
            Room.RoomManager.UserNames = userNames;
            Room.RoomManager.MyTurnId = myTurnId;

            SceneManager.LoadScene(2);

            AppManager.I.StopLoadingPanel();//todo make it hide after async load scene
        }

        [Rpc]
        public void RoomIsFilling()
        {
            AppManager.I.ShowLoadingPanel("Filling The Room");
        }

        //button
        public void AskForRoom(int genre, int playerCount)
        {
            Room.RoomManager.Genre = genre;
            Room.RoomManager.PlayerCount = playerCount;
            AppManager.I.HubConnection.Send("AskForRoom", genre, playerCount);
        }


    }
}