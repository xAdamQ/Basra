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

        ///every out rpc is visual, Real, Server
        public void RecordThrow(Card card)
        {
            InstantRpcRecord.Current.RecoredTransform(card);
        }
        public Card[] VisualThrow(Card card)
        {
            PlaceCard(card);
            return Cards.GetRange(0, 1).ToArray();
        }
        public void ConfirmThrow(Card card)
        {
            Cards.Add(card);
            card.Type = CardType.Ground;
        }

        //card -> list of cards


    }
}