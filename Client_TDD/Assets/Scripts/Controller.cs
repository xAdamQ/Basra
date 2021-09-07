using Basra.Common;
using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore.Messages;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Web;

#if UNITY_ANDROID && !UNITY_EDITOR
using HmsPlugin;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
#endif

using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public interface IController
{
    void InitGame(PersonalFullUserInfo myFullUserInfo, ActiveRoomState activeRoomState);

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
    UniTask<T> InvokeAsync<T>(string method, params object[] args);
    UniTask<T> InvokeAsync<T>(string method);
    void Send(string method, params object[] args);
    event Action OnAppPause;
}

public class ProjectReferences
{
    public static ProjectReferences I;

    public ProjectReferences()
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
        HTTPManager.Logger = new MyBestHttpLogger();
    }

    [ContextMenu("pause")]
    public void pauseTest()
    {
        OnApplicationPause(true);
    }
    [ContextMenu("resume")]
    public void resumeTest()
    {
        OnApplicationPause(false);
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        Debug.Log("app pause: " + pauseStatus);

        if (pauseStatus) //paused
        {
            // UniTask.Create(async () =>
            // {
            //     await SceneManager.UnloadSceneAsync(0);
            // });

            // OnAppPause?.Invoke(); //module groups register themselves to die here
            // hubConnection.CloseAsync();
            // hubConnection?.StartClose();
        }
        else //returned
        {
            if (RoomController.I == null) return;
            //you restart on the room only

            // if (HMSAccountManager.Instance.SigningIn)
            // return;

            RestartGame();
        }
    }

    public event Action OnAppPause;

    [SerializeField] private GameObject AdPlaceholder;

    public async UniTaskVoid Start()
    {
        await InitModules();

#if UNITY_ANDROID && !UNITY_EDITOR
        AdPlaceholder.SetActive(!HMSAdsKitManager.Instance.IsBannerAdLoaded);
        HMSAdsKitManager.Instance.OnBannerLoadEvent += () => AdPlaceholder.SetActive(false);

        HMSAccountManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountManager.Instance.OnSignInFailed = OnLoginFailure;
        SignInPanel.Create();
#endif

#if UNITY_EDITOR
        TestClientStart.Create();
#endif

#if UNITY_WEBGL && !UNITY_EDITOR
        if (JsManager.IsFigSdkInit() == 0)
        {
            Debug.Log("seems like you're in the demo not fig");
            TestClientStart.Create();
            return;
        }

        Debug.Log("user data: " + JsManager.GetUserData());
        var fbigUserData = JsonUtility.FromJson<FbigUserData>(JsManager.GetUserData());
        Debug.Log("user data loaeds: " + JsonUtility.ToJson(fbigUserData));

        if (fbigUserData.EnteredBefore == 0)
            ConnectToServer(fbigToken: fbigUserData.Token, fbigUserData.Name, fbigUserData.PictureUrl);
        else
            ConnectToServer(fbigToken: fbigUserData.Token);

        Repository.I.TopFriends = JsonUtility.FromJson<List<FbigFriend>>(JsManager.GetFriends())
        .Select(f => new MinUserInfo { Id = f.Id, Name = f.Name, PictureUrl = f.PictureUrl })
        .ToArray();

        Debug.Log("friends are: " + JsManager.GetFriends());
        Debug.Log("friends loaded: " + JsonUtility.ToJson(Repository.I.TopFriends));

        JsManager.StartFbigGame();
        //you can think it would make more sence to start when conntected, but there could be network issue and require reconnect for example
        //the decision is not final anyway
#endif
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnLoginFailure(HMSException exc)
    {
        Debug.Log("failed huawei login: " + exc);
    }

    private void OnLoginSuccess(AuthAccount authAccount)
    {
        ConnectToServer(huaweiAuthCode: authAccount.AuthorizationCode);
        // ConnectToServer(fbigToken: "123", name: "some_guest", demo: true);
    }
#endif

    public void LevelUp(int newLevel, int moneyReward)
    {
        LevelUpPanel.Create(newLevel, moneyReward).Forget();
        // Repository.I.PersonalFullInfo.Money += moneyReward;
        //both are added because the whole personal info object is updated 
    }


    private async UniTask InitModules()
    {
        new ProjectReferences();

        ProjectReferences.I.Canvas = (await Addressables.InstantiateAsync("canvas"))
            .GetComponent<Transform>();
        ProjectReferences.I.Canvas.GetComponent<Canvas>().sortingOrder = 10;

        new Repository();
        new BlockingOperationManager();
        await Background.Create();
        await Toast.Create();
    }

    public void TstStartClient(string id)
    {
        ConnectToServer(fbigToken: id, name: "guest", demo: true);
    }

    public void InitGame(PersonalFullUserInfo myFullUserInfo, ActiveRoomState activeRoomState)
    {
        Debug.Log("InitGame is being called");

        Repository.I.PersonalFullInfo = myFullUserInfo;

        Repository.I.PersonalFullInfo.DecreaseMoneyAimTimeLeft().Forget();

        LoadAppropriateModules(activeRoomState);
    }

    private void LoadAppropriateModules(ActiveRoomState activeRoomState)
    {
        if (activeRoomState == null)
        {
            new LobbyController();
            Debug.Log("attempt to create and forget Lobby");
        }
        else
        {
            new RoomSettings(activeRoomState);
            RoomController.Create(activeRoomState).Forget();
        }
    }

    public void ToggleFollow(string targetId)
    {
        Controller.I.SendAsync("ToggleFollow", targetId);
    }
    public async UniTask<bool> IsFollowing(string targetId)
    {
        return await Controller.I.InvokeAsync<bool>("IsFollowing", targetId);
    }

    #region rpc works

    private void AssignGeneralRpcs()
    {
        hubConnection.On<PersonalFullUserInfo, ActiveRoomState>(nameof(InitGame), InitGame);
        hubConnection.On<int, int>(nameof(LevelUp), LevelUp);
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

    [SerializeField] private int selctedAddress;
    [SerializeField] private string[] addresses;
    private string address => addresses[selctedAddress] + "/connect";

    private readonly IProtocol protocol = new JsonProtocol(new LitJsonEncoder());
    private readonly MyReconnectPolicy myReconnectPolicy = new MyReconnectPolicy();

    //I use event funtions because awaiting returns hubconn and this is useless
    private void ConnectToServer(string fbigToken = null, string huaweiAuthCode = null,
        string name = null, string pictureUrl = null,
        bool demo = false)
    {
        Debug.Log("connecting with token " + fbigToken);

        #region query with strings only

        // var query = "https://tstappname.azurewebsites.net/connect?";

        // query += $"access_token={fbigToken}";

        // if (name != null)
        // query += $"&name={name}";
        // if (pictureUrl != null)
        //     query += $"&pictureUrl={pictureUrl}";
        // if (demo)
        //     query += $"&demo=1  ";

        // var uriBuilder = new UriBuilder(address)
        // {
        //     Query = query,
        // };

        #endregion

        var query = HttpUtility.ParseQueryString(string.Empty);

        if (fbigToken != null)
            query["access_token"] = fbigToken;

        if (huaweiAuthCode != null)
            query["huaweiAuthCode"] = huaweiAuthCode;

        if (name != null)
            query["name"] = name;
        if (pictureUrl != null)
            query["pictureUrl"] = pictureUrl;

        if (demo)
            query["demo"] = "1";


        var uriBuilder = new UriBuilder(address);

        uriBuilder.Query = query.ToString();

        Debug.Log($"connecting with url {uriBuilder.ToString()}");

        var hubOptions = new HubOptions()
        {
            SkipNegotiation = true,
            PreferedTransport = TransportTypes.WebSocket,
        };

        hubConnection = new HubConnection(uriBuilder.Uri, protocol, hubOptions)
        {
            ReconnectPolicy = myReconnectPolicy,
        };

        AssignGeneralRpcs();

        //I don't have this term "authentication" despite I make token authentication
        // HubConnection.AuthenticationProvider = new DefaultAccessTokenAuthenticator(HubConnection);

        hubConnection.OnConnected += OnConnected;
        hubConnection.OnError += OnError;
        hubConnection.OnClosed += OnClosed;
        hubConnection.OnMessage += OnMessage;
        hubConnection.OnReconnecting += OnReconnecting;

        BlockingOperationManager.I.Forget(hubConnection.ConnectAsync().AsUniTask());
    }


    private bool OnMessage(HubConnection arg1, Message msg)
    {
#if !UNITY_WEBGL
        Debug.Log($"msg is {JsonConvert.SerializeObject(msg, Formatting.Indented)}");
#else
        Debug.Log($"msg is {JsonUtility.ToJson(msg)}");
#endif
        return true;
    }

    private void OnConnected(HubConnection obj)
    {
        Debug.Log("connected to server");

        SignInPanel.Destroy();

        Destroy(FindObjectOfType<TestClientStart>()?.gameObject);
    }
    private void OnClosed(HubConnection obj)
    {
        //don't restart game here because this is called only when the connection
        //is gracefully closed
        Debug.Log("OnClosed");
    }
    private void OnError(HubConnection arg1, string arg2)
    {
        RestartGame();
        Debug.Log($"OnError: {arg2}");
    }
    private void OnReconnecting(HubConnection arg1, string arg2)
    {
        Debug.Log("reconnecting");
    }

    private void RestartGame()
    {
        Debug.Log("restarting game");

        UniTask.Create(async () =>
        {
            try
            {
                await hubConnection.CloseAsync();
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            SceneManager.LoadSceneAsync(0);
        }).Forget(e => throw e);
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
        await hubConnection.SendAsync(
            itemType == ItemType.Cardback ? "SelectCardback" : "SelectBackground", id);
    }
    public async UniTask BuyItem(ItemType itemType, int id)
    {
        await hubConnection.SendAsync(
            itemType == ItemType.Cardback ? "BuyCardback" : "BuyBackground", id);
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
    public void Send(string method, params object[] args)
    {
        hubConnection.Send(method, args);
    }

    public async UniTask<T> InvokeAsync<T>(string method, params object[] args)
    {
        return await hubConnection.InvokeAsync<T>(method, args);
    }
    public async UniTask<T> InvokeAsync<T>(string method)
    {
        return await hubConnection.InvokeAsync<T>(method);
    }

    public async UniTask<MinUserInfo> TestWaitWithReturn()
    {
        return await hubConnection.InvokeAsync<MinUserInfo>("TestReturnObject");
    }
}