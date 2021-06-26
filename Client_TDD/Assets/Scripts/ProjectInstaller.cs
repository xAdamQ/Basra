using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class ProjectInstaller : MonoInstaller
{
    [SerializeField] private GameObject lobbyContextPrefab, roomContextPrefab, blockingPanelPrefab;
    [SerializeField] private GameObject standardCanvasPrefab;
    [SerializeField] private GameObject cameraPrefab, eventSystemPrefab;
    [SerializeField] private GameObject toastsPrefab;

    public class Settings
    {
        public bool EnableController;
        public bool EnableRepository;
        public bool EnableLobbyFactory;
        public bool EnableRoomFactory;
        public bool EnableBlockingPanel;
        public bool EnableBlockingOperationManager;
        public bool EnableToast;
        public bool EnableReferenceInstantiator;

        public Settings(bool defaultServiceState)
        {
            EnableReferenceInstantiator = defaultServiceState;
            EnableController = defaultServiceState;
            EnableRepository = defaultServiceState;
            EnableLobbyFactory = defaultServiceState;
            EnableRoomFactory = defaultServiceState;
            EnableBlockingPanel = defaultServiceState;
            EnableBlockingOperationManager = defaultServiceState;
            EnableToast = defaultServiceState;
        }
    }

    [InjectOptional] private Settings _settings = new Settings(true);


    public override void InstallBindings()
    {
        var loadedCanvas = FindObjectOfType<Canvas>();
        var moduleGroupCanvas = (!loadedCanvas) ? Instantiate(standardCanvasPrefab).transform : loadedCanvas.transform;

        if (!FindObjectOfType<Camera>()) Container.InstantiatePrefab(cameraPrefab);
        if (!FindObjectOfType<EventSystem>()) Container.InstantiatePrefab(eventSystemPrefab);


        if (_settings.EnableReferenceInstantiator)
            Container.Bind<ReferenceInstantiator>().AsSingle();

        if (_settings.EnableController)
            Container.BindInterfacesTo<Controller>().AsSingle();

        if (_settings.EnableRepository)
            Container.BindInterfacesTo<Repository>().AsSingle();

        if (_settings.EnableLobbyFactory)
            Container.BindFactory<LobbyController, LobbyController.Factory>()
            .FromSubContainerResolve()
            .ByNewContextPrefab(lobbyContextPrefab);

        if (_settings.EnableRoomFactory)
            Container.BindFactory<RoomSettings, RoomController, RoomController.Factory>()
                .FromSubContainerResolve()
                .ByNewContextPrefab<RoomInstaller>(roomContextPrefab);

        if (_settings.EnableBlockingPanel)
            Container.AddInstantSceneModule<BlockingPanel>(blockingPanelPrefab, moduleGroupCanvas, hasAbstraction: true);

        if (_settings.EnableBlockingOperationManager)
            Container.Bind<BlockingOperationManager>().AsSingle();

        Container.AddInstantSceneModule<Toast>(toastsPrefab, moduleGroupCanvas, hasAbstraction: true);
    }
}