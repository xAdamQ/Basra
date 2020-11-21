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
using Basra.Core;
using UnityEngine.SceneManagement;

public class AppManager : MonoBehaviour
{
    public static AppManager I;

    public HubConnection HubConnection;
    public int Money;

    private void Awake()
    {
        I = this;

        DontDestroyOnLoad(this);

#if UNITY_EDITOR
        // SceneManager.UnloadSceneAsync(1);
        Connect("5");
#endif

    }

    public GameObject TestLoginUI;
    public InputField TestFbigInf;
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

        HubConnection = new HubConnection(uriBuilder.Uri, protocol);

        //I don't have this term "authentication" despite I make token authentication
        // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

        HubConnection.OnConnected += OnConntected;
        HubConnection.OnError += OnError;
        HubConnection.OnClosed += OnClosed;
        HubConnection.OnMessage += OnMessage;

        // HubConnection.On<int>(nameof(AddMoneyAsync), AddMoney);
        HubConnection.On<int, int>(nameof(LobbyManger.Current.EnterRoom), (genre, playerCount) => LobbyManger.Current.EnterRoom(genre, playerCount)/*will make it runtime evaluated*/);
        HubConnection.On<List<int>>(nameof(RoomManager.Current.SetHand), (hand) => RoomManager.Current.SetHand(hand));
        HubConnection.On(nameof(LobbyManger.Current.RoomIsFilling), () => LobbyManger.Current.RoomIsFilling());

        HubConnection.ConnectAsync();
    }

    public void Disconnect()
    {
        HubConnection.CloseAsync();
    }

    #region events
    private bool OnMessage(HubConnection arg1, Message arg2)
    {
        Debug.Log($"OnMessage {arg2}");
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

}