using Basra.Client;
using Script.Manager;
using UnityEngine;
using Zenject;

public class AppInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<AppManager>().AsSingle();
        // Container.BindInterfacesTo<AppInterface>().AsSingle().WhenInjectedInto<IAppManager>();
        Container.BindInterfacesTo<NetManager>().AsSingle();
    }
}