using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace Basra.Client.Room
{
    public enum UserType { Me, Oppo }
    public class User : MonoBehaviour
    {
        public List<Card> Cards = new List<Card>();
        public static int Size = 4;
        public static float HandTime = 8;

        private string userName;
        public string Name { get => userName; set { userName = value; NameText.text = value; } }
        [SerializeField] TextMesh NameText;

        public RoomManager Room;

        public TurnTimer TurnTimer;

        public int TurnId;

        public UserType Type;

        private void Start()
        {
            if (Type == UserType.Me)
            {
                TurnTimer.Elapsed += OnTurnTimeout;
            }
        }

        private void OnTurnTimeout()
        {
            AppManager.I.HubConnection.Invoke<int>("InformTurnTimeout").OnSuccess(cardIndex => Cards[cardIndex].OverrideThrow());
            //this can't be instant because the random algo is not excpected
        }

        public void StartTurn()
        {
            TurnTimer.Play();
        }

        //act on all cards because we don't add indie cards
        protected void PlaceCards()
        {
            var xPointer = -(Size / 2) * Card.Bounds.x;
            xPointer -= xPointer / 2f;

            var xSpacing = Card.Bounds.x;

            for (var i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.localPosition = Vector3.right * xPointer;
                xPointer += xSpacing;
            }
        }

        //my
        public void Set(int[] hand)
        {
            for (var i = 0; i < hand.Length; i++)
            {
                var card = MakeCard(hand[i]);
                card.Type = CardType.Mine;
            }
            PlaceCards();
        }
        //oppo
        public void Set()
        {
            for (var i = 0; i < Size; i++)
            {
                MakeCard();
            }
            PlaceCards();
        }

        //my
        private Card MakeCard(int id)
        {
            var card = Instantiate(FrequentAssets.I.CardPrefab, transform).GetComponent<Card>();
            card.AddFront(id);
            Cards.Add(card);
            return card;
        }
        //oppo
        private Card MakeCard()
        {
            var card = Instantiate(FrequentAssets.I.CardPrefab, transform).GetComponent<Card>();
            Cards.Add(card);
            return card;
        }

    }
}