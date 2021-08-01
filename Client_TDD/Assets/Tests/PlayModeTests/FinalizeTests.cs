using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Basra.Common;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    public class FinalizeTests : UnitTestBase
    {
        //DiContainer Container = new DiContainer();

        //private void InstallProjectModule(ProjectInstaller.Settings settings)
        //{
        //    var installerPrefab =
        //        AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Contexts/ProjectInstaller.Prefab");
        //    var installer = UnityEngine.Object.Instantiate(installerPrefab).GetComponent<ProjectInstaller>();
        //    Container.BindInstance(settings).WhenInjectedInto<ProjectInstaller>();
        //    Container.Inject(installer);
        //    installer.InstallBindings();
        //}

        //private void InstallRoomServices()
        //{
        //    var installerPrefab =
        //        AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Contexts/FinalizeContext.Prefab");
        //    var installer = UnityEngine.Object.Instantiate(installerPrefab).GetComponent<FinalizeInstaller>();
        //    Container.Inject(installer);
        //    installer.InstallBindings();
        //}

        static PersonalFullUserInfo personalInfo = new PersonalFullUserInfo
        {
            BasraCount = 3,
            BigBasraCount = 3,
            Money = 433,
            PlayedRoomsCount = 56,
            WonRoomsCount = 22,
            EatenCardsCount = 298,
            WinStreak = 3,
            Id = "tstId",
            Name = "7oda el gamed",
            SelectedTitleId = 1,
            Picture = Texture2D.redTexture,
            MoneyAimTimePassed = null,
        };


        static List<FullUserInfo> fullUserInfos = new List<FullUserInfo>()
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
            },
            new FullUserInfo
            {
                BasraCount = 3,
                BigBasraCount = 3,
                WonRoomsCount = 22,
                EatenCardsCount = 298,
                WinStreak = 3,
                Id = "tstId",
                Name = "7oda el gamed",
                SelectedTitleId = 1,
                Picture = Texture2D.redTexture,
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
            },
        };

        //[UnityTest]
        //public IEnumerator GeneralTest()
        //{
        //    ProjectContext.Instance.Container.Inject(this);

        //    Container.Bind<IController>().FromMock();

        //    InstallProjectModule(new ProjectInstaller.Settings(false)
        //    {
        //        EnableBlockingOperationManager = true,
        //        EnableBlockingPanel = true,
        //        EnableFinalizeFactory = true,
        //        EnableRepository = true,
        //        EnableFullUserView = true,
        //    });

        //    var rs = new RoomSettings(0, 0, fullUserInfos, 0);
        //    var fr = (new FinalizeResult()
        //    {
        //        LastEaterTurnId = 1,
        //        PersonalFullUserInfo = personalInfo,

        //        RoomXpReport = new Basra.Models.Client.RoomXpReport()
        //        {
        //            Basra = 0,
        //            BigBasra = 30,
        //            Competition = 108,
        //            GreatEat = 250,
        //        },

        //        UserRoomStatus = new List<UserRoomStatus>()
        //        {
        //            new UserRoomStatus()
        //            {
        //                Basras = 0,
        //                BigBasras = 1,
        //                WinMoney = 100,
        //                EatenCards = 42,
        //            },
        //            new UserRoomStatus()
        //            {
        //                Basras = 1,
        //                BigBasras = 0,
        //                WinMoney = 0,
        //                EatenCards = 10,
        //            }
        //        }

        //    });

        //    Container.BindInstance(rs);
        //    Container.BindInstance(fr);

        //    var repo = Container.Resolve<IRepository>();
        //    repo.PersonalFullInfo = personalInfo;

        //    var fac = Container.Resolve<FinalizeController.Factory>();

        //    fac.Create(fr, rs);

        //    Container.Resolve<FinalizeController>().Initialize();

        //    yield return new WaitUntil(() => false);
        //}

        [UnityTest]
        public IEnumerator LoadMuv() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            var finalizeResult = (new FinalizeResult()
            {
                LastEaterTurnId = 1,
                PersonalFullUserInfo = personalInfo,

                RoomXpReport = new Basra.Common.RoomXpReport()
                {
                    Basra = 0,
                    BigBasra = 30,
                    Competition = 108,
                    GreatEat = 250,
                },

                UserRoomStatus = new List<UserRoomStatus>()
                {
                    new UserRoomStatus()
                    {
                        Basras = 0,
                        BigBasras = 1,
                        WinMoney = 100,
                        EatenCards = 42,
                    },
                    new UserRoomStatus()
                    {
                        Basras = 1,
                        BigBasras = 0,
                        WinMoney = 0,
                        EatenCards = 10,
                    },
                    new UserRoomStatus()
                    {
                        Basras = 1,
                        BigBasras = 0,
                        WinMoney = 0,
                        EatenCards = 10,
                    },
                    new UserRoomStatus()
                    {
                        Basras = 1,
                        BigBasras = 0,
                        WinMoney = 0,
                        EatenCards = 10,
                    },
                }
            });

            await FinalizeController.Construct(canvas, new RoomSettings(0, 0, fullUserInfos.GetRange(0, 4), 0), finalizeResult);

            await UniTask.Delay(9999999);
        });
        [UnityTest]
        public IEnumerator LoadSingleSpriteFromSprietSheet() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            var s = await Addressables.LoadAssetAsync<Sprite>("FrontSprites[0_0]");
            Debug.Log(s);

            new GameObject().AddComponent<SpriteRenderer>().sprite = s;


            //
            // try
            // {
            //     await Addressables.LoadAssetAsync<Sprite>("FrontSprites[0_0]");
            // }
            // catch (Exception)
            // {
            //     Debug.Log("method 1 failed");
            // }
            // try
            // {
            //     await Addressables.LoadAssetAsync<Sprite>("FrontSprites/[0_0]");
            // }
            // catch (Exception)
            // {
            //     Debug.Log("method 2 failed");
            // }
            // try
            // {
            //     await Addressables.LoadAssetAsync<Sprite>("FrontSprites/0_0");
            // }
            // catch (Exception)
            // {
            //     Debug.Log("method 3 failed");
            // }
            // try
            // {
            //     await Addressables.LoadAssetAsync<Sprite>("FrontSprites/0");
            // }
            // catch (Exception)
            // {
            //     Debug.Log("method 4 failed");
            // }
            // try
            // {
            //     await Addressables.LoadAssetAsync<Sprite>("FrontSprites[0]");
            // }
            // catch (Exception)
            // {
            //     Debug.Log("method 5 failed");
            // }


            s = null;

            await UniTask.Delay(9999999);
        });
    }
}