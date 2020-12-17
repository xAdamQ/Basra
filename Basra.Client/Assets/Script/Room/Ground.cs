using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Basra.Client.Room
{
    public class Ground : MonoBehaviour
    {
        public List<Card> Cards;

        public static Vector2 Bounds = new Vector2(1.5f, 2.5f);
        public RoomManager Room;

        public void Set(int[] ground)
        {
            for (var i = 0; i < ground.Length; i++)
            {
                var card = MakeCard(ground[i]);
                Distribute(card);
            }
        }

        public void Distribute(Card card)
        {
            Cards.Add(card);
            PlaceCard(card);
        }

        public Card MakeCard(int id)
        {
            var card = Instantiate(FrequentAssets.I.CardPrefab, transform).GetComponent<Card>();
            card.AddFront(id);
            return card;
        }

        private void PlaceCard(Card card)
        {
            var xPoz = Random.Range(-Bounds.x, Bounds.x);
            var yPoz = Random.Range(-Bounds.y, Bounds.y);
            card.transform.position = new Vector3(xPoz, yPoz);
        }

        public Card[] ThrowPt1(Card card)
        {
            card.Type = CardOwner.Ground;
            PlaceCard(card);
            return Cards.GetRange(0, 1).ToArray();
        }
        public void ThrowPt2(Card card)
        {
            Cards.Add(card);
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

