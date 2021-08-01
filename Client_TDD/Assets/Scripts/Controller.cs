using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore.Messages;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using Basra.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IController
{
    void InitGame(PersonalFullUserInfo myFullUserInfo, MinUserInfo[] yesterdayChampions,
        MinUserInfo[] topFriends, ActiveRoomState activeRoomState);

    //they are unitasks because they are rpcs
    UniTask<FullUserInfo> GetPublicFullUserInfo(string userId);
    UniTask<object> SendAsync(string method, params object[] args);
    void ThrowCard(int cardId);
    UniTask NotifyTurnMiss();

    UniTask SelectItem(ItemType itemType, int id);
    UniTask BuyItem(ItemType itemType, int id);

    UniTask RequestRandomRoom(int betChoice, int capacityChoice);
    UniTask Surrender();

    void TstStartClient(string id);

    void AssignRpc(Action action, string moduleGroupName);
    void AssignRpc<T1>(Action<T1> action, string moduleGroupName);
    void AssignRpc<T1, T2>(Action<T1, T2> action, string moduleGroupName);
    void AssignRpc<T1, T2, T3>(Action<T1, T2, T3> action, string moduleGroupName);
    void AssignRpc<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, string moduleGroupName);

    void RemoveModuleRpcs(string moduleName);
    UniTask<MinUserInfo> TestWaitWithReturn();
}

public class ProjectRefernces
{
    public static ProjectRefernces I;

    public ProjectRefernces()
    {
        I = this;
    }

    public Transform Canvas;
}

public class Controller : MonoBehaviour, IController
{
    public static IController I { get; set; }

    private void Awake()
    {
        I = this;
    }

    public async UniTaskVoid Start()
    {
        await InitModules();

        HTTPManager.Logger = new MyBestHttpLogger();

        // #if UNITY_EDITOR
        //         UnityEngine.Object.Destroy(GameObject.Find("tst client buttons"));
        //         ConnectToServer("0").Forget();
        //         AssignGeneralRpcs();
        // #endif


#if UNITY_WEBGL
        var fbigUserData = JsonConvert.DeserializeObject<FbigUserData>(JsManager.GetUserData());

        Debug.Log(JsManager.GetUserData());
        Debug.Log(JsonConvert.SerializeObject(fbigUserData, Formatting.Indented));

        if (fbigUserData.EnteredBefore == 0)
            ConnectToServer(fbigUserData.Token, fbigUserData.Name, fbigUserData.PictureUrl);
        else
            ConnectToServer(fbigUserData.Token);

        Repository.I.TopFriends = JsonConvert.DeserializeObject<List<FbigFriend>>(JsManager.GetFriends())
        .Select(f => new MinUserInfo { Id = f.Id, Name = f.Name, PictureUrl = f.PictureUrl })
        .ToArray();

        Debug.Log(JsManager.GetFriends());
        Debug.Log(JsonConvert.SerializeObject(Repository.I.TopFriends, Formatting.Indented));

#endif

    }

    private async UniTask InitModules()
    {
        new ProjectRefernces();

        ProjectRefernces.I.Canvas = (await Addressables.InstantiateAsync("canvas"))
            .GetComponent<Transform>();
        ProjectRefernces.I.Canvas.GetComponent<Canvas>().sortingOrder = 10;

        new Repository();
        await BlockingPanel.Create();
        new BlockingOperationManager();
        await Background.Create();
        await Toast.Create();
    }

    public void TstStartClient(string id)
    {
        ConnectToServer(id, demo: true);

        AssignGeneralRpcs();
    }

    public void InitGame(PersonalFullUserInfo myFullUserInfo, MinUserInfo[] yesterdayChampions,
        MinUserInfo[] topFriends, ActiveRoomState activeRoomState)
    {
        Repository.I.PersonalFullInfo = myFullUserInfo;
        Repository.I.YesterdayChampions = yesterdayChampions;
        // Repository.I.TopFriends = topFriends;

        Repository.I.PersonalFullInfo.DecreaseMoneyAimTimeLeft().Forget();

        LoadAppropriateModules(activeRoomState);
    }

