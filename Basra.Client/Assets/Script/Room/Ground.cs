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
                OverrideAdd(card);
            }
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

        // public void Add(Card card)
        // {
        //     // PrevCards = new List<Card>(Cards);
        //     // PrevCard = Instantiate(card);
        //     // PrevCard.gameObject.SetActive(false);

        //     Cards.Add(card);
        //     PlaceCard(card);
        //     card.Type = CardType.Ground;
        // }
        // public void ReversAddIF(Card card)
        // {
        //     Cards = new List<Card>(PrevCards);
        //     Destroy(card);
        //     card = PrevCard;
        //     card.gameObject.SetActive(true);
        // }

        // public void AddIF(Card card)
        // {
        //     Cards.Add(card);

        //     PlaceCard(card);

        //     card.Type = CardType.Ground;
        // }

        // public void ShadowAdd(Card card)
        // {
        //     ProcessingCard = card;
        //     PlaceCard(ProcessingCard);
        // }
        // public void ConfirmAdd()
        // {
        //     Cards.Add(ProcessingCard);
        //     ProcessingCard.Type = CardType.Ground;
        // }
        // public void ReverseAdd()
        // {
        //     //return card back to it's position, so you could memorized the state of it
        //     //memorizing is the key for a time machine

        // }

        ///every out rpc is visual, Real, Server
        public Card[] VisualAdd(Card card)
        {
            Cards.Add(card);

            InstantRpcRecord.Current.RecoredTransform(card);
            PlaceCard(card);

            return Cards.CutRange(1, fromEnd: false).ToArray();
        }
        public void RealAdd(Card card)
        {
            Cards.Add(card);
            card.Type = CardType.Ground;
        }
        public void OverrideAdd(Card card)
        {
            Cards.Add(card);
            PlaceCard(card);
            card.Type = CardType.Ground;
        }


    }
}