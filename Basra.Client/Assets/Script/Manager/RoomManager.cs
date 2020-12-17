using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;

namespace Basra.Client.Room
{
    //room has user room info like turn id
    //hand has 2 types.. oppo, mine

    public class RoomManager : MonoBehaviour
    {
        //this will work because room follows "Current" pattern
        //any static you have to reinit
        //they are static because they're set when there's no instance of this
        public static int Genre;
        public static int PlayerCount;
        public static int MyTurnId;
        public static string[] UserNames;

        [SerializeField] Text GenreText;

        private User[] Users;
        public Ground Ground;
        public int CurrentTurn;

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
            InitGround();
            InitVisuals();
        }

        #region init
        static float UpperPadding = 1.5f, ButtomPadding = 1f;
        static Vector2[] HandPozes = new Vector2[]
        {
            new Vector2(0, -5 + ButtomPadding),
            new Vector2(0, 5 - UpperPadding),
            new Vector2(2.5f, 0),
            new Vector2(-2.5f, 0),
        };
        static Vector3[] HandRotations = new Vector3[]
        {
            new Vector3(),
            new Vector3(0, 0, 180),
            new Vector3(0, 0, 90),
            new Vector3(0, 0, -90),
        };
        private void InitUsers()
        {
            Users = new User[PlayerCount];

            for (var i = 0; i < PlayerCount; i++)
            {
                var user = Instantiate(FrequentAssets.I.HandPrefab).GetComponent<User>();
                user.transform.position = HandPozes[i];
                user.transform.eulerAngles = HandRotations[i];
                user.Room = this;
                user.Name = UserNames[i];
                user.Type = i == MyTurnId ? UserType.Me : UserType.Oppo;
                user.TurnId = i;

                Users[i] = user;
            }
        }
        private void InitGround()
        {
            Ground = Instantiate(FrequentAssets.I.GroundPrefab, Vector3.zero, new Quaternion()).GetComponent<Ground>();
            Ground.Room = this;
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
            Ground.Set(ground);
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
                    Users[i].Set(hand);
                }
                else
                {
                    Users[i].Set();
                }
            }

            Debug.Log($"hand cards are {hand[0]} {hand[1]} {hand[2]} {hand[3]}");
        }

        //rev rpc
        public void Ready()
        {
            AppManager.I.HubConnection.SendAsync("Ready");
        }

        public void NextTurn()
        {
            CurrentTurn = ++CurrentTurn % Users.Length;
            UserInTurn.StartTurn();
        }
        public void ResetTurn()
        {

        }

        [Rpc]
        public void OppoThrow(int cardId)
        {
            var randomCard = Random.Range(0, UserInTurn.Cards.Count);
            UserInTurn.Cards[randomCard].OppoThrow(cardId);
        }
        [Rpc]
        public void OverrideThrow(int cardIndex)
        {
            UserInTurn.Cards[cardIndex].Throw();
        }

    }
}