// using UnityEngine;
// using Zenject;
//
// public class MasterInstaller : MonoInstaller
// {
//     [SerializeField] private GameObject shopContextPrefab;
//     public override void InstallBindings()
//     {
//         Container.BindInterfacesAndSelfTo<MasterController>().AsSingle();
//         Container.BindFactory<ShopController, ShopController.Factory>().FromSubContainerResolve()
//             .ByNewContextPrefab(shopContextPrefab);
//     }
// }

