using TMPro;
using UnityEngine;

public class FullUserView : MinUserView
{
    [SerializeField]
    private TMP_Text
        moneyText,
        playedRoomsText,
        wonRoomsText,
        eatenCardsText,
        winStreakText,
        basrasText,
        bigBasrasText,
        winRatioText;

    [SerializeField] private GameObject addFriendButton;

    public void Show(FullUserInfo fullUserInfo)
    {
        if (fullUserInfo is PersonalFullUserInfo) addFriendButton.SetActive(false);

        Init(fullUserInfo);

        moneyText.text = fullUserInfo.Money.ToString();
        playedRoomsText.text = fullUserInfo.PlayedRoomsCount.ToString();
        wonRoomsText.text = fullUserInfo.WonRoomsCount.ToString();
        eatenCardsText.text = fullUserInfo.EatenCardsCount.ToString();
        winStreakText.text = fullUserInfo.WinStreak.ToString();
        basrasText.text = fullUserInfo.BasraCount.ToString();
        bigBasrasText.text = fullUserInfo.BigBasraCount.ToString();
        //winRatioText.text = ((float)fullUserInfo.WonRoomsCount / fullUserInfo.PlayedRoomsCount).ToString("p2");
        winRatioText.text = fullUserInfo.WinRatio.ToString("p2");


        gameObject.SetActive(true);
    }
}