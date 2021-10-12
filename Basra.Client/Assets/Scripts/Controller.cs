using Basra.Common;
using BestHTTP;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using BestHTTP.SignalRCore.Messages;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
#if HMS
using HmsPlugin;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
#endif
#if UNITY_ANDROID
using Newtonsoft.Json;
#endif
#if GMS
using Facebook.Unity;
#endif

public interface IController
{
    void InitGame(PersonalFullUserInfo myFullUserInfo, ActiveRoomState activeRoomState,
        int messageIndex);

    //they are unitasks because they are rpcs
    UniTask<FullUserInfo> GetPublicFullUserInfo(string userId);
    UniTask<object> SendAsync(string method, params object[] args);
    void ThrowCard(int cardId);
    UniTask NotifyTurnMiss();

    UniTask SelectItem(ItemType itemType, int id);
    UniTask BuyItem(ItemType itemType, int id);

    UniTask RequestRandomRoom(int betChoice, int capacityChoice);

    void TstStartClient(string id);

    UniTask<T> InvokeAsync<T>(string method, params object[] args);
    void Send(string method, params object[] args);

    void ConnectToServer(string fbigToken = null, string huaweiAuthCode = null,
        string facebookAccToken = null, string name = null,
        string pictureUrl = null, bool demo = false);

    void AddRpcContainer(object container);
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

[Rpc]
public class Controller : MonoBehaviour, IController
{
    public static IController I { get; set; }

    private void Awake()
    {
        I = this;
        HTTPManager.Logger = new MyBestHttpLogger();

        AddRpcContainer(this);

        FetchRpcInfos();
    }

    [SerializeField] private GameObject adPlaceholder;

    public async UniTaskVoid Start()
    {
        await InitModules();

        SignInPanel.Create().Forget();
        LangSelector.Create();

#if GMS
        FB.Init(OnInitComplete);
#endif

#if HMS
        AdPlaceholder.SetActive(!HMSAdsKitManager.Instance.IsBannerAdLoaded);
        HMSAdsKitManager.Instance.OnBannerLoadEvent += () => AdPlaceholder.SetActive(false);
#endif

#if UNITY_WEBGL
        if (JsManager.IsFigSdkInit() == 0)
        {
            Debug.Log("seems like you're in the demo not fig");
            //TestClientStart.Create();
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


#if GMS
    private void OnInitComplete()
    {
        Debug.Log(
            $"OnInitCompleteCalled IsLoggedIn={FB.IsLoggedIn} IsInitialized={FB.IsInitialized}" +
            $" and the AccessToken.CurrentAccessToken is {AccessToken.CurrentAccessToken}");

        if (FB.IsLoggedIn)
            ConnectToServer(facebookAccToken: AccessToken.CurrentAccessToken.TokenString);
        else
            SignInPanel.Create().Forget();
    }
#endif

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

    [Rpc]
    public void InitGame(PersonalFullUserInfo myFullUserInfo, ActiveRoomState activeRoomState,
        int messageIndex)
    {
        Debug.Log("InitGame is being called");

        Repository.I.PersonalFullInfo = myFullUserInfo;

        Repository.I.PersonalFullInfo.DecreaseMoneyAimTimeLeft().Forget();

        this.messageIndex = messageIndex;

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
        return await InvokeAsync<bool>("IsFollowing", targetId);
    }

    #region hub

    private HubConnection hubConnection;

    [SerializeField] private int selectedAddress;
    [SerializeField] private string[] addresses;

    private string getAddress()
    {
        return JsManager.BackendAddress() ?? addresses[selectedAddress] + "/connect";
    }

    private readonly IProtocol protocol = new JsonProtocol(new LitJsonEncoder());
    private readonly MyReconnectPolicy myReconnectPolicy = new MyReconnectPolicy();

    //I use event functions because awaiting returns hubconn and this is useless
    public void ConnectToServer(string fbigToken = null, string huaweiAuthCode = null,
        string facebookAccToken = null, string name = null, string pictureUrl = null,
        bool demo = false)
    {
        Debug.Log("connecting to server");

        var query = HttpUtility.ParseQueryString(string.Empty);

        if (facebookAccToken != null)
            query["fb_access_token"] = facebookAccToken;

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


        var uriBuilder = new UriBuilder(getAddress())
        {
            Query = query.ToString()
        };

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

        // AssignGeneralRpcs();

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
        if (msg.type == MessageTypes.Invocation)
        {
#if !UNITY_WEBGL
            Debug.Log(
                $"msg is {msg.target} {JsonConvert.SerializeObject(msg, Formatting.Indented)}");
#else
            Debug.Log($"msg is {JsonUtility.ToJson(msg)}");
#endif
            HandleInvocationMessage(msg).Forget();
            return false;
        }

        return true;
    }

    private void OnConnected(HubConnection obj)
    {
        Debug.Log("connected to server");


        SignInPanel.DestroyModule();

        LangSelector.DestroyModule();

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

    [ContextMenu("restart")]
    public void RestartGame()
    {
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

            await SceneManager.LoadSceneAsync(0);
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

    #endregion

    #region rpc with reflections

    private readonly Dictionary<string, (MethodInfo info, Type[] types)> rpcInfos =
        new Dictionary<string, (MethodInfo info, Type[] types)>();

    private void FetchRpcInfos()
    {
        //you need instance of each object of the fun when server calls
        //get the type of each one and pass the right object

        var namespaceTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && t.GetCustomAttribute<RpcAttribute>() != null);

        foreach (var type in namespaceTypes)
        {
            foreach (var method in type.GetMethods())
            {
                var attr = method.GetCustomAttribute<RpcAttribute>();
                if (attr == null) continue;
                rpcInfos.Add(attr.RpcName ?? method.Name, (method, method.GetParameterTypes()));
            }
        }
    }

    private readonly Dictionary<Type, object> rpcContainers = new Dictionary<Type, object>();

    public void AddRpcContainer(object container)
    {
        var t = container.GetType();

        if (rpcContainers.ContainsKey(t))
            rpcContainers[t] = container;
        else
            rpcContainers.Add(t, container);
    }

    private readonly List<Message> pendingInvocations = new List<Message>();

    private int messageIndex;

    private bool rpcCalling;

    private async UniTaskVoid HandleInvocationMessage(Message message)
    {
        if (message.target != nameof(InitGame)
            && (int)message.arguments[0] != messageIndex)
        {
            pendingInvocations.Add(message);
            return;
        }

        await UniTask.WaitUntil(() => !rpcCalling);

        rpcCalling = true;

        var method = rpcInfos[message.target];

        var realArgs = hubConnection.Protocol.GetRealArguments(method.types,
            message.arguments.Skip(1).ToArray());

        var container = method.info.IsStatic ? null : rpcContainers[method.info.DeclaringType!];

        if (method.info.ReturnType == typeof(UniTask))
            await method.info.InvokeAsync(container, realArgs);
        else
            method.info.Invoke(container, realArgs);

        messageIndex++;

        rpcCalling = false;

        if (pendingInvocations.Any(m => (int)m.arguments[0] == messageIndex))
            HandleInvocationMessage(pendingInvocations
                    .First(m => (int)m.arguments[0] == messageIndex))
                .Forget();
    }

    #endregion
}