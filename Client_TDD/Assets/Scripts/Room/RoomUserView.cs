using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

/// <summary>
/// it's dependent on player
/// </summary>
public class RoomUserView : MinUserView
{
    [Inject] private RoomSettings _roomSettings;
    [Inject] private IRepository _repository;

    [SerializeField] private Image turnFillImage;

    public void SetTurnFill(float progress)
    {
        turnFillImage.fillAmount = progress;
    }

    public override void ShowFullInfo()
    {
        var oppoFullInfo = _roomSettings.UserInfos.FirstOrDefault(_ => _.Id == Id);
        _fullUserView.Show(oppoFullInfo ?? _repository.PersonalFullInfo);
    }

    public interface IManager
    {
        List<RoomUserView> RoomUserViews { get; set; }
        List<RoomUserView> Init(List<FullUserInfo> fullUserInfos, int myTurn);
    }

    public class Manager : IManager
    {
        [Inject] private readonly IInstantiator _instantiator;
        [Inject] private readonly GameObject[] _roomUserViewsPrefabs;
        [Inject] private readonly Transform _parent;

        //tested
        private RoomUserView Create(int turn, MinUserInfo minUserInfo)
        {
            var view = _instantiator.InstantiatePrefab(_roomUserViewsPrefabs[turn], _parent)
                .GetComponent<RoomUserView>();

            view.Init(minUserInfo);

            return view;
        }

        public List<RoomUserView> RoomUserViews { get; set; }

        public List<RoomUserView> Init(List<FullUserInfo> fullUserInfos, int myTurn)
        {
            var views = new List<RoomUserView>();

            var oppoPlaceCounter = 1;

            for (int i = 0; i < fullUserInfos.Count; i++)
            {
                var placeIndex = i == myTurn ? 0 : oppoPlaceCounter++;

                views.Add(Create(placeIndex, fullUserInfos[i]));
            }

            RoomUserViews = views;

            return views;
        }
    }
}