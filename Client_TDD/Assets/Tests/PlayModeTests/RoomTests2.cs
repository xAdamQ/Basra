using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    [TestFixture]
    public class RoomTests2 : UnitTestBase
    {
        [UnityTest]
        public IEnumerator Background_ShouldShowWithBiggestLevel() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            Background.Create();
            Background.I.SetForRoom(new List<FullUserInfo>()
            {
                new FullUserInfo()
                {
                    Level = 10,
                    SelectedBackground = 2,
                },
                new FullUserInfo()
                {
                    Level = 7,
                    SelectedBackground = 3,
                },
                new FullUserInfo()
                {
                    Level = 90,
                    SelectedBackground = 1,
                },
                new FullUserInfo()
                {
                    Level = 14,
                    SelectedBackground = 0,
                },
            });


            await UniTask.Delay(5000);

            Background.Create();
            Background.I.SetForRoom(new List<FullUserInfo>()
            {
                new FullUserInfo()
                {
                    Level = 90,
                    SelectedBackground = 2,
                },
                new FullUserInfo()
                {
                    Level = 7,
                    SelectedBackground = 3,
                },
                new FullUserInfo()
                {
                    Level = 90,
                    SelectedBackground = 1,
                },
                new FullUserInfo()
                {
                    Level = 14,
                    SelectedBackground = 0,
                },
            });

            await UniTask.Delay(5000);
        });

        [UnityTest]
        public IEnumerator ItemShops_Interactive() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            Controller.I = new Mock<IController>().Object;
            new Repository();
            await Toast.Create(canvas);
            await BlockingPanel.Create();
            new BlockingOperationManager();

            await UniTask.DelayFrame(2);

            Repository.I.PersonalFullInfo = new PersonalFullUserInfo()
            {
                SelectedCardback = 2,
                SelectedBackground = 3,

                OwnedBackgroundsIds = new List<int>() {1, 3},
                OwnedCardBackIds = new List<int>() {0, 2, 3, 9},

                Money = 1000,
            };

            Shop.Create(canvas, ItemType.Cardback);
            Shop.Create(canvas, ItemType.Background);

            await UniTask.Delay(int.MaxValue);
        });

        [UnityTest]
        public IEnumerator loadFolder() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            var a = await Addressables.LoadAssetsAsync<Sprite>("background", _ => { });
        });
    }
}