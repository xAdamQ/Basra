using System;
using Basra.Models.Client;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// this not a preloaded module like core modules
/// this is loaded and destroyed
/// referenced by the module group (room)
/// </summary>
public class RoomResultPanel : MonoBehaviour
{
    [SerializeField] private Text
        competetionScoreText,
        basraScoreText,
        bigBasraScoreText,
        greatEatScoreText,
        eatenCards,
        basras,
        superBasras,
        winRatioChange,
        earnedMoney,
        winStreak;


    [Inject] private readonly IRoomController _roomController;
    [Inject] private readonly LobbyController.Factory _lobbyFactory;

    public static void Instantiate(ReferenceInstantiator<RoomInstaller> referenceInstantiator,
        RoomInstaller.References refs, /*first 2 are services*/RoomXpReport roomXpReport, PersonalFullUserInfo oldInfo,
        PersonalFullUserInfo personalFullUserInfo, int betChoice)
    {
        referenceInstantiator.Instantiate(refs.RoomResultPanelRef,
            go => go.GetComponent<RoomResultPanel>()
                .Construct(roomXpReport, oldInfo, personalFullUserInfo, betChoice),
            refs.Canvas);
    }

    private void Construct(RoomXpReport roomXpReport, PersonalFullUserInfo oldInfo, PersonalFullUserInfo personalFullUserInfo,
        int betChoice)
    {
        Debug.Log(_roomController);
        Debug.Log(_lobbyFactory);

        if (roomXpReport.Competition == 0) competetionScoreText.transform.parent.gameObject.SetActive(false);
        if (roomXpReport.Basra == 0) basraScoreText.transform.parent.gameObject.SetActive(false);
        if (roomXpReport.BigBasra == 0) bigBasraScoreText.transform.parent.gameObject.SetActive(false);
        if (roomXpReport.GreatEat == 0) greatEatScoreText.transform.parent.gameObject.SetActive(false);

        competetionScoreText.text = roomXpReport.Competition.ToString();
        basraScoreText.text = roomXpReport.Basra.ToString();
        bigBasraScoreText.text = roomXpReport.BigBasra.ToString();
        greatEatScoreText.text = roomXpReport.GreatEat.ToString();

        eatenCards.text = (personalFullUserInfo.EatenCardsCount - oldInfo.EatenCardsCount).ToString();
        basras.text = (personalFullUserInfo.BasraCount - oldInfo.BasraCount).ToString();
        superBasras.text = (personalFullUserInfo.BigBasraCount - oldInfo.BigBasraCount).ToString();
        winRatioChange.text = (personalFullUserInfo.WinRatio - oldInfo.WinRatio).ToString("p2");

        winStreak.text = personalFullUserInfo.WinStreak.ToString();

        //know win or loose by this
        var moneyChange = (personalFullUserInfo.Money - oldInfo.Money);

        var bet = RoomSettings.Bets[betChoice];

        if (moneyChange < 0) earnedMoney.color = Color.red;
        else earnedMoney.text = ((bet - (bet * .1f)) * 2).ToString("f0");
    }

    public void ToLobby()
    {
        _roomController.DestroyModuleGroup();
        _lobbyFactory.Create();
    }

    public void RequestRematch()
    {
        throw new System.NotImplementedException();
    }
}