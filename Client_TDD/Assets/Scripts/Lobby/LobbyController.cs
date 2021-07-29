using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface ILobbyController
{
    void PrepareRequestedRoomRpc(int betChoice, int capacityChoice, List<FullUserInfo> userInfos, int myTurn);
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

public class LobbyController : ILobbyController
{
    public LobbyController()
    {
        Initialize().Forget();
    }

    public async UniTaskVoid Initialize()
    {
        await UniTask.DelayFrame(1);

        var containerRoot = new GameObject("Lobby").transform;

        new LobbyReferences();

        LobbyReferences.I.Canvas = (await Addressables.InstantiateAsync("canvas", containerRoot))
            .GetComponent<Transform>();

        await FriendsView.Create();

        await PersonalActiveUserView.Create();

        await RoomRequester.Create();

        await Shop.Create(LobbyReferences.I.Canvas, ItemType.Cardback);

        await Shop.Create(LobbyReferences.I.Canvas, ItemType.Background);

        AssignRpcs();
    }


    private void AssignRpcs()
    {
        Controller.I.AssignRpc<int, int, List<FullUserInfo>, int>(PrepareRequestedRoomRpc,
            nameof(LobbyController));
    }

    public void PrepareRequestedRoomRpc(int betChoice, int capacityChoice, List<FullUserInfo> userInfos, int myTurn)
    {
        DestroyLobby();

        new RoomSettings(betChoice, capacityChoice, userInfos, myTurn);

        RoomController.Create(null).Forget();
    }

    public event Action Destroyed;

    private void DestroyLobby()
    {
        LobbyReferences.I = null;

        Controller.I.RemoveModuleRpcs(GetType().ToString());
        UnityEngine.Object.Destroy(GameObject.Find("Lobby"));

        //todo find better way to locate it
        // Object.Destroy(Object.FindObjectOfType<LobbyInstaller>().gameObject);

        Destroyed?.Invoke();
    }
}