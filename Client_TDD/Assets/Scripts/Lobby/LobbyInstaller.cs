using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using Zenject;

public class LobbyInstaller : MonoInstaller
{
    [SerializeField] private GameObject publicMinUserViewPrefab;
    [SerializeField] private GameObject friendsViewPrefab;
    [SerializeField] private GameObject roomChoiceViewPrefab;
    [SerializeField] private GameObject personalActiveUserViewPrefab;

    [SerializeField] private GameObject fullUserViewPrefab;
    [SerializeField] private GameObject personalFullUserViewPrefab;

    [SerializeField] private GameObject CameraPrefab, EventSystemPrefab;
    [SerializeField] private GameObject standardCanvasPrefab;

    [SerializeField] private GameObject cardbackShopPrefab;
    [SerializeField] private GameObject cardbackShopItemPrefab;
    [SerializeField] private AssetReference cardbackSheetRef;

    [InjectOptional] private Settings _settings = new Settings();

    public class Settings
    {
        public bool EnableLobbyController;
        public bool EnableMinUserViewFactory;
        public bool EnableFirendsView;
        public bool EnablePersonalActiveUserView;
        public bool EnableFullUserView;
        public bool EnablePersonalFullUserView;
        public bool EnableCardbackShop;
        public bool EnableRoomChoicesView;

        public Settings(bool defaultServiceState = true)
        {
            EnableLobbyController = defaultServiceState;
            EnableMinUserViewFactory = defaultServiceState;
            EnableFirendsView = defaultServiceState;
            EnablePersonalActiveUserView = defaultServiceState;
            EnableFullUserView = defaultServiceState;
            EnablePersonalFullUserView = defaultServiceState;
            EnableCardbackShop = defaultServiceState;
            EnableRoomChoicesView = defaultServiceState;
        }
    }

    public override void InstallBindings()
    {
        if (!FindObjectOfType<Camera>()) ProjectContext.Instance.Container.InstantiatePrefab(CameraPrefab);
        if (!FindObjectOfType<EventSystem>()) ProjectContext.Instance.Container.InstantiatePrefab(EventSystemPrefab);
        //general across scenes

        var standardCanvas = Container.InstantiatePrefab(standardCanvasPrefab).transform;

        if (_settings.EnableLobbyController)
            Container.BindInterfacesAndSelfTo<LobbyController>().AsSingle().NonLazy();

        if (_settings.EnableMinUserViewFactory)
            Container.BindFactory<MinUserView, MinUserView.BasicFactory>()
                .FromComponentInNewPrefab(publicMinUserViewPrefab);

        if (_settings.EnableFirendsView)
            Container.AddInstantSceneModule<FriendsView>(friendsViewPrefab, standardCanvas);

        if (_settings.EnablePersonalActiveUserView)
            Container.AddInstantSceneModule<PersonalActiveUserView>(personalActiveUserViewPrefab, standardCanvas);

        if (_settings.EnableFullUserView)
            Container.AddInstantSceneModule<FullUserView>(fullUserViewPrefab, standardCanvas);
        if (_settings.EnablePersonalFullUserView)
            Container.AddInstantSceneModule<PersonalFullUserView>(personalFullUserViewPrefab, standardCanvas);

        if (_settings.EnableCardbackShop)
            Container.AddInstantSceneModule<CardbackShop>(cardbackShopPrefab, standardCanvas)
                .WithArguments(cardbackSheetRef, cardbackShopItemPrefab);

        if (_settings.EnableRoomChoicesView)
            Container.InstantiatePrefab(roomChoiceViewPrefab, standardCanvas);
        // Container.AddInstantSceneModule<RoomRequester>(roomChoiceViewPrefab, standardCanvas);
    }
}