using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ILobbyController
{
    void PrepareRequestedRoomRpc(int betChoice, int capacityChoice, List<FullUserInfo> userInfos, int myTurn);
}

public class LobbyController : ILobbyController, IInitializable, System.IDisposable
{
    [Inject] private readonly IController _controller;
    [Inject] private readonly IRepository _repository;
    [Inject] private readonly RoomController.Factory _roomFactory;

    public void Initialize()
    {
        AssignRpcs();
    }

    public void Dispose()
    {
        _controller.RemoveModuleRpcs(nameof(LobbyController));
    }

    private void AssignRpcs()
    {
        _controller.AssignRpc<int, int, List<FullUserInfo>, int>(PrepareRequestedRoomRpc,
            nameof(LobbyController));
    }

    public class Factory : PlaceholderFactory<LobbyController>
    {
        public static Factory I;

        public Factory()
        {
            I = this;
        }
    }

    public void PrepareRequestedRoomRpc(int betChoice, int capacityChoice, List<FullUserInfo> userInfos, int myTurn)
    {
        DestroyLobby();

        var roomsSettings = new RoomSettings(betChoice, capacityChoice, userInfos, myTurn);

        _repository.PersonalFullInfo.Money -= roomsSettings.BetMoneyToPay();

        _roomFactory.Create(roomsSettings, null);
    }

    private void DestroyLobby()
    {
        //todo find better way to locate it
        Object.Destroy(Object.FindObjectOfType<LobbyInstaller>().gameObject);
    }
}