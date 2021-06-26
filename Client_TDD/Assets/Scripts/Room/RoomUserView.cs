using System.Collections.Generic;
using System.Linq;
using Basra.Models.Client;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoomUserView : MinUserView
{
    [Inject] private RoomSettings _roomSettings;
    [Inject] private IRepository _repository;

    public override void ShowFullInfo()
    {
        var oppoFullInfo = _roomSettings.OpposInfo.First(_ => _.FullUserInfo.Id == id);
        _fullUserView.Show(oppoFullInfo.FullUserInfo ?? _repository.PersonalFullInfo);
    }

    public class Factory
    {
        [Inject] private readonly IInstantiator _instantiator;

        private readonly GameObject[] _roomUserViewsPrefabs;
        private readonly Transform _parent;

        public Factory(GameObject[] roomUserViewsPrefabs, Transform parent)
        {
            _roomUserViewsPrefabs = roomUserViewsPrefabs;
            _parent = parent;
        }

        //tested
        public MinUserView Create(int turn, MinUserInfo minUserInfo)
        {
            var view = _instantiator.InstantiatePrefab(_roomUserViewsPrefabs[turn], _parent)
                .GetComponent<MinUserView>();

            view.Init(minUserInfo);

            return view;
        }

        public List<MinUserView> Create(List<MinUserInfo> minUserInfo)
        {
            var views = new List<MinUserView>();

            for (int i = 0; i < minUserInfo.Count; i++)
            {
                views.Add(Create(i, minUserInfo[i]));
            }

            return views;
        }

    }
}