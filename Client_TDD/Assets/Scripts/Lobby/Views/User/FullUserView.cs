using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class FullUserView : MinUserView
{
    [SerializeField] private Text playedRoomsText;
    [SerializeField] private Text wonRoomsText;
    [SerializeField] private Text eatenCardsText;
    [SerializeField] private Text winStreakText;
    [SerializeField] private Text basrasText;
    [SerializeField] private Text bigBasrasText;
    [SerializeField] private Text winRatioText;

    public void Show(FullUserInfo fullUserInfo)
    {
        Init(fullUserInfo);

        playedRoomsText.text = fullUserInfo.PlayedRoomsCount.ToString();
        wonRoomsText.text = fullUserInfo.WonRoomsCount.ToString();
        eatenCardsText.text = fullUserInfo.EatenCardsCount.ToString();
        winStreakText.text = fullUserInfo.WinStreak.ToString();
        basrasText.text = fullUserInfo.BasrasCount.ToString();
        bigBasrasText.text = fullUserInfo.BigBasrasCount.ToString();
        winRatioText.text = fullUserInfo.WinRatio.ToString("p2");

        gameObject.SetActive(true);
    }
}