using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Object = UnityEngine.Object;

public class FullUserView : MinUserView
{
    public enum MatchRequestResult
    {
        Offline,
        Playing,
        NoMoney,
        Available,
    }

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
        totalEarnedMoney,
        followingBackText,
        followButtonText,
        openMatchesText;

    [SerializeField] private GameObject challengeButton;

    private static FullUserView activeInstance;
    public FullUserInfo FullUserInfo;

    [SerializeField] private Image openMatchesCheck;

    public static void Show(FullUserInfo fullUserInfo)
    {
        UniTask.Create(async () =>
        {
            var key = (fullUserInfo is PersonalFullUserInfo) ? "personalFuv" : "fullUserView";

            if (!activeInstance)
            {
                activeInstance = await Create(key);
            }
            else if (activeInstance.FullUserInfo.GetType() != fullUserInfo.GetType())
            {
                activeInstance.Destroy();
                activeInstance = await Create(key);
            }

            activeInstance.Init(fullUserInfo);
        });
    }

    private static async UniTask<FullUserView> Create(string key)
    {
        return (await Addressables.InstantiateAsync(key, ProjectReferences.I.Canvas))
            .GetComponent<FullUserView>();
    }

    private void UpdateFriendShipView()
    {
        followingBackText.gameObject.SetActive(false);

        if (FullUserInfo.Friendship == (int)FriendShip.Following ||
            FullUserInfo.Friendship == (int)FriendShip.Friend)
            followingBackText.gameObject.SetActive(true);

        followButtonText.text =
            (FullUserInfo.Friendship == (int)FriendShip.Following ||
             FullUserInfo.Friendship == (int)FriendShip.None)
                ? "Follow"
                : "Unfollow";
    }

    private void Init(FullUserInfo fullUserInfo)
    {
        this.FullUserInfo = fullUserInfo;

        if (fullUserInfo is PersonalFullUserInfo)
        {
            UpdateOpenMatchesView();
        }
        else
        {
            UpdateFriendShipView();
            challengeButton.GetComponent<Button>().interactable = fullUserInfo
                .EnableOpenMatches && RoomController.I == null;
        }

        base.Init(fullUserInfo);

        moneyText.text = fullUserInfo.Money.ToString();
        playedRoomsText.text = fullUserInfo.PlayedRoomsCount.ToString();
        wonRoomsText.text = fullUserInfo.WonRoomsCount.ToString();
        eatenCardsText.text = fullUserInfo.EatenCardsCount.ToString();
        winStreakText.text = fullUserInfo.WinStreak.ToString();
        maxWinStreakText.text = fullUserInfo.MaxWinStreak.ToString();
        basrasText.text = fullUserInfo.BasraCount.ToString();
        bigBasrasText.text = fullUserInfo.BigBasraCount.ToString();
        winRatioText.text = fullUserInfo.WinRatio.ToString("p2");
        totalEarnedMoney.text = fullUserInfo.TotalEarnedMoney.ToString();

        gameObject.SetActive(true);
    }

    public void Challenge()
    {
        if (Repository.I.PersonalFullInfo.Money < RoomSettings.MinBet)
            Toast.I.Show("مالك لا يكفي");

        UniTask.Create(async () =>
        {
            var res = await Controller.I.InvokeAsync<MatchRequestResult>("RequestMatch", Id);

            if (res == MatchRequestResult.Available)
                BlockingPanel.Show("a challenge request is sent to the player",
                        () => Controller.I.Send("CancelChallengeRequest"))
                    .Forget(e => throw e);
            else
                Toast.I.Show(res.ToString());
        });
    }

    public void ToggleFollow()
    {
        UniTask.Create(async () =>
        {
            await Controller.I.SendAsync("ToggleFollow", Id);

            switch (FullUserInfo.Friendship)
            {
                case (int)FriendShip.Friend:
                    FullUserInfo.Friendship = (int)FriendShip.Following;
                    Repository.I.PersonalFullInfo.Followings
                        .RemoveAll(i => i.Id == Id);
                    break;
                case (int)FriendShip.Follower:
                    FullUserInfo.Friendship = (int)FriendShip.None;
                    Repository.I.PersonalFullInfo.Followings
                        .RemoveAll(i => i.Id == Id);
                    break;
                case (int)FriendShip.Following:
                    FullUserInfo.Friendship = (int)FriendShip.Friend;
                    Repository.I.PersonalFullInfo.Followings.Add(FullUserInfo);
                    break;
                case (int)FriendShip.None:
                    FullUserInfo.Friendship = (int)FriendShip.Follower;
                    Repository.I.PersonalFullInfo.Followings.Add(FullUserInfo);
                    break;
            }


            UpdateFriendShipView();
        });
    }

    private void UpdateOpenMatchesView()
    {
        if (FullUserInfo.EnableOpenMatches)
        {
            openMatchesCheck.gameObject.SetActive(true);
            openMatchesText.text = "يمكن لاي شحص ان بتحداك للعب";
        }
        else
        {
            openMatchesCheck.gameObject.SetActive(false);
            openMatchesText.text = "يمكن للاصدقاء فقط تحديك للعب";
        }
    }

    public void ToggleOpenMatches()
    {
        UniTask.Create(async () =>
        {
            await Controller.I.SendAsync("ToggleOpenMatches");
            FullUserInfo.EnableOpenMatches = !FullUserInfo.EnableOpenMatches;
            UpdateOpenMatchesView();
        });
    }

    public void Destroy()
    {
        Object.Destroy(gameObject);
    }
}