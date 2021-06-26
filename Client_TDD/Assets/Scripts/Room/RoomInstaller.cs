using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using Zenject;

public class RoomInstaller : MonoInstaller
{
    [SerializeField]
    private GameObject[]
        roomUserViewPrefabs,
        playerPrefabs;

    [SerializeField] private Sprite[] frontSprites;

    [SerializeField]
    private GameObject
        frontPrefab,
        cardPrefab,
        groundPrefab,
        fullUserViewPrefab,
        personalFullUserViewPrefab,
        standardCanvasPrefab,
        turnTimerPrefab,
        CameraPrefab,
        EventSystemPrefab;


    [Inject] private readonly RoomSettings _roomSettings;

    public class ModuleSwitches
    {
        public bool EnableRoomController;
        public bool EnableCoreGameplay;
        public bool EnableRoomUserViewFactory;
        public bool EnableFrontFactory;
        public bool EnablePlayerBaseFactory;
        public bool EnableCardFactory;
        public bool EnableGround;
        public bool EnableFullUserView;
        public bool EnablePersonalUserView;
        public bool EnableTurnTimer;

        public ModuleSwitches(bool defaultServiceState)
        {
            EnableRoomController =
            EnableCoreGameplay =
            EnableRoomUserViewFactory =
            EnableFrontFactory =
            EnablePlayerBaseFactory =
            EnableCardFactory =
            EnableGround =
            EnableFullUserView =
            EnablePersonalUserView =
            EnableTurnTimer =
            defaultServiceState;
        }
    }

    [InjectOptional] private ModuleSwitches _moduleSwitches = new ModuleSwitches(true);


    [System.Serializable]
    /// <summary>
    /// things here are exposed to all modules at playtime
    /// but the prefabs are not lited here because they are used to construct
    /// game modules at container start
    /// </summary>
    public class Refernces
    {
        public AssetReference
         RoomResultPanelRef;

        //this class contains scenes assigning and dyanamic assigning like this
        [HideInInspector] public Transform Canvas;
    }

    [SerializeField] private Refernces references;

    public override void Start()
    {
        base.Start();

        if (!FindObjectOfType<Camera>()) Instantiate(CameraPrefab);
        if (!FindObjectOfType<EventSystem>()) Instantiate(EventSystemPrefab);
    }

    public override void InstallBindings()
    {
        references.Canvas = Container.InstantiatePrefab(standardCanvasPrefab).transform;

        Container.BindInstance(references);

        if (_moduleSwitches.EnableRoomController)
            Container.BindInterfacesAndSelfTo<RoomController>().AsSingle().NonLazy();

        if (_moduleSwitches.EnableCoreGameplay)
            Container.BindInterfacesTo<CoreGameplay>().AsSingle();

        if (!Container.HasBinding<RoomSettings>())
            Container.BindInstance(_roomSettings).AsSingle();
        //todo this maybe required, this will break the tests

        //custom factories
        if (_moduleSwitches.EnableFrontFactory)
            Container.Bind<Front.Factory>().AsSingle().WithArguments(frontPrefab, frontSprites);
        if (_moduleSwitches.EnablePlayerBaseFactory)
            Container.Bind<PlayerBase.Factory>().AsSingle().WithArguments(playerPrefabs);
        if (_moduleSwitches.EnableCardFactory)
            Container.Bind<Card.Factory>().AsSingle().WithArguments(cardPrefab);

        if (_moduleSwitches.EnableGround)
            Container.BindInterfacesTo<Ground>().FromComponentInNewPrefab(groundPrefab).AsSingle();

        if (_moduleSwitches.EnableRoomUserViewFactory)
            Container.Bind<RoomUserView.Factory>().AsSingle().WithArguments(roomUserViewPrefabs, references.Canvas);

        if (_moduleSwitches.EnableFullUserView)
            Container.Bind<FullUserView>().FromComponentInNewPrefab(fullUserViewPrefab).AsSingle();

        if (_moduleSwitches.EnablePersonalUserView)
            Container.Bind<PersonalFullUserView>().FromComponentInNewPrefab(personalFullUserViewPrefab).AsSingle();

        if (_moduleSwitches.EnableTurnTimer)
            Container.AddInstantSceneModule<TurnTimer>(turnTimerPrefab, references.Canvas, hasAbstraction: true);
    }
}