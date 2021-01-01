using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Basra.Client.Room.Components
{
    public class User : MonoBehaviour
    {
        public Room.User Logical;

        private static float UpperPadding = 1.5f, ButtomPadding = 1f;
        private static Vector2[] UserPositions = new Vector2[]
        {
            new Vector2(0, -5 + ButtomPadding),
            new Vector2(0, 5 - UpperPadding),
            new Vector2(2.5f, 0),
            new Vector2(-2.5f, 0),
        };
        private static Vector3[] UserRotations = new Vector3[]
        {
            new Vector3(),
            new Vector3(0, 0, 180),
            new Vector3(0, 0, 90),
            new Vector3(0, 0, -90),
        };

        public TurnTimer TurnTimer;

        public void SetName(string name)
        {
            NameText.text = name;
        }
        [SerializeField] TextMesh NameText;

        public List<Card> Cards { get; set; } = new List<Card>();

        private bool Constructed;

        static GameObject Prefab;

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("User");
        }

        public static User Construct(Room.User lUser)
        {
            var user = Object.Instantiate(Prefab).GetComponent<User>();
            user._construct(lUser);
            return user;
        }
        private void _construct(Room.User lUser)
        {
            if (Constructed) throw new System.Exception("the object is already constructed");
            Constructed = true;

            SetName(name);

            transform.position = UserPositions[lUser.TurnId];
            transform.eulerAngles = UserRotations[lUser.TurnId];

            TurnTimer.Construct(Logical.UniTaskTimer);
        }

        public void EnterTurn()
        {
            Logical.EnterTurn();
            TurnTimer.Play();
        }

        public void CancelTurn()
        {
            Logical.CancelTurn();
            TurnTimer.Stop();
        }

        private void PlaceCards()
        {
            var pointer = new Vector3(-(Room.User.HandSize / 2) * Card.Bounds.x, 0, 0);
            pointer.x -= pointer.x / 2f;

            var spacing = new Vector3(Card.Bounds.x, 0, .05f);

            for (var i = 0; i < Cards.Count; i++)
            {
                Cards[i].transform.localPosition = pointer;
                pointer += spacing;
            }
        }
        //act on all cards because we don't add indie cards

        public void CreateCards_Me(int[] hand)
        {
            Logical.CreateCards_Me(hand);
            foreach (var lCard in Logical.Cards)
            {
                var card = Card.Construct(lCard, user: this);
            }
            PlaceCards();
            //for (var i = 0; i < hand.Length; i++)
            //{
            //    var card = CreateCard_Me(hand[i]);
            //    card.Type = CardOwner.Me;
            //}
            //PlaceCards();
        }
        public void CreateCards_Oppo()
        {
            Logical.CreateCards_Oppo();
            foreach (var lCard in Logical.Cards)
            {
                var card = Card.Construct(lCard);
            }
            PlaceCards();
            //for (var i = 0; i < Room.User.HandSize; i++)
            //{
            //    CreateCard_Oppo();
            //}
            //PlaceCards();
        }

        //private Card CreateCard_Me(int id)
        //{
        //    var card = Card.Construct(user: this, frontId: id);
        //    Cards.Add(card);
        //    return card;
        //}
        //private Card CreateCard_Oppo()
        //{
        //    var card = Object.Instantiate(Card.Prefab, transform).GetComponent<Card>();
        //    Cards.Add(card);
        //    return card;
        //}
    }
}

namespace Basra.Client.Room
{
    public enum UserType { Me, Oppo }

    public class User
    {
        public static int HandTime = 8000;
        public static int HandSize = 4;

        private string Name;

        public List<Card> Cards { get; set; } = new List<Card>();

        public RoomManager Room { get; set; }

        public UniTaskTimer UniTaskTimer;

        public int TurnId { get; set; }

        public UserType Type { get; set; }

        public User(RoomManager room, string name, int turnId)
        {
            Room = room;
            TurnId = turnId;

            //var user = Object.Instantiate(Prefab).GetComponent<User>();
            //transform.position = UserPositions[turnId];
            //transform.eulerAngles = UserRotations[turnId];

            Type = turnId == RoomManager.MyTurnId ? UserType.Me : UserType.Oppo;

            UniTaskTimer = new UniTaskTimer(HandTime, 100);

            if (Type == UserType.Me)
            {
                UniTaskTimer.Elapsed += OnTurnTimeout;
            }
        }

        private void OnTurnTimeout()
        {
            AppManager.I.HubConnection.Send("InformTurnTimeout");
            //this can't be instant because the random algo is not excpected
        }

        public void EnterTurn()
        {
            UniTaskTimer.Play().Forget((ecx) => Debug.LogError(ecx.Message));
        }

        public void CancelTurn()
        {
            UniTaskTimer.Stop();
        }

        public void CreateCards_Me(int[] hand)
        {
            for (var i = 0; i < hand.Length; i++)
            {
                //var card = Card.Construct(user: this, frontId: id);
                var card = new Card(user: this, frontId: hand[i]);
                card.Type = CardOwner.Me;

                Cards.Add(card);
            }
        }
        public void CreateCards_Oppo()
        {
            for (var i = 0; i < HandSize; i++)
            {
                //var card = Object.Instantiate(Card.Prefab, transform).GetComponent<Card>();
                var card = new Card(user: this);
                card.Type = CardOwner.Oppo;
                Cards.Add(card);
            }
            //PlaceCards();
        }

    }
}