using UnityEngine;
using Zenject;

public class RoomMenu : MonoBehaviour
{
    [Inject] private readonly IRoomController _roomController;
    [Inject] private readonly IController _controller;
    [Inject] private readonly LobbyController.Factory _lobbyFactory;
    [Inject] private readonly BlockingOperationManager _blockingOperationManager;

    public async void Surrender()
    {
        await _blockingOperationManager.Start(_controller.Surrender());

        _roomController.DestroyModule();
        _lobbyFactory.Create();
    }
}