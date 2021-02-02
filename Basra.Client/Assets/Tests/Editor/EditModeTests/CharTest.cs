using NUnit.Framework;
using Moq;
using UnityEngine;

namespace Basra.Client.Test
{
    public class CharTest
    {
        [Test]
        public void With90ArmorTakes10PercentDamage()
        {
            //arr
            var chr = new Mock<ICharacter>();
            var inv = new Inventory(chr.Object);
            var pants = new Item { EquipmentSlot = EquipmentSlot.LeftHand, Armor = 40 };
            var shield = new Item { EquipmentSlot = EquipmentSlot.Chest, Armor = 50 };

            //act
            inv.EquipItem(pants);
            inv.EquipItem(shield);
            chr.Setup(obj => obj.Inventory).Returns(inv);
            chr.Setup(obj => obj.Level).Returns(15);

            var actual = DamageCalc.CalcDamage(1000, chr.Object);

            //ass
            var expected = 100;

            Assert.AreEqual(expected, actual);
        }
    }
}
