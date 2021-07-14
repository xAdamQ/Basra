using TMPro;
using UnityEngine;

public class UserRoomStatusView : MonoBehaviour
{
    [SerializeField]
    private TMP_Text
        eatenCardsText,
        basrasText,
        bigBasrasText,
        winMoneyText;

    public void Init(UserRoomStatus oppoRoomResult)
    {
        eatenCardsText.text = oppoRoomResult.EatenCards.ToString();
        basrasText.text = oppoRoomResult.Basras.ToString();
        bigBasrasText.text = oppoRoomResult.BigBasras.ToString();
        winMoneyText.text = oppoRoomResult.WinMoney.ToString();
    }
}
