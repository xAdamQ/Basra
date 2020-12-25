using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Basra.Client
{
    public class Character : MonoBehaviour, ICharacter
    {
        public Inventory Inventory { get; set; }
        public int Health { get; set; }
        public int Level { get; set; }

        public void OnItemEquipped(Item item)
        {
            Debug.Log($"you equipped {item} in {item.EquipmentSlot}");
            throw new NotImplementedException();
        }
    }
}
