using Cysharp.Threading.Tasks;
using Script.Manager;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using Zenject;

namespace Basra.Client.Room
{
    public interface IRoom
    {
        int BetChoice { get; }
        int CapacityChoice { get; }
        int MyTurnId { get; }
        Ground Ground { get; set; }
        int CurrentTurn { get; set; }
        void Initialize();
        void InitialDistribute(int[] hand, int[] ground);
        void Distribute(int[] hand);

        /// <summary>
        /// the room is loaded, so the player is ready
        /// </summary>
        void Ready();

        void NextTurn();
        void RevertTurn();
        void CurrentOppoThrow(int cardId);
        void OverrideMyLastThrow(int cardIndex);
    }

    public class Room : IInitializable, IRoom
    {
        private readonly IRoomInterface _roomInterface;
        private readonly User.Factory _userFactory;

        public struct Settings
        {
            public int BetChoice;
            public int CapacityChoice;
            public int MyTurnId;
            public string[] UserNames;
        }

        public int BetChoice { get; }
        public int CapacityChoice { get; }
        public int MyTurnId { get; }
        private string[] UserNames { get; }

        private User[] Users { get; }

        public Ground Ground { get; set; }

        public int CurrentTurn { get; set; }
        private User UserInTurn => Users[CurrentTurn];


        [Inject]
        public Room(Settings settings, IRoomInterface roomInterface, User.Factory userFactory)
        {
            _roomInterface = roomInterface;
            _userFactory = userFactory;

            BetChoice = settings.BetChoice;
            CapacityChoice = settings.CapacityChoice;
            UserNames = settings.UserNames;
            MyTurnId = settings.MyTurnId;

            Users = new User[CapacityChoice];
        }

        public void Initialize()
        {
            Ground = Ground.Construct(this);
            for (var i = 0; i < CapacityChoice; i++)
            {
                _userFactory.Create(UserNames[i], i);
            }

            // Users[i] = User.Construct(this, UserNames[i], i);}
            Ready();
            InitVisuals();
        }

        //have something more than it's name, can be changes in the future
        [Rpc]
        public void InitialDistribute(int[] hand, int[] ground)
        {
            Ground.CreateInitialCards(ground);
            Distribute(hand);

            UserInTurn.EnterTurn();
        }

        [Rpc]
        public void Distribute(int[] hand)
        {
            for (var i = 0; i < CapacityChoice; i++)
            {
                if (Users[i].Type == UserType.Me)
                {
                    Users[i].CreateCards_Me(hand);
                }
                else
                {
                    Users[i].CreateCards_Oppo();
                }
            }

            Debug.Log($"hand cards are {hand[0]} {hand[1]} {hand[2]} {hand[3]}");
        }

        //rev rpc
        /// <summary>
        /// the room is loaded, so the player is ready
        /// </summary>
        public void Ready()
        {
            Debug.Log("calling ready");
            NetManager.I.Send("Ready");
        }

        int PrevTurn;

        public void NextTurn()
        {
            PrevTurn = CurrentTurn;
            CurrentTurn = ++CurrentTurn % Users.Length;
            UserInTurn.EnterTurn();
        }

        public void RevertTurn()
        {
            UserInTurn.CancelTurn();
            CurrentTurn = PrevTurn;
        }

        [Rpc]
        public void CurrentOppoThrow(int cardId)
        {
            Debug.Log($"CurrentOppoThrow on user: {CurrentTurn} and card index: {cardId}");

            var randomCard = Random.Range(0, UserInTurn.Cards.Count);
            UserInTurn.Cards[randomCard].OppoThrow(cardId);
        }

        [Rpc]
        public void OverrideMyLastThrow(int cardIndex)
        {
            UserInTurn.Cards[cardIndex].Throw();
        }

        private void InitVisuals()
        {
            _roomInterface.BetChoice = BetChoice.ToString();
        }
    }
}