    private void LoadAppropriateModules(ActiveRoomState activeRoomState)
    {
        if (activeRoomState == null)
        {
            new LobbyController();
        }
        else
        {
            new RoomSettings(activeRoomState);
            RoomController.Create(activeRoomState).Forget();
        }
    }

    #region rpc works

    private void AssignGeneralRpcs()
    {
        hubConnection.On<PersonalFullUserInfo, MinUserInfo[], MinUserInfo[], ActiveRoomState>(nameof(InitGame), InitGame);
    }

    private Dictionary<string, List<string>> RpcsNames = new Dictionary<string, List<string>>();

    private void SaveRpcName(string actionName, string moduleName)
    {
        if (RpcsNames.ContainsKey(moduleName))
            RpcsNames[moduleName].Add(actionName);
        else
            RpcsNames.Add(moduleName, new List<string> { actionName });
    }

    public void AssignRpc(Action action, string moduleGroupName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleGroupName);
    }
    public void AssignRpc<T1>(Action<T1> action, string moduleGroupName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleGroupName);
    }
    public void AssignRpc<T1, T2>(Action<T1, T2> action, string moduleGroupName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleGroupName);
    }
    public void AssignRpc<T1, T2, T3>(Action<T1, T2, T3> action, string moduleGroupName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleGroupName);
    }
    public void AssignRpc<T1, T2, T3, T4>(Action<T1, T2, T3, T4> action, string moduleGroupName)
    {
        var actionName = action.Method.Name;

        hubConnection.On(actionName, action);

        SaveRpcName(actionName, moduleGroupName);
    }

    public void RemoveModuleRpcs(string moduleName)
    {
        RpcsNames[moduleName].ForEach(_ => hubConnection.Remove(_));
    }

    #endregion

    #region hub

    private HubConnection hubConnection;
    // private readonly string address = "http://localhost:5000/connect";
    private readonly string address = "https://tstappname.azurewebsites.net/connect";
    private readonly IProtocol protocol = new JsonProtocol(new LitJsonEncoder());
    private readonly MyReconnectPolicy myReconnectPolicy = new MyReconnectPolicy();

    //I use event funtions because awaiting returns hubconn and this is useless
    private void ConnectToServer(string fbigToken, string name = null, string pictureUrl = null, bool demo = false)
    {
        Debug.Log("connecting with token " + fbigToken);

        var uriBuilder = new UriBuilder(address);
        uriBuilder.Query += $"access_token={fbigToken}";

        if (name != null)
            uriBuilder.Query += $"name={name}";
        if (pictureUrl != null)
            uriBuilder.Query += $"pictureUrl={pictureUrl}";

        if (demo)
            uriBuilder.Query += $"demo=1";


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

        hubConnection.ConnectAsync();
    }

    private bool OnMessage(HubConnection arg1, Message msg)
    {
        Debug.Log($"msg is {JsonConvert.SerializeObject(msg, Formatting.Indented)}");

        return true;
    }

    private void OnConnected(HubConnection obj)
    {

#if UNITY_WEBGL
        JsManager.StartFbigGame();
#endif


        Debug.Log("connected to server");

        Destroy(FindObjectOfType<TestClientStart>()?.gameObject);
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
        return await hubConnection.InvokeAsync<FullUserInfo>("GetUserData", userId);
    }
    public async UniTask NotifyTurnMiss()
    {
        await hubConnection.SendAsync("MissTurn");
    }
    public void ThrowCard(int cardId)
    {
        hubConnection.Send("Throw", cardId).OnError(e => throw e);
    }
    public async UniTask SelectItem(ItemType itemType, int id)
    {
        await hubConnection.SendAsync(itemType == ItemType.Cardback ? "SelectCardback" : "SelectBackground", id);
    }
    public async UniTask BuyItem(ItemType itemType, int id)
    {
        await hubConnection.SendAsync(itemType == ItemType.Cardback ? "BuyCardback" : "BuyBackground", id);
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