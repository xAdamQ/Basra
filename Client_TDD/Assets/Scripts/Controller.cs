using System;
using System.Collections.Generic;
using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

public interface IController
{
    void InitGame(PersonalFullUserInfo myFullUserInfo, MinUserInfo[] yesterdayChampions,
        MinUserInfo[] topFriends);

    //they are unitasks because they are rpcs
    UniTask<FullUserInfo> GetPublicFullUserInfo(string userId);
    UniTask<object> SendAsync(string method, params object[] args);
    UniTask<ThrowResponse> ThrowCard(int cardId);
    UniTask NotifyTurnMiss();
    UniTask SelectCardback(int cardbackIndex);
    UniTask BuyCardback(int Index);
    UniTask RequestRandomRoom(int betChoice, int capacityChoice);
    void AddLobbyRpcs(ILobbyController lobbyController);
    UniTask Surrender();
    void RemoveLobbyRpcs();

    void TstStartClient(string id);
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
        Object.Destroy(GameObject.Find("tst client buttons"));
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
        _roomFactory.Create(new RoomSettings(0, 0,
            new List<RoomOppoInfo> {new RoomOppoInfo {TurnId = 1, FullUserInfo = new FullUserInfo {Id = "0"}}}, 0));

        // _lobbyFactory.Create();
        Debug.Log("the lobby modules are loaded");
        //depending on in room or in lobby
        //lobby modules is loaded by it's context
    }

    private void AssignGeneralRpcs()
    {
        HubConnection.On<PersonalFullUserInfo, MinUserInfo[], MinUserInfo[]>(nameof(InitGame), InitGame);
    }

    private List<string> LobbyRpcNames;
    public void AddLobbyRpcs(ILobbyController lobbyController)
    {
        LobbyRpcNames = new List<string>();

        HubConnection.On<List<RoomOppoInfo>, int>(nameof(lobbyController.StartRequestedRoomRpc),
            lobbyController.StartRequestedRoomRpc);
        LobbyRpcNames.Add(nameof(lobbyController.StartRequestedRoomRpc));
    }
    public void RemoveLobbyRpcs()
    {
        LobbyRpcNames.ForEach(_ => HubConnection.Remove(_));
    }

    public const string ADDRESS = "http://localhost:5000/connect";
    private HubConnection HubConnection;
    private IProtocol Protocol = new JsonProtocol(new LitJsonEncoder());
    private MyReconnectPolicy _myReconnectPolicy = new MyReconnectPolicy();

    private async UniTask ConnectToServer(string fbigToken)
    {
        Debug.Log("connecting with token " + fbigToken);

        var uriBuilder = new UriBuilder(ADDRESS);
        uriBuilder.Query += $"access_token={fbigToken}";

        HubConnection = new HubConnection(uriBuilder.Uri, Protocol)
        {
            ReconnectPolicy = _myReconnectPolicy
        };

        //I don't have this term "authentication" despite I make token authentication
        // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

        HubConnection.OnConnected += OnConnected;
        HubConnection.OnError += OnError;
        HubConnection.OnClosed += OnClosed;
        // HubConnection.OnMessage += OnMessage;

        await HubConnection.ConnectAsync();
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


    //call the server
    public async UniTask<FullUserInfo> GetPublicFullUserInfo(string userId)
    {
        return await HubConnection.InvokeAsync<FullUserInfo>("GetPublicInfo");
    }
    public async UniTask NotifyTurnMiss()
    {
        await HubConnection.SendAsync("MissTurn");
    }
    public async UniTask<ThrowResponse> ThrowCard(int cardId)
    {
        return await HubConnection.InvokeAsync<ThrowResponse>("Throw", cardId);
    }
    public async UniTask SelectCardback(int cardbackIndex)
    {
        await HubConnection.SendAsync("SelectCardback", cardbackIndex);
    }
    public async UniTask BuyCardback(int Index)
    {
        await HubConnection.SendAsync("BuyCardback", Index);
    }
    public async UniTask RequestRandomRoom(int betChoice, int capacityChoice)
    {
        await HubConnection.SendAsync("RequestRandomRoom", betChoice, capacityChoice);
    }
    public async UniTask Surrender()
    {
        await HubConnection.SendAsync("Surrender");
    }

    public async UniTask<object> SendAsync(string method, params object[] args)
    {
        return await HubConnection.SendAsync(method, args);
    }
}