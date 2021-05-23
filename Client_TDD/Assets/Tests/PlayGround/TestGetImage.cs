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

public class TestGetImage : MonoBehaviour
{
    public HubConnection HubConnection;

    private void Start()
    {
        Connect("1");
    }

    public void Connect(string fbigToken)
    {
        var protocol = new JsonProtocol(new LitJsonEncoder());

        var uriBuilder = new UriBuilder("http://localhost:5000/connect");
        uriBuilder.Query += $"access_token={fbigToken}";

        HubConnection = new HubConnection(uriBuilder.Uri, protocol);

        HubConnection.OnConnected += OnConntected;
        HubConnection.OnError += OnError;
        HubConnection.OnClosed += OnClosed;
        HubConnection.OnMessage += OnMessage;

        HubConnection.ConnectAsync();
    }

    public void setImage(Texture2D texture2D)
    {
    }

    #region events

    private void OnConntected(HubConnection obj)
    {
        HubConnection.Send("GetImage").OnComplete(future => setImage(future.value as Texture2D));
    }

    private bool OnMessage(HubConnection arg1, Message message)
    {
        //call the rpcs using reflection and lobby, room objects
        // [Invocation Id: , Target: 'EnterRoom', Argument count: 2, Stream Ids: 0]
        //
        // if (message.type == MessageTypes.Invocation)
        // {
        //     HandleInvocationMessage(message);
        //     return false;
        // }

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
}