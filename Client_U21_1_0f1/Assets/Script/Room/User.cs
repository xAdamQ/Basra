using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using BestHTTP.SecureProtocol.Org.BouncyCastle.Asn1.X509;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Basra.Client.Room
{
    public enum UserType
    {
        Me,
        Oppo
    }

    public interface IUserInterface
    {
        Vector3 Position { get; set; }
        Vector3 EulerAngles { get; set; }
        string Name { get; set; }
    }

    public class UserInterfaceFactory : PlaceholderFactory<IUserInterface>
    {
    }

    public class UserInterface : MonoBehaviour, IUserInterface
    {
        [SerializeField] TextMesh NameText;

        public Vector3 Position
        {
            get => transform.position;
            set => transform.position = value;
        }

        public Vector3 EulerAngles
        {
            get => transform.eulerAngles;
            set => transform.eulerAngles = value;
        }

        public string Name
        {
            get => NameText.text;
            set => NameText.text = value;
        }
    }

    public interface IUser
    {
        string Name { get; }
        Client.User UserData { get; }
        List<ICard> Cards { get; }
        int TurnId { get; }
        UserType Type { get; }
        void EnterTurn();
        void CancelTurn();
        void CreateCards_Me(int[] hand);
        void CreateCards_Oppo();
        TurnTimer TurnTimer { get; }
        List<ICard> Eaten { get; }
    }

    public class User : IUser
    {
        public IUserInterface Interface { get; }

        public const int Size = 4;
        public const int HandTime = 8000;

        public string Name { get; }

        public Client.User UserData { get; }

        public List<ICard> Cards { get; } = new List<ICard>();

        public TurnTimer TurnTimer { get; }

        public int TurnId { get; }

        public UserType Type { get; }

        private readonly Card.Factory _cardFactory;
        private IRoom _room;

        public List<ICard> Eaten { get; }

        [Inject]
        public User(string name, int turnId, Card.Factory cardFactory, UserInterfaceFactory userInterfaceFactory,
            TurnTimer.Factory turnTimerFactory, IRoom room)
        {
            _cardFactory = cardFactory;
            _room = room;
            Interface = userInterfaceFactory.Create();
            TurnTimer = turnTimerFactory.Create();

            UserData = new Client.User {Name = name};
            TurnId = turnId;

            Type = turnId == _room.MyTurnId ? UserType.Me : UserType.Oppo;

            Interface.Name = UserData.Name;
            Interface.Position = UserPositions[TurnId];
            Interface.EulerAngles = UserRotations[TurnId];

            if (Type == UserType.Me)
            {
                TurnTimer.AddToOnElapsed(OnTurnTimeout);
            }
        }

        private const float UpperPadding = 1.5f, BottomPadding = 1f;

        private static readonly Vector2[] UserPositions =
        {
            new Vector2(0, -5 + BottomPadding),
            new Vector2(0, 5 - UpperPadding),
            new Vector2(2.5f, 0),
            new Vector2(-2.5f, 0),
        };

        private static readonly Vector3[] UserRotations =
        {
            new Vector3(),
            new Vector3(0, 0, 180),
            new Vector3(0, 0, 90),
            new Vector3(0, 0, -90),
        };


        private void OnTurnTimeout()
        {
            NetManager.I.Send("InformTurnTimeout");
            //this can't be instant because the random algo is not excpected
        }

        public void EnterTurn()
        {
            TurnTimer.Play();
        } //integration

        public void CancelTurn()
        {
            TurnTimer.Stop();
        } //integration

        private void PlaceCards()
        {
            var pointer = new Vector3(-(Size / 2) * Card.Bounds.x, 0, 0);
            pointer.x -= pointer.x / 2f;

            var spacing = new Vector3(Card.Bounds.x, 0, .05f);

            for (var i = 0; i < Cards.Count; i++)
            {
                Cards[i].Interface.Transform.localPosition = pointer;
                pointer += spacing;
            }
        }
        //act on all cards because we don't add indie cards

        public void CreateCards_Me(int[] hand)
        {
            for (var i = 0; i < hand.Length; i++)
            {
                var card = _cardFactory.Create(this, hand[i]);
                Cards.Add(card);
                card.Type = CardOwner.Me;
            }

            PlaceCards();
        }

        public void CreateCards_Oppo()
        {
            for (var i = 0; i < Size; i++)
            {
                var card = _cardFactory.Create(this, -1);
                Cards.Add(card);
            }

            PlaceCards();
        }

        public class Factory : PlaceholderFactory<string, int, IUser>
        {
        }
    }
}