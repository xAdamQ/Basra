using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using Basra.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IRoomController
{
    void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds); //trivial to test
    void DestroyModuleGroup();
    void FinalizeRoom(FinalizeResult finalizeResult);
    event System.Action Destroyed;
}


[Rpc]
public class RoomController : IRoomController
{
    public event System.Action Destroyed;

    public static IRoomController I;

    private RoomController()
    {
        I = this;
        Controller.I.AddRpcContainer(this);
    }

    // public RoomController(ActiveRoomState activeRoomState) : this()
    // {
    //     new RoomSettings(activeRoomState.BetChoice, activeRoomState.CapacityChoice, activeRoomState.UserInfos,
    //         activeRoomState.MyTurnId);
    //
    //     Initialize(activeRoomState).Forget();
    // }
    // public RoomController(int betChoice, int capacityChoice, List<FullUserInfo> userInfos, int myTurn) : this()
    // {
    //     new RoomSettings(betChoice, capacityChoice, userInfos, myTurn);
    //
    //     Initialize(null).Forget();
    // }

    public static async UniTaskVoid Create(ActiveRoomState activeRoomState)
    {
        var roomController = new RoomController();
        await roomController.Initialize(activeRoomState);
    }

    //A. this is registering the services
    //meaning all I is resolved

    //Q. can we pass params to services?

    //B.
    //the initialization issue, this should be after injection, means after awake
    private async UniTask Initialize(ActiveRoomState activeRoomState)
    {
        // Controller.I.OnAppPause += DestroyModuleGroup;

        var containerRoot = new GameObject("Room").transform;

        new RoomReferences();
        RoomReferences.I.Canvas = (await Addressables.InstantiateAsync("canvas", containerRoot))
            .GetComponent<Transform>();
        RoomReferences.I.Root = containerRoot;

        PrizeView.Create();

        await ChatSystem.Create();

        await Ground.Create();

        new CoreGameplay();

        new RoomUserView.Manager();

        //dependent on RoomSettings
        //this will make registering requires order, so no circular dependencies possible

        RoomUserView.Manager.I.Init();

        await CoreGameplay.I.CreatePlayers();

        Repository.I.PersonalFullInfo.Money -= RoomSettings.I.BetMoneyToPay();

        Background.I.SetForRoom(RoomSettings.I.UserInfos);

        // AssignRpcs();

        if (activeRoomState == null)
            Controller.I.SendAsync("Ready").Forget(e => throw e);
        else
        {
            //todo why this
            // await UniTask.DelayFrame(1);

            CoreGameplay.I.ResumeGame(activeRoomState.MyHand, activeRoomState.Ground,
                activeRoomState.HandCounts,
                activeRoomState.CurrentTurn);
        }
    }

    // private void AssignRpcs()
    // {
    //     var moduleGroupName = nameof(RoomController);
    //
    //     Controller.I.AssignRpc<List<int>, List<int>>(StartRoomRpc, moduleGroupName);
    //     Controller.I.AssignRpc<FinalizeResult>(FinalizeRoom, moduleGroupName);
    // }

    [Rpc]
    public void FinalizeRoom(FinalizeResult finalizeResult)
    {
        UniTask.Create(async () =>
        {
            //wait for the last throw operation, this can be done better
            await UniTask.Delay(1200);

            await CoreGameplay.I.EatLast(finalizeResult.LastEaterTurnId);

            // FinalizeController.I.

            // DestroyModuleGroup();

            // _finalizeFactory.Create(finalizeResult, _roomSettings);

            RoomUserView.Manager.I.RoomUserViews.ForEach(ruv => Object.Destroy(ruv.gameObject));
            Object.FindObjectsOfType<PlayerBase>().ForEach(obj => Object.Destroy(obj.gameObject));
            Object.Destroy(Object.FindObjectOfType<ChatSystem>().gameObject);
            Object.Destroy(PrizeView.I.gameObject);

            //immmm this will cause issues on the running funs like decreaseMoneyAimTime and events
            //change indie values instead of rewrite the whole object
            finalizeResult.PersonalFullUserInfo.Followers =
                Repository.I.PersonalFullInfo.Followers;
            finalizeResult.PersonalFullUserInfo.Followings =
                Repository.I.PersonalFullInfo.Followings;

            Repository.I.PersonalFullInfo = finalizeResult.PersonalFullUserInfo;
            Repository.I.PersonalFullInfo.DecreaseMoneyAimTimeLeft().Forget();
            //todo you MUST edit each value on it's own now?, this is about replacing the whole
            //data object, but it seems fine


            FinalizeController.Construct(RoomReferences.I.Canvas, RoomSettings.I, finalizeResult)
                .Forget();
        });
    }

    [Rpc]
    public void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds)
    {
        CoreGameplay.I.BeginGame(handCardIds, groundCardIds);

        BlockingPanel.Hide();
    }

    public void DestroyModuleGroup()
    {
        // Controller.I.OnAppPause -= DestroyModuleGroup;

        //killing non mb
        RoomReferences.I = null;
        CoreGameplay.I = null;
        RoomUserView.Manager.I = null;
        RoomSettings.I = null;

        //killing mb
        Object.Destroy(GameObject.Find("Room"));

        // Controller.I.RemoveModuleRpcs(GetType().ToString());

        I = null;

        Destroyed?.Invoke();
    }
}