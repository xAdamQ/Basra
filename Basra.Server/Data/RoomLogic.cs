using Basra.Server.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Basra.Server.Data
{
    public static partial class RoomLogic
    {
        public static List<int> Eat(int cardId, List<int> ground, out bool basra, out bool bigBasra)
        {
            var KOMI_ID = 19;
            var BOY_VALUE = 11;
            var BOY_IDS = new int[] { 10, 23, 36, 49 };

            basra = false;
            bigBasra = false;

            var cardValue = CardValueFromId(cardId);

            if (cardId == KOMI_ID)
            {
                basra = ground.Count == 1;

                return new List<int>(ground);
            }
            else if (cardValue == BOY_VALUE)
            {
                bigBasra = ground.Count == 1 && BOY_IDS.Contains(ground[0]);

                return new List<int>(ground);
            }
            else if (cardValue > 10)
            {
                return ground.Where(c => CardValueFromId(c) == cardValue).ToList();
            }
            else
            {
                var groups = ground.Permutations();

                var bestGroupLength = -1;
                int[] bestGroup = null;
                foreach (var group in groups)
                {
                    if (group.Select(c => CardValueFromId(c)).Sum() == cardValue && group.Length > bestGroupLength)
                    {
                        bestGroup = group;
                        bestGroupLength = bestGroup.Length;
                    }
                }

                basra = bestGroup.Length == ground.Count;

                return new List<int>(bestGroup);
            }

            int CardValueFromId(int id)
            {
                return (id % 13) + 1;
            }

        }
    }
}