using System.Collections.Generic;
using System.Linq;

namespace Basra.Client
{
    public class Inventory
    {
        private readonly ICharacter _character;
        Dictionary<EquipmentSlot, Item> Equipped = new Dictionary<EquipmentSlot, Item>();
        List<Item> UnEquipped = new List<Item>();

        public Inventory(ICharacter character)
        {
            _character = character;
        }

        public void EquipItem(Item item)
        {
            if (Equipped.ContainsKey(item.EquipmentSlot))
                UnEquipped.Add(item);

            Equipped[item.EquipmentSlot] = item;

            _character.OnItemEquipped(item);
        }

        public Item GetItem(EquipmentSlot slot)
        {
            if (Equipped.ContainsKey(slot))
                return Equipped[slot];

            return null;
        }

        public int GetTotalArmor()
        {
            return Equipped.Values.Sum(i => i.Armor);
        }
    }
}
