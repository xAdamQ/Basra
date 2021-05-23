using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameObject lobbyContextPrefab, roomContextPrefab, blockingPanelPrefab;
    [SerializeField] private GameObject standardCanvasPrefab;
    [SerializeField] private GameObject cameraPrefab, eventSystemPrefab;
    [SerializeField] private GameObject toastsPrefab;

    public override void InstallBindings()
    {
        if (!FindObjectOfType<Camera>()) ProjectContext.Instance.Container.InstantiatePrefab(cameraPrefab);
        if (!FindObjectOfType<EventSystem>()) ProjectContext.Instance.Container.InstantiatePrefab(eventSystemPrefab);
        var standardCanvas = Container.InstantiatePrefab(standardCanvasPrefab).transform;
        //each module set has a canvas


        Container.BindInterfacesTo<Controller>().AsSingle();
        Container.BindInterfacesTo<Repository>().AsSingle();

        Container.BindFactory<Lobby, Lobby.Factory>().FromSubContainerResolve()
            .ByNewContextPrefab(lobbyContextPrefab);

        Container.BindFactory<RoomController, RoomController.Factory>().FromSubContainerResolve()
            .ByNewContextPrefab(roomContextPrefab);

        Container.AddInstantSceneModule<BlockingPanel>(blockingPanelPrefab, standardCanvas, hasAbstraction: true);
        Container.Bind<BlockingOperationManager>().AsSingle();

        Container.AddInstantSceneModule<Toast>(toastsPrefab, standardCanvas, hasAbstraction: true);
    }
}