using System.Collections;
using Cysharp.Threading.Tasks;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace PlayModeTests
{
    [TestFixture]
    public class LobbyTests2 : UnitTestBase
    {
        [UnityTest]
        public IEnumerator MoneyAid() => UniTask.ToCoroutine(async () =>
        {
            await LoadEss();

            Repository.I = new Repository
            {
                PersonalFullInfo = new PersonalFullUserInfo
                {
                    Money = 11,
                    Name = "Holy Hanaka",
                    Picture = Texture2D.linearGrayTexture,
                    Xp = 14352913,
                    MoneyAimTimePassed = null,
                    MoneyAidRequested = 2,
                }
            };

            Repository.I.PersonalFullInfo.DecreaseMoneyAimTimeLeft();

            Controller.I = new Mock<IController>().Object;

            LobbyReferences.I = new LobbyReferences {Canvas = canvas};
            ProjectRefernces.I = new ProjectRefernces {Canvas = canvas};

            await Toast.Create();

            RoomSettings.I = new RoomSettings(0, 0, null, 0);

            await PersonalActiveUserView.Create();

            await UniTask.Delay(int.MaxValue);
        });
    }
}