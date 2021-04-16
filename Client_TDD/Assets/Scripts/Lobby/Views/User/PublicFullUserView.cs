using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PublicFullUserView : PublicMinUserView
{
    [SerializeField] private Text playedRoomsText;
    [SerializeField] private Text wonRoomsText;
    [SerializeField] private Text eatenCardsText;
    [SerializeField] private Text winStreakText;
    [SerializeField] private Text basrasText;
    [SerializeField] private Text bigBasrasText;
    [SerializeField] private Text winRatioText;

    public void Show(PublicFullUserInfo publicFullUserInfo)
    {
        Init(publicFullUserInfo);

        playedRoomsText.text = publicFullUserInfo.PlayedRoomsCount.ToString();
        wonRoomsText.text = publicFullUserInfo.WonRoomsCount.ToString();
        eatenCardsText.text = publicFullUserInfo.EatenCardsCount.ToString();
        winStreakText.text = publicFullUserInfo.WinStreak.ToString();
        basrasText.text = publicFullUserInfo.BasrasCount.ToString();
        bigBasrasText.text = publicFullUserInfo.BigBasrasCount.ToString();
        winRatioText.text = publicFullUserInfo.WinRatio.ToString("p2");

        gameObject.SetActive(true);
    }
}