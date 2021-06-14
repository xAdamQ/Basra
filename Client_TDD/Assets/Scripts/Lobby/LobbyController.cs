using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ILobbyController
{
    void PrepareRequestedRoomRpc(List<RoomOppoInfo> roomOppoInfo, int myTurn);
}

public class LobbyController : ILobbyController, IInitializable
{
    private readonly IController _controller;
    private readonly RoomController.Factory _roomFactory;
    private readonly RoomRequester _roomRequester;

    [Inject]
    public LobbyController(IController controller, RoomController.Factory roomFactory,
        RoomRequester roomRequester)
    {
        _controller = controller;
        _roomFactory = roomFactory;
        _roomRequester = roomRequester;
    }

    public void Initialize()
    {
        _controller.AddLobbyRpcs(this);
    }

    public class Factory : PlaceholderFactory<LobbyController>
    {
    }

    public void PrepareRequestedRoomRpc(List<RoomOppoInfo> roomOppoInfo, int myTurn)
    {
        DestroyLobby();

        var roomChoice = _roomRequester.LastChoice;

        _roomFactory.Create(new RoomSettings(roomChoice.Item1, roomChoice.Item2, roomOppoInfo, myTurn));
    }

    private void DestroyLobby()
    {
        _controller.RemoveLobbyRpcs();

        //todo find better way to locate it
        Object.Destroy(Object.FindObjectOfType<LobbyInstaller>().gameObject);
    }
}