using System;
using UnityEngine;
using Zenject;

public class RoomRequester : MonoBehaviour
{
    [Inject] private IController _controller;
    [Inject] private IBlockingPanel _blockingPanel;
    [Inject] private BlockingOperationManager _blockingOperationManager;

    public Tuple<int, int> LastChoice;

    public async void RequestRandomRoom(int betChoice)
    {
        LastChoice = new Tuple<int, int>(betChoice, 0); //even if the result is an error!

        await _blockingOperationManager.Start(_controller.RequestRandomRoom(betChoice, 0));

        _blockingPanel.Show("room is pending");
        //this is shown even if the room is started, it's removed before game start directly
    }
}