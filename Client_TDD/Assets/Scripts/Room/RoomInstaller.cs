using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Object = UnityEngine.Object;

public class RoomInstaller : MonoInstaller
{
    [SerializeField] private GameObject[] roomUserViewPrefabs;
    [SerializeField] private GameObject frontPrefab;
    [SerializeField] private Sprite[] frontSprites;
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject oppoPrefab;
    [SerializeField] private GameObject groundPrefab;
    [SerializeField] private GameObject fullUserViewPrefab;
    [SerializeField] private GameObject personalFullUserViewPrefab;
    [SerializeField] private GameObject standardCanvasPrefab;
    [SerializeField] private GameObject turnTimerPrefab;

    [SerializeField] private GameObject CameraPrefab, EventSystemPrefab;

    public class Settings
    {
        public bool EnableRoomController;
        public bool EnableRoomUserViewFactory;
        public bool EnableFrontFactory;
        public bool EnablePlayerBaseFactory;
        public bool EnableCardFactory;
        public bool EnableGround;
        public bool EnableFullUserView;
        public bool EnablePersonalUserView;
        public bool EnableTurnTimer;

        public Settings(bool defaultServiceState)
        {
            EnableRoomController = defaultServiceState;
            EnableRoomUserViewFactory = defaultServiceState;
            EnableFrontFactory = defaultServiceState;
            EnablePlayerBaseFactory = defaultServiceState;
            EnableCardFactory = defaultServiceState;
            EnableGround = defaultServiceState;
            EnableFullUserView = defaultServiceState;
            EnablePersonalUserView = defaultServiceState;
            EnableTurnTimer = defaultServiceState;
        }
    }

    [InjectOptional] private Settings _settings = new Settings(true);

    private Transform StandardCanvas => standardCanvas == null
        ? standardCanvas = Instantiate(standardCanvasPrefab.gameObject).transform
        : standardCanvas;
    private Transform standardCanvas;

    public override void Start()
    {
        base.Start();

        if (!FindObjectOfType<Camera>()) Instantiate(CameraPrefab);
        if (!FindObjectOfType<EventSystem>()) Instantiate(EventSystemPrefab);
    }

    public override void InstallBindings()
    {
        if (_settings.EnableRoomController)
            Container.BindInterfacesTo<RoomController>().AsSingle().NonLazy();

        //custom factories
        if (_settings.EnableFrontFactory)
            Container.Bind<Front.Factory>().AsSingle().WithArguments(frontPrefab, frontSprites);
        if (_settings.EnablePlayerBaseFactory)
            Container.Bind<PlayerBase.Factory>().AsSingle().WithArguments(playerPrefab, oppoPrefab);
        if (_settings.EnableCardFactory)
            Container.Bind<Card.Factory>().AsSingle().WithArguments(cardPrefab);

        if (_settings.EnableGround)
            Container.BindInterfacesTo<Ground>().FromComponentInNewPrefab(groundPrefab).AsSingle();
        //
        if (_settings.EnableRoomUserViewFactory)
            Container.Bind<RoomUserView.Factory>().AsSingle().WithArguments(roomUserViewPrefabs, StandardCanvas);

        if (_settings.EnableFullUserView)
            Container.Bind<FullUserView>().FromComponentInNewPrefab(fullUserViewPrefab)
                .AsSingle();
        if (_settings.EnablePersonalUserView)
            Container.Bind<PersonalFullUserView>().FromComponentInNewPrefab(personalFullUserViewPrefab)
                .AsSingle();

        if (_settings.EnableTurnTimer)
            Container.Bind<TurnTimer>().FromComponentInNewPrefab(turnTimerPrefab).UnderTransform(StandardCanvas)
                .AsSingle();
    }
}