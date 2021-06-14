using System;
using System.Collections.Generic;
using Basra.Models.Client;
using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore.Messages;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface IController
{
    void InitGame(PersonalFullUserInfo myFullUserInfo, MinUserInfo[] yesterdayChampions,
        MinUserInfo[] topFriends);

    //they are unitasks because they are rpcs
    UniTask<FullUserInfo> GetPublicFullUserInfo(string userId);
    UniTask<object> SendAsync(string method, params object[] args);
    void ThrowCard(int cardId);
    UniTask NotifyTurnMiss();
    UniTask SelectCardback(int cardbackIndex);
    UniTask BuyCardback(int index);
    UniTask RequestRandomRoom(int betChoice, int capacityChoice);
    void AddLobbyRpcs(ILobbyController lobbyController);
    UniTask Surrender();
    void RemoveLobbyRpcs();

    void TstStartClient(string id);
    void UpdatePersonalInfo(PersonalFullUserInfo obj);
}

public class Controller : IController, IInitializable
{
    private readonly IRepository _repository;
    private readonly LobbyController.Factory _lobbyFactory;
    private readonly RoomController.Factory _roomFactory;

    [Inject]
    public Controller(IRepository repository,
        LobbyController.Factory lobbyFactory, RoomController.Factory roomFactory)
    {
        _repository = repository;
        _lobbyFactory = lobbyFactory;
        _roomFactory = roomFactory;
    }

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
        MinUserInfo[] topFriends)
    {
        _repository.PersonalFullInfo = myFullUserInfo;
        _repository.YesterdayChampions = yesterdayChampions;
        _repository.TopFriends = topFriends;

        //these actions mutate the data, and need to be inited #start

        myFullUserInfo.DownloadPicture().Forget(e => throw e);
        foreach (var user in yesterdayChampions)
            user.DownloadPicture().Forget(e => throw e);
        foreach (var user in topFriends)
            user.DownloadPicture().Forget(e => throw e);

        if (_repository.PersonalFullInfo.MoneyAimTimeLeft != null)
            _repository.PersonalFullInfo.DecreaseMoneyAimTimeLeft().Forget(e => throw e);

        //#end

        LoadAppropriateModules();
    }

    private void LoadAppropriateModules()
    {
        // _roomFactory.Create(new RoomSettings(0, 0,
        // new List<RoomOppoInfo> { new RoomOppoInfo { TurnId = 1, FullUserInfo = new FullUserInfo { Id = "0" } } }, 0));

        _lobbyFactory.Create();
        Debug.Log("the lobby modules are loaded");
        //depending on in room or in lobby
        //lobby modules is loaded by it's context
    }

    private void AssignGeneralRpcs()
    {
        hubConnection.On<PersonalFullUserInfo, MinUserInfo[], MinUserInfo[]>(nameof(InitGame), InitGame);
        hubConnection.On<PersonalFullUserInfo>(nameof(UpdatePersonalInfo), UpdatePersonalInfo);
    }


    private List<string> lobbyRpcNames;
    public void AddLobbyRpcs(ILobbyController lobbyController)
    {
        lobbyRpcNames = new List<string>();

        hubConnection.On<List<RoomOppoInfo>, int>(nameof(lobbyController.PrepareRequestedRoomRpc),
            lobbyController.PrepareRequestedRoomRpc);
        lobbyRpcNames.Add(nameof(lobbyController.PrepareRequestedRoomRpc));
    }
    public void RemoveLobbyRpcs()
    {
        lobbyRpcNames.ForEach(_ => hubConnection.Remove(_));
    }

    private List<string> roomRpcNames;
    public void AddRoomRpcs(IRoomController roomController)
    {
        roomRpcNames = new List<string>();

        hubConnection.On<ThrowResult>(nameof(roomController.MyThrowResult),
            roomController.MyThrowResult);
        lobbyRpcNames.Add(nameof(roomController.MyThrowResult));

        hubConnection.On<List<int>, List<int>>(nameof(roomController.StartRoomRpc),
            roomController.StartRoomRpc);
        lobbyRpcNames.Add(nameof(roomController.StartRoomRpc));

        hubConnection.On<List<int>>("Distribute",
            roomController.PlayersDistribute);
        lobbyRpcNames.Add("Distribute");
        //* this is an exc on naming convention

        hubConnection.On<ThrowResult>(nameof(roomController.ForcePlay),
            roomController.ForcePlay);
        lobbyRpcNames.Add(nameof(roomController.ForcePlay));

        hubConnection.On<ThrowResult>(nameof(roomController.CurrentOppoThrow),
            roomController.CurrentOppoThrow);
        lobbyRpcNames.Add(nameof(roomController.CurrentOppoThrow));


    }
    public void RemoveRoomRpcs()
    {
        roomRpcNames.ForEach(_ => hubConnection.Remove(_));
    }

    private HubConnection hubConnection;
    private readonly string address = "http://localhost:5000/connect";
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
        Debug.Log($"msg is {JsonUtility.ToJson(msg)}");
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


    public void UpdatePersonalInfo(PersonalFullUserInfo personalFullUserInfo)
    {
        _repository.PersonalFullInfo = personalFullUserInfo;
    }


    //call the server
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

    public async UniTask<object> SendAsync(string method, params object[] args)
    {
        return await hubConnection.SendAsync(method, args);
    }
}