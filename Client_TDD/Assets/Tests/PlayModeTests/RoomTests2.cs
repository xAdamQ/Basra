using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basra.Common;
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
        static FullUserInfo[] fullUserInfos = new[]
        {
            new FullUserInfo
            {
                BasraCount = 3,
                BigBasraCount = 3,
                PlayedRoomsCount = 56,
                WonRoomsCount = 22,
                EatenCardsCount = 298,
                WinStreak = 3,
                Id = "tstId",
                Name = "7oda el gamed",
                SelectedTitleId = 1,
                Picture = Texture2D.redTexture,
                SelectedCardback = 7,
            },
            new FullUserInfo
            {
                BasraCount = 3,
                BigBasraCount = 3,
                PlayedRoomsCount = 56,
                WonRoomsCount = 22,
                EatenCardsCount = 298,
                WinStreak = 3,
                Id = "tstId",
                Name = "7oda el gamed",
                SelectedTitleId = 1,
                Picture = Texture2D.redTexture,
                SelectedCardback = 2,
            },
            new FullUserInfo
            {
                BasraCount = 3,
                BigBasraCount = 3,
                PlayedRoomsCount = 56,
                WonRoomsCount = 22,
                EatenCardsCount = 298,
                WinStreak = 3,
                Id = "tstId",
                Name = "7oda el gamed",
                SelectedTitleId = 1,
                Picture = Texture2D.redTexture,
                SelectedCardback = 5,
            },
            new FullUserInfo
            {
                BasraCount = 3,
                BigBasraCount = 3,
                PlayedRoomsCount = 56,
                WonRoomsCount = 22,
                EatenCardsCount = 298,
                WinStreak = 3,
                Id = "tstId",
                Name = "7oda el gamed",
                SelectedTitleId = 1,
                Picture = Texture2D.redTexture,
                SelectedCardback = 9,
            },
        };

        [UnityTest]
        public IEnumerator Background_ShouldShowWithBiggestLevel() => UniTask.ToCoroutine(
            async () =>
            {
                await LoadEss();

                await Background.Create();
                Background.I.SetForRoom(new List<FullUserInfo>()
                {
                    new FullUserInfo()
                    {
                        SelectedBackground = 2,
                    },
                    new FullUserInfo()
                    {
                        SelectedBackground = 3,
                    },
                    new FullUserInfo()
                    {
                        SelectedBackground = 1,
                    },
                    new FullUserInfo()
                    {
                        SelectedBackground = 0,
                    },
                });


                await UniTask.Delay(5000);

                await Background.Create();
                Background.I.SetForRoom(new List<FullUserInfo>()
                {
                    new FullUserInfo()
                    {
                        SelectedBackground = 2,
                    },
                    new FullUserInfo()
                    {
                        SelectedBackground = 3,
                    },
                    new FullUserInfo()
                    {
                        SelectedBackground = 1,
                    },
                    new FullUserInfo()
                    {
                        SelectedBackground = 0,
                    },
                });

                await UniTask.Delay(5000);
            });

        [UnityTest]
        public IEnumerator ItemShops_Interactive() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            new ProjectReferences() { Canvas = canvas };

            Controller.I = new Mock<IController>().Object;
            new Repository();
            await Toast.Create();
            new BlockingOperationManager();

            await UniTask.DelayFrame(2);

            Repository.I.PersonalFullInfo = new PersonalFullUserInfo()
            {
                SelectedCardback = 2,
                SelectedBackground = 3,

                OwnedBackgroundsIds = new List<int>() { 1, 3 },
                OwnedCardBackIds = new List<int>() { 0, 2, 3, 9 },

                Money = 1000,
            };

            Repository.I.PersonalFullInfo.DecreaseMoneyAimTimeLeft().Forget();

            await Shop.Create(canvas, ItemType.Cardback);
            await Shop.Create(canvas, ItemType.Background);

            await UniTask.Delay(int.MaxValue);
        });

        [UnityTest]
        public IEnumerator loadFolder() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            var a = await Addressables.LoadAssetsAsync<Sprite>("background", _ => { });
        });

        [UnityTest]
        public IEnumerator ChatSystem() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            RoomReferences.I = new RoomReferences { Canvas = canvas };
            Controller.I = new Mock<IController>().Object;
            RoomSettings.I = new RoomSettings(0, 0, null, 0);

            await global::ChatSystem.Create();
            var cs = global::ChatSystem.I;

            await UniTask.DelayFrame(1);

            cs.ShowMessage(1, "kofta");
            await UniTask.Delay(2000);

            cs.ShowMessage(2, "angle");
            await UniTask.Delay(2000);

            cs.ShowMessage(3, "angry");
            await UniTask.Delay(2000);

            cs.ShowMessage(1, "dizzy");
            await UniTask.Delay(2000);

            cs.ShowMessage(3, "kofta");
            await UniTask.Delay(2000);

            await UniTask.Delay(int.MaxValue);
        });

        [UnityTest]
        public IEnumerator AddressablesTests()
        {
            yield return LoadEss2();

            // var handle = Addressables.LoadAssetAsync<Sprite>(BackgroundType.berry.ToString());
            // yield return handle;
            // new GameObject().AddComponent<SpriteRenderer>().sprite = handle.Result;
            //
            // var handle2 = Addressables.LoadAssetAsync<Sprite>(BackgroundType.brownLeaf.ToString());
            // yield return handle2;
            // var go = new GameObject().AddComponent<SpriteRenderer>();
            // go.sprite = handle2.Result;
            // go.transform.position = Vector3.right * 2;
            //
            // Addressables.Release(handle);
            // Addressables.Release(handle2.Result);

            // var handle3 = Addressables.LoadAssetsAsync<Sprite>(new List<string>
            // {
            //     BackgroundType.berry.ToString(),
            //     BackgroundType.arabesqueBlack.ToString(),
            //     BackgroundType.blueTrans.ToString(),
            //     BackgroundType.arabesqueDark.ToString(),
            // }, sprite => { Debug.Log(sprite); }, Addressables.MergeMode.Union);
            //
            // yield return handle3;
            //
            // Debug.Log(handle3.Result.Count);

            Extensions.LoadAndReleaseAsset<Sprite>(BackgroundType.purpleCris.ToString(), sprite =>
                new GameObject()
                    .AddComponent<SpriteRenderer>().sprite = sprite).Forget(e => throw e);

            yield return new WaitUntil(() => false);
        }

        [UnityTest]
        public IEnumerator AutoThrow() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            RoomReferences.I = new RoomReferences
                { Canvas = canvas, Root = new GameObject().transform };
            RoomSettings.I = new RoomSettings(0, 0, fullUserInfos.ToList(), 0);

            new RoomUserView.Manager();
            RoomUserView.Manager.I.Init();

            Controller.I = new Mock<IController>().Object;
            CoreGameplay.I = new Mock<ICoreGameplay>().Object;
            await Ground.Create();

            var oppo = await PlayerBase.Create(0, 1, 0) as IOppo;
            await oppo.Distribute();

            Ground.I.Distribute(new List<int> { 1, 2, 3, 4 });

            oppo.Throw(new ThrowResult { ThrownCard = 5, EatenCardsIds = new List<int>() });
            await UniTask.Delay(2000);
            oppo.Throw(new ThrowResult { ThrownCard = 5, EatenCardsIds = new List<int>() });
            await UniTask.Delay(2000);
            oppo.Throw(new ThrowResult { ThrownCard = 5, EatenCardsIds = new List<int>() });
            await UniTask.Delay(2000);
            oppo.Throw(new ThrowResult { ThrownCard = 5, EatenCardsIds = new List<int>() });
            await UniTask.Delay(2000);
        });
    }
}