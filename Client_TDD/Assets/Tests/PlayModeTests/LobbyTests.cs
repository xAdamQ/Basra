using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace PlayModeTests
{
    public class LobbyTests : ZenjectUnitTestFixture
    {
        protected void InstallLobbyModules(LobbyInstaller.Settings settings)
        {
            var installerPrefab =
                AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Contexts/LobbyContext.Prefab");
            var installer = Object.Instantiate(installerPrefab).GetComponent<LobbyInstaller>();

            Container.BindInstance(settings).WhenInjectedInto<LobbyInstaller>();
            Container.Inject(installer); //set it's container to the current test container

            installer.InstallBindings();
        }
        private void LoadProjectModules()
        {
            Container.Bind<BlockingOperationManager>().AsSingle();
        }

        // A UnityTest behaves like a coroutine in PlayMode
        // and allows you to yield null to skip a frame in EditMode
        [UnityTest]
        public IEnumerator CardbackShop_InteractiveTest()
        {
            LoadProjectModules(); //if the lobby is self contained, it should have this like camera

            Container.BindInterfacesTo<ConsoleToast>().AsSingle();

            var repoMock = new Mock<IRepository>();
            repoMock.Setup(r => r.CardbackPrices).Returns(new int[] {100, 150, 300, 600, 960, 6000, 70003, 10007});
            repoMock.Setup(r => r.PersonalFullInfo).Returns(new PersonalFullUserInfo
            {
                OwnedCardBackIds = new List<int> {1, 3},
                SelectedCardback = 3
            });

            var bp = new Mock<IBlockingPanel>();
            bp.Setup(b => b.Show(It.IsAny<string>())).Callback(() => Debug.Log("panel show"));
            bp.Setup(b => b.Hide(It.IsAny<string>())).Callback(() => Debug.Log("panel hide"));

            Container.Bind<IController>().FromMock();
            Container.Bind<IRepository>().FromInstance(repoMock.Object);
            Container.Bind<IBlockingPanel>().FromInstance(bp.Object);

            //load lobby modules from it's installer
            InstallLobbyModules(new LobbyInstaller.Settings(false)
            {
                EnableCardbackShop = true,
            });

            Container.Resolve<CardbackShop>();

            yield return new WaitUntil(() => false); //global flag using hte editor?
        }
    }
}