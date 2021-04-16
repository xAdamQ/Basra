using UnityEngine;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<Controller>().AsSingle();
        Container.BindInterfacesTo<Repository>().AsSingle();
        Debug.Log("ProjectInstaller installed");
    }
}