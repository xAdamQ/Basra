using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

public class FullUserView : MinUserView
{
    [SerializeField] private TMP_Text
        moneyText,
        playedRoomsText,
        wonRoomsText,
        eatenCardsText,
        winStreakText,
        maxWinStreakText,
        basrasText,
        bigBasrasText,
        winRatioText,
        totalEarnedMoney;

    [SerializeField] private GameObject addFriendButton;

    private static FullUserView ActiveInstance;

    public static void Show(FullUserInfo fullUserInfo)
    {
        UniTask.Create(async () =>
        {
            if (!ActiveInstance) ActiveInstance = await Create();

            ActiveInstance.SetData(fullUserInfo);
        });
    }

    private static async UniTask<FullUserView> Create()
    {
        return (await Addressables.InstantiateAsync("fullUserView", ProjectRefernces.I.Canvas))
            .GetComponent<FullUserView>();
    }

    private void SetData(FullUserInfo fullUserInfo)
    {
        if (fullUserInfo is PersonalFullUserInfo) addFriendButton.SetActive(false);

        base.Init(fullUserInfo);

        moneyText.text = fullUserInfo.Money.ToString();
        playedRoomsText.text = fullUserInfo.PlayedRoomsCount.ToString();
        wonRoomsText.text = fullUserInfo.WonRoomsCount.ToString();
        eatenCardsText.text = fullUserInfo.EatenCardsCount.ToString();
        winStreakText.text = fullUserInfo.WinStreak.ToString();
        maxWinStreakText.text = fullUserInfo.MaxWinStreak.ToString();
        basrasText.text = fullUserInfo.BasraCount.ToString();
        bigBasrasText.text = fullUserInfo.BigBasraCount.ToString();
        //winRatioText.text = ((float)fullUserInfo.WonRoomsCount / fullUserInfo.PlayedRoomsCount).ToString("p2");
        winRatioText.text = fullUserInfo.WinRatio.ToString("p2");
        totalEarnedMoney.text = fullUserInfo.TotalEarnedMoney.ToString();

        gameObject.SetActive(true);
    }

    public void Destroy()
    {
        Object.Destroy(gameObject);
    }
}