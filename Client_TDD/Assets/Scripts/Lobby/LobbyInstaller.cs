using UnityEngine;
using Zenject;

public class LobbyInstaller : MonoInstaller
{
    [SerializeField] private GameObject publicMinUserViewPrefab;
    [SerializeField] private PublicFullUserView publicFullUserView;
    [SerializeField] private PersonalFullUserView personalFullUserView;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<Lobby>().AsSingle().NonLazy();
        Container.BindFactory<PublicMinUserView, PublicMinUserView.BasicFactory>()
            .FromComponentInNewPrefab(publicMinUserViewPrefab);
        Container.BindInstance(publicFullUserView).AsSingle();
        Container.BindInstance(personalFullUserView).AsSingle();
    }
}