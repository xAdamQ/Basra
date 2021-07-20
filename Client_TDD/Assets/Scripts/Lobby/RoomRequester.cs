using UnityEngine;
using Zenject;

public class RoomRequester : MonoBehaviour
{
    [Inject] private IController _controller;
    [Inject] private IBlockingPanel _blockingPanel;
    [Inject] private BlockingOperationManager _blockingOperationManager;

    [SerializeField] private ChoiceButton capacityChoiceButton;

    public async void RequestRandomRoom(int betChoice)
    {
        if (Repository.I.PersonalFullInfo.Money < RoomSettings.Bets[betChoice])
        {
            Toast.I.Show("No enough money");
            return;
        }

        await _blockingOperationManager.Start(_controller.RequestRandomRoom(betChoice, capacityChoiceButton.CurrentChoice));

        _blockingPanel.Show("room is pending");
        //this is shown even if the room is started, it's removed before game start directly
    }
}