using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class RoomUserView : MinUserView
{
    [Inject] private IRoomRepo _roomRepo;
    [Inject] private IRepository _repository;

    public override void ShowFullInfo()
    {
        var oppoFullInfo = _roomRepo.OpposInfo.FirstOrDefault(_ => _.Id == id);
        _fullUserView.Show(oppoFullInfo ?? _repository.PersonalFullInfo);
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
    }
}