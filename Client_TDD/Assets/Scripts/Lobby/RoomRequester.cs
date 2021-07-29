using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RoomRequester : MonoBehaviour
{
    public static async UniTask Create()
    {
        await Addressables.InstantiateAsync("roomRequester", LobbyReferences.I.Canvas);
    }

    [SerializeField] private ChoiceButton capacityChoiceButton;
    [SerializeField] private TMP_Text betText, ticketText;

    private void Awake()
    {
        var bet = RoomSettings.Bets[transform.GetSiblingIndex()];

        betText.text = bet.ToString();
        ticketText.text = (bet * .1f).ToString();
    }

    public async void RequestRandomRoom(int betChoice)
    {
        if (Repository.I.PersonalFullInfo.Money < RoomSettings.Bets[betChoice])
        {
            Toast.I.Show("No enough money");
            return;
        }

        await BlockingOperationManager.I.Start(Controller.I.RequestRandomRoom(betChoice, capacityChoiceButton.CurrentChoice));

        BlockingPanel.I.Show("room is pending");
        //this is shown even if the room is started, it's removed before game start directly
    }
}