using Basra.Client;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Zenject;

public class LobbyInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<Lobby>().AsSingle();
    }
}