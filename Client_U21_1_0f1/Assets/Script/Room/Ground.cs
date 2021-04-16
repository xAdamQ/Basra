using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using System;
using System.Linq;

//the use of Gos in unity
//in your game context when you need them?
//represent actual visual elements in your scene
//so can these elemets relay on non-components?
//especially on unity events like mouse down?
//what makes me sure that I won't need the go to script relation

//the humle pattern seems over-work

namespace Basra.Client.Room
{
    public class Ground : MonoBehaviour
    {
        private readonly Card.Factory _cardFactory;
        public List<ICard> Cards;

        public static Vector2 Bounds = new Vector2(1.5f, 2.5f);
        private IRoom Room;

        public static GameObject Prefab { get; private set; }

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Ground");
        }

        public Ground(IRoom room, Card.Factory cardFactory)
        {
            _cardFactory = cardFactory;
        }

        public static Ground Construct(IRoom room)
        {
            var ground = Instantiate(Prefab, Vector3.zero, new Quaternion()).GetComponent<Ground>();
            ground._construct(room);
            return ground;
        }

        private void _construct(IRoom room)
        {
            Room = room;
        }

        public void CreateInitialCards(int[] ground)
        {
            for (var i = 0; i < ground.Length; i++)
            {
                var card = MakeCard(ground[i]);
                Distribute(card);
            }
        }

        public void Distribute(ICard card)
        {
            Cards.Add(card);
            PlaceCard(card);
        }

        public ICard MakeCard(int id)
        {
            return _cardFactory.Create(null, id);
        }

        private void PlaceCard(ICard card)
        {
            var xPoz = UnityEngine.Random.Range(-Bounds.x, Bounds.x);
            var yPoz = UnityEngine.Random.Range(-Bounds.y, Bounds.y);
            card.Interface.Transform.position = new Vector3(xPoz, yPoz);
        }

        private ICard[] EatenCardsPending;

        public void AddPt1(Card card)
        {
            card.Type = CardOwner.Ground; //simulate not my card (block the user from interacting with it)
            PlaceCard(card); //visual feedback
            EatenCardsPending = Eat(card.Front.Id);
        }

        public void AddPt2(ICard card)
        {
            Array.ForEach(EatenCardsPending, (eatenCard) => eatenCard.Interface.Hide());
            card.User.Eaten.AddRange(EatenCardsPending);

            Debug.Log("eaten cards are " + string.Join(" ,", EatenCardsPending.Select(c => c.Front.Id)));

            card.Interface.Transform.SetParent(transform);

            card.User = null;
            Cards.Add(card);
        }

        public ICard[] Eat(int cardId)
        {
            var cardValue = (cardId % 13) + 1;
            var per = LogicLab.Permutations(Cards);

            var maxGroupLength = -1;
            ICard[] maxGroup = null;
            foreach (var group in per)
            {
                var gSum = group.Select(c => (c.Front.Id % 13) + 1).Sum();
                if (gSum == cardValue && group.Length > maxGroupLength)
                {
                    maxGroup = group;
                    maxGroupLength = maxGroup.Length;
                }
            }

            return maxGroup;
        }
    }
}

namespace Basra.Client
{
    public static partial class LogicLab
    {
        public static int[] Eat(int cardId, int[] ground)
        {
            var cardValue = (cardId % 13) + 1;
            var realGround = new int[ground.Length];
            for (int c = 0; c < ground.Length; c++)
            {
                realGround[c] = (ground[c] % 13) + 1;
            }

            var per = Permutations(realGround);
            var maxGroupLength = -1;
            int[] maxGroup = null;
            foreach (var group in per)
            {
                var gSum = group.Sum();
                if (gSum == cardValue && group.Length > maxGroupLength)
                {
                    maxGroup = group;
                    maxGroupLength = maxGroup.Length;
                }
            }

            return maxGroup;
        }

        public static IEnumerable<T[]> Permutations<T>(IEnumerable<T> source)
        {
            if (null == source)
                throw new ArgumentNullException(nameof(source));

            T[] data = source.ToArray();

            return Enumerable
                .Range(0, 1 << (data.Length))
                .Select(index => data
                    .Where((v, i) => (index & (1 << i)) != 0)
                    .ToArray());
        }
    }
}

#region general instant feedback with reflection, currenlty deprecated

///every out rpc is visual, Real, Server
// public void RecordThrow(Card card)
// {
//     InstantRpcRecord.Current.RecordField(nameof(card.Type), card.Type);
//     InstantRpcRecord.Current.RecoredTransform(card);
// }

#endregion