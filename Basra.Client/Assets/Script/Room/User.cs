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
        #region props

        #region public static

        public static int Size = 4;
        public static int HandTime = 8000;

        #endregion

        #region private static

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

        #endregion

        #region public

        private string Name;
        public void SetName(string name)
        {
            Name = name;
            NameText.text = name;
        }
        [SerializeField] TextMesh NameText;

        public List<Card> Cards { get; set; } = new List<Card>();

        public IRoomManager Room { get; set; }

        public TurnTimer TurnTimer;

        public int TurnId { get; set; }

        public UserType Type { get; set; }

        #endregion

        #region private

        private bool Constructed;

        #endregion

        #endregion

        static GameObject Prefab;

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("User");
        }

        public static User Construct(IRoomManager room, string name, int turnId)
        {
            var user = Object.Instantiate(Prefab).GetComponent<User>();
            user._construct(room, name, turnId);
            return user;
        }
        private void _construct(IRoomManager room, string name, int turnId)
        {
            if (Constructed) throw new System.Exception("the object is already constructed");
            Constructed = true;

            Room = room;
            SetName(name);
            TurnId = turnId;

            transform.position = UserPositions[turnId];
            transform.eulerAngles = UserRotations[turnId];
            Type = turnId == RoomManager.MyTurnId ? UserType.Me : UserType.Oppo;
        }

        private void Start()
        {
            if (Type == UserType.Me)
            {
                TurnTimer.AddToOnElapsed(OnTurnTimeout);
            }
        }

        private void OnTurnTimeout()
        {
            AppManager.I.HubConnection.Send("InformTurnTimeout");
            //this can't be instant because the random algo is not excpected
        }

        public void EnterTurn()
        {
            TurnTimer.Play();
        }

        public void CancelTurn()
        {
            TurnTimer.Stop();
        }

        //[Inject]
        //public User(IRoomManager)
        //{
        //    Go = Object.Instantiate(new GameObject()/*the user prefab*/);
        //    //the limitation of this approach is the configuration
        //    //and the archeticture is not unity's, so somthing bad may happen
        //    //I mean this script won't ever appear in editor despite it's existance
        //}

        private void PlaceCards()
        {
            var pointer = new Vector3(-(Size / 2) * Card.Bounds.x, 0, 0);
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
            for (var i = 0; i < hand.Length; i++)
            {
                var card = CreateCard_Me(hand[i]);
                card.Type = CardOwner.Me;
            }
            PlaceCards();
        }
        public void CreateCards_Oppo()
        {
            for (var i = 0; i < Size; i++)
            {
                CreateCard_Oppo();
            }
            PlaceCards();
        }

        private Card CreateCard_Me(int id)
        {
            var card = Card.Construct(user: this, frontId: id);
            Cards.Add(card);
            return card;
        }
        private Card CreateCard_Oppo()
        {
            var card = Object.Instantiate(Card.Prefab, transform).GetComponent<Card>();
            Cards.Add(card);
            return card;
        }
    }
}

namespace Basra.Client.Room
{
    public enum UserType { Me, Oppo }

    public class User
    {
        public static int HandTime = 8000;

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

        private string Name;
        public void SetName(string name)
        {
            Name = name;
            NameText.text = name;
        }
        [SerializeField] TextMesh NameText;

        public List<Card> Cards { get; set; } = new List<Card>();

        public IRoomManager Room { get; set; }

        public UniTaskTimer UniTaskTimer;

        public int TurnId { get; set; }

        public UserType Type { get; set; }

        static GameObject Prefab;

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("User");
        }

        public User(IRoomManager room, string name, int turnId)
        {
            Room = room;
            SetName(name);
            TurnId = turnId;

            //var user = Object.Instantiate(Prefab).GetComponent<User>();
            //transform.position = UserPositions[turnId];
            //transform.eulerAngles = UserRotations[turnId];

            Type = turnId == RoomManager.MyTurnId ? UserType.Me : UserType.Oppo;

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
            UniTaskTimer.Play().Forget();
        }

        public void CancelTurn()
        {
            UniTaskTimer.Stop();
        }

        public void CreateCards_Me(int[] hand)
        {
            for (var i = 0; i < hand.Length; i++)
            {
                var card = CreateCard_Me(hand[i]);
                card.Type = CardOwner.Me;
            }
            PlaceCards();
        }
        public void CreateCards_Oppo()
        {
            for (var i = 0; i < Size; i++)
            {
                CreateCard_Oppo();
            }
            PlaceCards();
        }

        private Card CreateCard_Me(int id)
        {
            var card = Card.Construct(user: this, frontId: id);
            Cards.Add(card);
            return card;
        }
        private Card CreateCard_Oppo()
        {
            var card = Object.Instantiate(Card.Prefab, transform).GetComponent<Card>();
            Cards.Add(card);
            return card;
        }
    }
}