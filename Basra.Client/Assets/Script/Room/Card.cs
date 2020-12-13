using System.Collections;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Basra.Client.Room
{
    public enum CardType { Opponent, Mine, Ground }

    public class Card : MonoBehaviour
    {
        public static Vector2 Bounds = new Vector2(.75f, 1f);

        public CardType Type;

        //works on my, ground
        public Front Front;

        //works on my, oppo
        public User User;

        protected void Awake()
        {
            User = transform.parent.GetComponent<User>();

            switch (Type)
            {
                case CardType.Opponent:
                    break;
                case CardType.Mine:
                    Front = transform.GetChild(0).GetComponent<Front>();
                    break;
                case CardType.Ground:
                    Front = transform.GetChild(0).GetComponent<Front>();
                    break;
            }
        }

        //works on me
        private void OnMouseDown()
        {
            if (User.Type != UserType.Me || User.TurnId != User.Room.CurrentTurn) return;

            UnConfirmedThrow();
        }

        private void RecordThrow()
        {
            new InstantRpcRecord(ConfirmThrow);
            User.Room.Ground.RecordThrow(this);
        }
        private void VisualThrow()
        {
            User.Room.NextTurn();
            User.TurnTimer.Stop();
            //not recored, because it's not reverted

            User.Room.Ground.VisualThrow(this);
        }
        private void ConfirmThrow()
        {
            User.Cards.Remove(this);
            User.Room.Ground.ConfirmThrow(this);
        }//stored in delegate and can be called outside

        public void Throw()
        {
            if (User.Type != UserType.Me) throw new System.Exception("wrong user operation");

            //happens when it's time, not in the error message, the client is blocked before the server by seconds so there shouldn't be error possibilty
            //this design make the user init every action which is not true
            VisualThrow();
            ConfirmThrow();
        }
        private void UnConfirmedThrow()
        {
            RecordThrow();
            VisualThrow();

            //by initiator only
            AppManager.I.SendUnconfirmed("Throw", User.Cards.IndexOf(this));
        }

        public void OppoThrow(int id)
        {
            if (User.Type != UserType.Oppo) throw new System.Exception("wrong user operation");
            AddFront(id);
            Throw();
        }

        public void AddFront(int id)
        {
            Front = Instantiate(FrequentAssets.I.FrontPrefab, transform).GetComponent<Front>();
            Front.transform.localPosition = Vector3.back * .01f;
            Front.Set(id);
        }

        //2 types of design, instant feedback and server feedback
        //server feedback is easier
        //in the instant feedback you are blocked in some actions
        //in server feedback you're blocked by default

    }
}