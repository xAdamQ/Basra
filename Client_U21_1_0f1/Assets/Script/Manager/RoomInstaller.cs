using Basra.Client.Room;
using UnityEngine;
using Zenject;
using Basra.Client.Room;

public class RoomInstaller : MonoInstaller
{
    [Inject] private Room.Settings Settings;
    [SerializeField] private GameObject UserPrefab;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<Room>().AsSingle().WithArguments(Settings);
        Container.BindInterfacesTo<User>();
        Container.BindFactory<string, int, IUser, User.Factory>();
        Container.BindInterfacesTo<CardInterface>().FromComponentInNewPrefab(UserPrefab);
    }
}