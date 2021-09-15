// using Cysharp.Threading.Tasks;
// using UnityEngine;
// using Zenject;
//
// public class RoomInstaller : MonoInstaller
// {
//     [SerializeField] private GameObject[]
//         roomUserViewPrefabs,
//         playerPrefabs;
//
//     [SerializeField] private Sprite[] frontSprites;
//
//     [SerializeField] private GameObject
//         frontPrefab,
//         cardPrefab,
//         groundPrefab,
//         menuPrefab;
//
//     [Inject] private readonly Canvas _standardCanvasPrefab;
//
//     // [Inject] private readonly RoomSettings _roomSettings;
//     // [InjectOptional] private readonly ActiveRoomState _activeRoomState;
//
//     public class ModuleSwitches
//     {
//         public bool EnableRoomController;
//         public bool EnableCoreGameplay;
//         public bool EnableRoomUserViewFactory;
//         public bool EnableFrontFactory;
//         public bool EnablePlayerBaseFactory;
//         public bool EnableCardFactory;
//         public bool EnableGround;
//         public bool EnableReferenceInstantiator;
//
//         public ModuleSwitches(bool defaultServiceState)
//         {
//             EnableRoomController =
//                 EnableCoreGameplay =
//                     EnableRoomUserViewFactory =
//                         EnableFrontFactory =
//                             EnablePlayerBaseFactory =
//                                 EnableCardFactory =
//                                     EnableGround =
//                                         EnableReferenceInstantiator =
//                                             defaultServiceState;
//         }
//     }
//
//     [InjectOptional] private ModuleSwitches _moduleSwitches = new ModuleSwitches(true);
//
//
//     /// <summary>
//     /// things here are exposed to all modules at playtime
//     /// but the prefabs are not lited here because they are used to construct
//     /// game modules at container start
//     /// </summary>

using UnityEngine;

public class RoomReferences
{
    public static RoomReferences I;

    public RoomReferences()
    {
        I = this;
    }

    //this class contains scenes assigning and dyanamic assigning like this
    public Transform Canvas;
    public Transform Root;
}
//
//     [SerializeField] private References references;
//
//     public override void InstallBindings()
//     {
//         references.Canvas = Container.InstantiatePrefab(_standardCanvasPrefab.gameObject).transform;
//
//         Container.BindInstance(references);
//
//         if (_moduleSwitches.EnableRoomController)
//             if (_activeRoomState != null)
//                 Container.BindInterfacesAndSelfTo<RoomController>().AsSingle().WithArguments(_activeRoomState).NonLazy();
//             else
//                 Container.BindInterfacesAndSelfTo<RoomController>().AsSingle().NonLazy();
//
//         Container.AddInstantSceneModule<RoomMenu>(menuPrefab, references.Canvas);
//
//         if (_moduleSwitches.EnableReferenceInstantiator)
//             Container.Bind<ReferenceInstantiator<RoomInstaller>>().AsSingle();
//
//         if (_moduleSwitches.EnableCoreGameplay)
//             Container.BindInterfacesTo<CoreGameplay>().AsSingle();
//
//         if (!Container.HasBinding<RoomSettings>())
//             Container.BindInstance(_roomSettings).AsSingle();
//
//         //custom factories
//         if (_moduleSwitches.EnableFrontFactory)
//             Container.Bind<Front.Factory>().AsSingle().WithArguments(frontPrefab, frontSprites);
//         if (_moduleSwitches.EnablePlayerBaseFactory)
//             Container.Bind<PlayerBase.Factory>().AsSingle().WithArguments(playerPrefabs);
//         if (_moduleSwitches.EnableCardFactory)
//             Container.Bind<Card.Factory>().AsSingle().WithArguments(cardPrefab);
//
//         if (_moduleSwitches.EnableGround)
//             Container.BindInterfacesTo<Ground>().FromComponentInNewPrefab(groundPrefab).AsSingle();
//
//         if (_moduleSwitches.EnableRoomUserViewFactory)
//             Container.BindInterfacesTo<RoomUserView.Manager>().AsSingle()
//                 .WithArguments(roomUserViewPrefabs, references.Canvas);
//
//         Background.I.SetForRoom(_roomSettings.UserInfos).Forget();
//     }
// }