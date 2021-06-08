using System.Collections;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Basra.Client.Test
{
    public class InventoryTest
    {
        [Test]
        public void OnlyAllowChest()
        {
            //arr
            var chr = new Mock<ICharacter>();
            var item = new Item { EquipmentSlot = EquipmentSlot.Chest };
            chr.Setup(c => c.OnItemEquipped(item));
            var item2 = new Item { EquipmentSlot = EquipmentSlot.Chest };

            var inv = new Inventory(chr.Object);
            //act
            inv.EquipItem(item);
            inv.EquipItem(item2);

            //assert
            var actual = inv.GetItem(EquipmentSlot.Chest);
            var expected = item2;

            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void TellWhrnEquipped()
        {
            //arr
            var chr = new Mock<ICharacter>();

            var item = new Item { EquipmentSlot = EquipmentSlot.Chest };
            var item2 = new Item { EquipmentSlot = EquipmentSlot.Chest };

            chr.Setup(c => c.OnItemEquipped(item));

            var inv = new Inventory(chr.Object);

            //act
            inv.EquipItem(item);
            inv.EquipItem(item2);

            //Assert.
            chr.Verify(c => c.OnItemEquipped(item));
        }

    }
}
