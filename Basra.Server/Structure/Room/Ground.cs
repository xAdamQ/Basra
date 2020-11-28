using Basra.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Basra.Server.Structure.Room
{
    public class Ground
    {
        public List<int> Cards { get; set; }

        public Ground(List<int> cards)
        {
            Cards = cards;
        }

        public int[] Throw(int card)
        {
            Cards.Add(card);

            if (Cards.Count == 0)
                return null;
            else
                return Cards.CutRange(1).ToArray();
        }
    }
}
