using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface ILobbyController
{
    void PrepareRequestedRoomRpc(int betChoice, int capacityChoice, List<FullUserInfo> userInfos,
        int myTurn);
    event System.Action Destroyed;
}

public class LobbyReferences
{
    public static LobbyReferences I;

    public LobbyReferences()
    {
        I = this;
    }

    public Transform Canvas;
}

[Rpc]
public class LobbyController : ILobbyController
{
    public static LobbyController I;

    public LobbyController()
    {
        I = this;

        Initialize().Forget();
    }

    private async UniTaskVoid Initialize()
    {
        Controller.I.AddRpcContainer(this);
        // Controller.I.OnAppPause += DestroyLobby;

        await UniTask.DelayFrame(1);

        var containerRoot = new GameObject("Lobby").transform;

        new LobbyReferences();

        LobbyReferences.I.Canvas = (await Addressables.InstantiateAsync("canvas", containerRoot))
            .GetComponent<Transform>();

        await FriendsView.Create();

        SoundButton.Create();

        await PersonalActiveUserView.Create();

        await RoomRequester.Create();

        await Shop.Create(LobbyReferences.I.Canvas, ItemType.Cardback);

        await Shop.Create(LobbyReferences.I.Canvas, ItemType.Background);

        IapShop.Create();

        Background.I.SetForLobby();

        // AssignRpcs();
    }

    // private void AssignRpcs()
    // {
    //     Controller.I.AssignRpc<int, int, List<FullUserInfo>, int>(PrepareRequestedRoomRpc,
    //         nameof(LobbyController));
    //
    //     Controller.I.AssignRpc<MinUserInfo>(ChallengeRequest,
    //         nameof(LobbyController));
    //
    //     Controller.I.AssignRpc<bool>(RespondChallenge,
    //         nameof(LobbyController));
    //     
    //     Controller.I.AssignRpc<int>(AddMoney,
    //         nameof(LobbyController));
    // }

    [Rpc]
    public void AddMoney(int amount)
    {
        AddMoneyPopup.Show(amount)
            .Forget(e => throw e);
    }
    [Rpc]
    public void PrepareRequestedRoomRpc(int betChoice, int capacityChoice,
        List<FullUserInfo> userInfos, int myTurn)
    {
        DestroyLobby();

        new RoomSettings(betChoice, capacityChoice, userInfos, myTurn);

        RoomController.Create(null).Forget();
    }

    [Rpc]
    public void ChallengeRequest(MinUserInfo senderInfo)
    {
        ChallengeResponsePanel.Show(senderInfo);
    }

    [Rpc]
    public void RespondChallenge(bool response)
    {
        if (!response)
        {
            BlockingPanel.Hide();
            Toast.I.Show(Translatable.GetText("player_rejected"));
        }
        else
        {
            BlockingPanel.HideDismiss();
            //panel is removed at start
            Toast.I.Show(Translatable.GetText("creating_room"));
        }
    }

    public event Action Destroyed;

    private void DestroyLobby()
    {
        // Controller.I.OnAppPause -= DestroyLobby;

        LobbyReferences.I = null;

        // Controller.I.RemoveModuleRpcs(GetType().ToString());
        UnityEngine.Object.Destroy(GameObject.Find("Lobby"));

        I = null;

        Destroyed?.Invoke();
    }
}