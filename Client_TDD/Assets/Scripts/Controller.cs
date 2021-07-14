using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore.Messages;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface IController
{
    void InitGame(PersonalFullUserInfo myFullUserInfo, MinUserInfo[] yesterdayChampions,
        MinUserInfo[] topFriends, ActiveRoomState activeRoomState);

    //they are unitasks because they are rpcs
    UniTask<FullUserInfo> GetPublicFullUserInfo(string userId);
    UniTask<object> SendAsync(string method, params object[] args);
    void ThrowCard(int cardId);
    UniTask NotifyTurnMiss();
    UniTask SelectCardback(int cardbackIndex);
    UniTask BuyCardback(int index);
    UniTask RequestRandomRoom(int betChoice, int capacityChoice);
    UniTask Surrender();

    void TstStartClient(string id);
    void UpdatePersonalInfo(PersonalFullUserInfo obj);

    void AssignRpc(Action action, string moduleName);
    void AssignRpc<T1>(Action<T1> action, string moduleName);
    void AssignRpc<T1, T2>(Action<T1, T2> action, string moduleName);
    void AssignRpc<T1, T2, T3>(Action<T1, T2, T3> action, string moduleName);
    void AssignRpc<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, string moduleName);

    void RemoveModuleRpcs(string moduleName);
    UniTask<MinUserInfo> TestWaitWithReturn();
}

public class Controller : IController, IInitializable
{
    [Inject] private readonly IRepository _repository;
    [Inject] private readonly LobbyController.Factory _lobbyFactory;
    [Inject] private readonly RoomController.Factory _roomFactory;

    public void Initialize()
    {
        HTTPManager.Logger = new MyBestHttpLogger();

#if UNITY_EDITOR
        UnityEngine.Object.Destroy(GameObject.Find("tst client buttons"));
        ConnectToServer("0").Forget();
        AssignGeneralRpcs();
#endif
    }

    public void TstStartClient(string id)
    {
        ConnectToServer(id).Forget();
        AssignGeneralRpcs();
    }

    public void InitGame(PersonalFullUserInfo myFullUserInfo, MinUserInfo[] yesterdayChampions,
        MinUserInfo[] topFriends, ActiveRoomState activeRoomState)
    {
        _repository.PersonalFullInfo = myFullUserInfo;
        _repository.YesterdayChampions = yesterdayChampions;
        _repository.TopFriends = topFriends;

        LoadAppropriateModules(activeRoomState);
    }

    public void UpdatePersonalInfo(PersonalFullUserInfo personalFullUserInfo)
    {
        _repository.PersonalFullInfo = personalFullUserInfo;
    }

    #region rpc works

    private void LoadAppropriateModules(ActiveRoomState activeRoomState)
    {
        if (activeRoomState == null)
            _lobbyFactory.Create();
        else
            _roomFactory.Create(new RoomSettings(activeRoomState), activeRoomState);
    }

    private void AssignGeneralRpcs()
    {
        hubConnection.On<PersonalFullUserInfo, MinUserInfo[], MinUserInfo[], ActiveRoomState>(nameof(InitGame), InitGame);
        hubConnection.On<PersonalFullUserInfo>(nameof(UpdatePersonalInfo), UpdatePersonalInfo);
    }

    private Dictionary<string, List<string>> RpcsNames = new Dictionary<string, List<string>>();

    private void SaveRpcName(string actionName, string moduleName)
    {
        if (RpcsNames.ContainsKey(moduleName))
            RpcsNames[moduleName].Add(actionName);
        else
            RpcsNames.Add(moduleName, new List<string> { actionName });
    }

    public void AssignRpc(Action action, string moduleName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleName);
    }
    public void AssignRpc<T1>(Action<T1> action, string moduleName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleName);
    }
    public void AssignRpc<T1, T2>(Action<T1, T2> action, string moduleName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleName);
    }
    public void AssignRpc<T1, T2, T3>(Action<T1, T2, T3> action, string moduleName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleName);
    }
    public void AssignRpc<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, string moduleName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleName);
    }

    public void RemoveModuleRpcs(string moduleName)
    {
        RpcsNames[moduleName].ForEach(_ => hubConnection.Remove(_));
    }

    #endregion

    #region hub

    private HubConnection hubConnection;
    private readonly string address = "http://localhost:5000/connect";
    //private readonly string address = "https://tstappname.azurewebsites.net/connect";
    private readonly IProtocol protocol = new JsonProtocol(new LitJsonEncoder());
    private readonly MyReconnectPolicy myReconnectPolicy = new MyReconnectPolicy();

    private async UniTask ConnectToServer(string fbigToken)
    {
        Debug.Log("connecting with token " + fbigToken);

        var uriBuilder = new UriBuilder(address);
        uriBuilder.Query += $"access_token={fbigToken}";

        hubConnection = new HubConnection(uriBuilder.Uri, protocol)
        {
            ReconnectPolicy = myReconnectPolicy
        };

        //I don't have this term "authentication" despite I make token authentication
        // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

        hubConnection.OnConnected += OnConnected;
        hubConnection.OnError += OnError;
        hubConnection.OnClosed += OnClosed;
        hubConnection.OnMessage += OnMessage;

        await hubConnection.ConnectAsync();
    }

    private bool OnMessage(HubConnection arg1, Message msg)
    {
        Debug.Log($"msg is {JsonConvert.SerializeObject(msg, Formatting.Indented)}");

        return true;
    }
    private void OnConnected(HubConnection obj)
    {
        Debug.Log("connected to server");
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

    #region out rpcs

    public async UniTask<FullUserInfo> GetPublicFullUserInfo(string userId)
    {
        return await hubConnection.InvokeAsync<FullUserInfo>("GetPublicInfo");
    }
    public async UniTask NotifyTurnMiss()
    {
        await hubConnection.SendAsync("MissTurn");
    }
    public void ThrowCard(int cardId)
    {
        hubConnection.Send("Throw", cardId).OnError(e => throw e);
    }
    public async UniTask SelectCardback(int cardbackIndex)
    {
        await hubConnection.SendAsync("SelectCardback", cardbackIndex);
    }
    public async UniTask BuyCardback(int index)
    {
        await hubConnection.SendAsync("BuyCardback", index);
    }
    public async UniTask RequestRandomRoom(int betChoice, int capacityChoice)
    {
        await hubConnection.SendAsync("RequestRandomRoom", betChoice, capacityChoice);
    }
    public async UniTask Surrender()
    {
        await hubConnection.SendAsync("Surrender");
    }

    #endregion

    public async UniTask<object> SendAsync(string method, params object[] args)
    {
        return await hubConnection.SendAsync(method, args);
    }

    public async UniTask<MinUserInfo> TestWaitWithReturn()
    {
        return await hubConnection.InvokeAsync<MinUserInfo>("TestReturnObject");
    }
}