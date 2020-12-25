using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Basra.Client
{
    public class DamageCalc
    {
        public static int CalcDamage(int amount, float mitiPercent)
        {
            return Convert.ToInt32(amount * (1 - mitiPercent));
        }
        public static int CalcDamage(int amount, ICharacter character)
        {
            var totalArmor = character.Inventory.GetTotalArmor() + (character.Level * 10);
            var multiplier = (100f - totalArmor) / 100;

            return Convert.ToInt32(amount * multiplier);
        }
    }
}
