using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using Basra.Client.Components;

namespace Basra.Client.Room
{
    public class RoomManager : MonoBehaviour
    {
        public Room.RoomManager Logical;

        [SerializeField] Text GenreText;

        private User[] Users { get; set; }
        public Ground Ground { get; set; }

        public int CurrentTurn { get; set; }
        private User UserInTurn => Users[CurrentTurn];

        private void Awake()
        {
            Client.Components.AppManager.I.RoomManager = this;
            // _appManager.Currents.RemoveAll(c => c.GetType() == GetType());
            //_appManager.Managers.Add(this);
            Logical.OnInitialDistribute += InitialDistribute;
        }

        //private void OnDestroy()
        //{
        //    _appManager.Managers.Remove(this);
        //}

        private void Start()
        {
            InitUsers();
            Ground = Ground.Construct(Logical.Ground, this);
            InitVisuals();

            //Ready();
        }

        private void InitUsers()
        {
            Users = new User[Logical.PlayerCount];

            for (var i = 0; i < Users.Length; i++)
            {
                Users[i] = User.Construct(Logical.Users[i]);
            }
        }
        private void InitVisuals()
        {
            GenreText.text = Logical.Genre.ToString();
        }

        //have something more than it's name, can be changes in the future
        [Rpc]
        private void InitialDistribute(int[] hand, int[] ground)
        {
            Ground.CreateInitialCards(ground);
            Distribute(hand);

            UserInTurn.EnterTurn();
        }

        [Rpc]
        private void Distribute(int[] hand)
        {
            //for (var i = 0; i < Logical.PlayerCount; i++)
            //{
            //    if (Users[i].Type == UserType.Me)
            //    {
            //        Users[i].CreateCards_Me(hand);
            //    }
            //    else
            //    {
            //        Users[i].CreateCards_Oppo();
            //    }
            //}

            Debug.Log($"hand cards are {hand[0]} {hand[1]} {hand[2]} {hand[3]}");
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
        private void CurrentOppoThrow(int cardId)
        {
            var randomCard = Random.Range(0, UserInTurn.Cards.Count);
            //UserInTurn.Cards[randomCard].OppoThrow(cardId);
        }

        [Rpc]
        private void OverrideMyLastThrow(int cardIndex)
        {
            //UserInTurn.Cards[cardIndex].Throw();
        }

    }
}

namespace Basra.Client.aa
{

}

namespace Basra.Client.Room.LogicSystem
{
    public class RoomManager
    {
        public int Genre;
        public int PlayerCount;
        public int MyTurnId;
        public string[] UserNames;

        public User[] Users { get; set; }
        public Ground Ground { get; set; }

        public int CurrentTurn { get; set; }

        private User UserInTurn => Users[CurrentTurn];

        public AppManager AppManager;

        public RoomManager(AppManager appManager, int genre, int playerCount)
        {
            AppManager = appManager;

            Genre = genre;
            PlayerCount = playerCount;

            AppManager.RoomManager = this;

            // _appManager.Currents.RemoveAll(c => c.GetType() == GetType());
            AppManager.Managers.Add(this);

            InitUsers();
            Ground = new Ground(this);

            Ready();
        }

        public void Start(int myTurnId, string[] userNames)
        {
            MyTurnId = myTurnId;
            UserNames = userNames;
        }

        ~RoomManager()
        {
            AppManager.Managers.Remove(this);
        }

        //things that doesn't have refernces is exposed utility
        //things that is called by the system, mostly isn't legal to extend it
        private void InitUsers()
        {
            Users = new User[PlayerCount];

            for (var i = 0; i < PlayerCount; i++)
            {
                Users[i] = new User(this, UserNames[i], i);
            }
        }

        //have something more than it's name, can be changes in the future
        public event System.Action<int[], int[]> OnInitialDistribute;
        [Rpc]
        public void InitialDistribute(int[] hand, int[] ground)
        {
            Ground.CreateInitialCards(ground);
            Distribute(hand);

            UserInTurn.EnterTurn();

            OnInitialDistribute?.Invoke(hand, ground);
        }

        [Rpc]
        public void Distribute(int[] hand)
        {
            for (var i = 0; i < PlayerCount; i++)
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
        private void Ready()
        {
            AppManager.HubConnection.SendAsync("Ready");
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
            var randomCard = Random.Range(0, UserInTurn.Cards.Count);
            UserInTurn.Cards[randomCard].OppoThrow(cardId);
        }
        [Rpc]
        public void OverrideMyLastThrow(int cardIndex)
        {
            UserInTurn.Cards[cardIndex].Throw();
        }

    }
}