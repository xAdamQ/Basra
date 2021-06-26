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
    [SerializeField]
    private Text
        competetionScoreText,
        basraScoreText,
        bigBasraScoreText,
        greatEatScoreText;

    [SerializeField] private PersonalFullUserView statesView;

    [Inject] private IRoomController _roomController;
    [Inject] private LobbyController.Factory _lobbyFactory;

    public static void Instantiate(ReferenceInstantiator _referenceInstantiator, RoomInstaller.Refernces refs,/*first 2 are services*/ RoomXpReport roomXpReport, PersonalFullUserInfo personalFullUserInfo)
    {
        _referenceInstantiator.Instantiate(refs.RoomResultPanelRef, go => go.GetComponent<RoomResultPanel>().Construct(roomXpReport, personalFullUserInfo), refs.Canvas);
    }

    public void Construct(RoomXpReport roomXpReport, PersonalFullUserInfo personalFullUserInfo)
    {
        if (roomXpReport.Competition == 0) competetionScoreText.transform.parent.gameObject.SetActive(false);
        if (roomXpReport.Basra == 0) basraScoreText.transform.parent.gameObject.SetActive(false);
        if (roomXpReport.BigBasra == 0) bigBasraScoreText.transform.parent.gameObject.SetActive(false);
        if (roomXpReport.GreatEat == 0) greatEatScoreText.transform.parent.gameObject.SetActive(false);

        competetionScoreText.text = roomXpReport.Competition.ToString();
        basraScoreText.text = roomXpReport.Basra.ToString();
        bigBasraScoreText.text = roomXpReport.BigBasra.ToString();
        greatEatScoreText.text = roomXpReport.GreatEat.ToString();

        statesView.Show(personalFullUserInfo);
    }

    public void ToLobby()
    {
        _roomController.DestroyModule();
        _lobbyFactory.Create();
    }

    public void RequestRematch()
    {
        throw new System.NotImplementedException();
    }

}