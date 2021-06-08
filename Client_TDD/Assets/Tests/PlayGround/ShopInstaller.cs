// using UnityEngine;
// using Zenject;
//
// public class ShopInstaller : MonoInstaller
// {
//     [SerializeField] private GameObject frontPrefab;
//
//     [SerializeField] private Sprite[] frontSprites;
//
//     // [SerializeField] private GameObject inputHandler;
//     [SerializeField] private GameObject inputHandlerPrefab;
//
//     public override void InstallBindings()
//     {
//         Container.BindInterfacesAndSelfTo<ShopController>().AsSingle();
//         // Container.BindInterfacesAndSelfTo<InputHandler>().FromInstance(inputHandler).AsSingle();
//         Container.BindInterfacesAndSelfTo<InputHandler>().FromComponentInNewPrefab(inputHandlerPrefab).AsSingle();
//
//         Container.Bind<Front.Factory>().AsSingle().WithArguments(frontPrefab, frontSprites);
//     }
// }

