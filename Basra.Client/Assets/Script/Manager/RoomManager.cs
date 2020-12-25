using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;

namespace Basra.Client.Room
{
    //room has user room info like turn id
    //hand has 2 types.. oppo, mine

    public class RoomManager : MonoBehaviour, IRoomManager
    {
        //this will work because room follows "Current" pattern
        //any static you have to reinit
        //they are static because they're set when there's no instance of this
        public static int Genre;
        public static int PlayerCount;
        public static int MyTurnId;
        public static string[] UserNames;

        [SerializeField] Text GenreText;

        private User[] Users { get; set; }
        public Ground Ground { get; set; }
        public int CurrentTurn { get; set; }

        private User UserInTurn => Users[CurrentTurn];

        private void Awake()
        {
            AppManager.I.Room = this;
            // AppManager.I.Currents.RemoveAll(c => c.GetType() == GetType());
            AppManager.I.Managers.Add(this);
        }

        private void OnDestroy()
        {
            AppManager.I.Managers.Remove(this);
        }

        private void Start()
        {
            Ready();

            InitUsers();

            Ground = Ground.Construct(this);

            InitVisuals();
        }

        #region init
        private void InitUsers()
        {
            Users = new User[PlayerCount];

            for (var i = 0; i < PlayerCount; i++)
            {
                Users[i] = User.Construct(this, UserNames[i], i);
            }
        }
        private void InitVisuals()
        {
            GenreText.text = Genre.ToString();
        }
        #endregion

        //have something more than it's name, can be changes in the future
        [Rpc]
        public void InitialDistribute(int[] hand, int[] ground)
        {
            Ground.CreateInitialCards(ground);
            Distribute(hand);

            UserInTurn.StartTurn();
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
        public void Ready()
        {
            AppManager.I.HubConnection.SendAsync("Ready");
        }

        int PrevTurn;
        public void NextTurn()
        {
            PrevTurn = CurrentTurn;
            CurrentTurn = ++CurrentTurn % Users.Length;
            UserInTurn.StartTurn();
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