using NUnit.Framework;
using Moq;
using UnityEngine;
using Zenject;
using Basra.Client.Room;

namespace Basra.Client.Test
{
    [TestFixture]
    public class UserTests : ZenjectUnitTestFixture
    {
        // [Test]
        // public void With90ArmorTakes10PercentDamage()
        // {
        //     //arr
        //     var chr = new Mock<ICharacter>();
        //     var inv = new Inventory(chr.Object);
        //     var pants = new Item {EquipmentSlot = EquipmentSlot.LeftHand, Armor = 40};
        //     var shield = new Item {EquipmentSlot = EquipmentSlot.Chest, Armor = 50};
        //
        //     //act
        //     inv.EquipItem(pants);
        //     inv.EquipItem(shield);
        //     chr.Setup(obj => obj.Inventory).Returns(inv);
        //     chr.Setup(obj => obj.Level).Returns(15);
        //
        //     var actual = DamageCalc.CalcDamage(1000, chr.Object);
        //
        //     //ass
        //     var expected = 100;
        //
        //     Assert.AreEqual(expected, actual);
        // }

        [SetUp]
        public void AllMockInstall()
        {
            Container.Bind<ITurnTimerInterface>().FromMock().AsSingle();
            // Container.Bind<ICardInterface>().FromMock().AsSingle();
            // Container.Bind<IUserInterface>().FromMock().AsSingle();
            Container.Bind<IRoom>().FromMock().AsSingle();
            Container.Bind<ICard>().FromMock();

            Container.BindFactory<ICardInterface, CardInterfaceFactory>().FromMock();
            Container.BindFactory<IUserInterface, UserInterfaceFactory>().FromMock();

            Container.BindFactory<IUser, int, ICard, Card.Factory>()
                .FromMethod((c, u, id) => Container.Resolve<ICard>());
            Container.BindFactory<TurnTimer, TurnTimer.Factory>();
            Container.BindInterfacesTo<Room.User>().AsSingle().WithArguments("hany", 1);
        }

        [Test]
        public void TestSomething()
        {
            var user = Container.Resolve<IUser>();
            // user.CreateCards_Oppo();
        }
    }
}