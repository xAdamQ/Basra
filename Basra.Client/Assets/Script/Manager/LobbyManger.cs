using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestHTTP.SignalRCore;
using UnityEngine.SceneManagement;

public class LobbyManger : MonoBehaviour
{
    [SerializeField] Image UserPic;
    [SerializeField] Text UserName;

    private void Awake()
    {
        AppManager.I.Lobby = this;
    }

    void Start()
    {
        UserName.text = AppManager.I.User.FbId.ToString();
        // UserName.text = AppManger.I.FirebaseAuth.CurrentUser.DisplayName;
    }

    //rpc
    public void EnterRoom(int genre, int playerCount)
    {
        Debug.Log("Should Enter Room");
        RoomManager.Genre = genre;
        RoomManager.PlayerCount = playerCount;
        SceneManager.LoadScene(2);
        AppManager.I.StopLoadingPanel();//todo make it hide after async load scene
    }

    //rpc
    public void RoomIsFilling()
    {
        AppManager.I.ShowLoadingPanel("Filling The Room");
    }

    //button
    public void AskForRoom(int genre, int playerCount)
    {
        AppManager.I.HubConnection.Send("AskForRoom", genre, playerCount);
    }

}
