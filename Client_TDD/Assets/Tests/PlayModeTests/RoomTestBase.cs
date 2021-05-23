using System.Linq;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace PlayModeTests
{
    public abstract class RoomTestBase : ZenjectUnitTestFixture
    {
        protected void InstallerRoomServices(RoomInstaller.Settings settings)
        {
            var installerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/RoomInstaller.Prefab");
            var installer = Object.Instantiate(installerPrefab).GetComponent<RoomInstaller>();
            Container.BindInstance(settings).WhenInjectedInto<RoomInstaller>();
            Container.Inject(installer);
            installer.InstallBindings();
        }

        private void LoadCamera()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Camera.Prefab");
            Object.Instantiate(prefab);
        }
        private void LoadCanvas()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Canvas.Prefab");
            Object.Instantiate(prefab);
        }
        private void LoadEventSystem()
        {
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/EventSystem.Prefab");
            Object.Instantiate(prefab);
        }

        protected void LoadCore()
        {
            LoadCamera();
            LoadCanvas();
            LoadEventSystem();
        }

        protected void InstallPlayerFactory()
        {
            var playerPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/RoomUnits/Player.prefab");
            var oppoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/RoomUnits/oppo.prefab");
            Container.Bind<PlayerBase.Factory>().AsSingle().WithArguments(playerPrefab, oppoPrefab);
        }
        protected void InstallCardFactory()
        {
            var cardPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Card.prefab");
            Container.Bind<Card.Factory>().AsSingle().WithArguments(cardPrefab);
        }
        protected void InstallFrontFactory()
        {
            var frontPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Front.prefab");
            var objects = AssetDatabase.LoadAllAssetsAtPath("Assets/Artwork/nums.png");
            var frontSprites = objects.Where(q => q is Sprite).Cast<Sprite>().ToArray();
            Container.Bind<Front.Factory>().AsSingle().WithArguments(frontPrefab, frontSprites);
        }
        protected void InstallGround()
        {
            var groundPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/RoomUnits/Ground.prefab");
            Container.BindInterfacesTo<Ground>().FromComponentInNewPrefab(groundPrefab).AsSingle();
        }
        protected void InstallTurnTimer()
        {
            var groundPrefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/AppUnits/turn timer.prefab");

            var canvas = Object.FindObjectOfType<Canvas>().transform;

            Container.Bind<TurnTimer>().FromComponentInNewPrefab(groundPrefab)
                .UnderTransform(canvas)
                .AsSingle();
        }
        protected void InstallRoomUserViewFactory()
        {
            var roomUserViewPrefabs =
                AssetDatabase.LoadAllAssetsAtPath("Assets/Prefabs/UI/room min user views").Cast<GameObject>().ToArray();

            var canvas = Object.FindObjectOfType<Canvas>().transform;

            Container.Bind<RoomUserView.Factory>().AsSingle()
                .WithArguments(roomUserViewPrefabs, canvas);
        }
    }
}