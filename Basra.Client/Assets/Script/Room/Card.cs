﻿using System.Collections;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Basra.Client.Room
{
    public enum CardOwner { Opponent, Mine, Ground }

    public class Card : MonoBehaviour
    {
        public static Vector2 Bounds = new Vector2(.75f, 1f);

        public CardOwner Type;

        //works on my, ground
        public Front Front;

        //works on my, oppo
        public User User;

        protected void Awake()
        {
            InitProps();

            SetType();
        }

        private void InitProps()
        {
            User = transform.parent.GetComponent<User>();

        }

        private void SetType()
        {
            switch (Type)
            {
                case CardOwner.Opponent:
                    break;
                case CardOwner.Mine:
                    Front = transform.GetChild(0).GetComponent<Front>();
                    break;
                case CardOwner.Ground:
                    Front = transform.GetChild(0).GetComponent<Front>();
                    break;
            }
        }

        //works on me
        private void OnMouseDown()
        {
            if (User.Type != UserType.Me || User.TurnId != User.Room.CurrentTurn) return;

            MyThrow();
        }

        TransformValue RecordedTransform;
        private void RecordThrow()
        {
            RecordedTransform = new TransformValue(transform);
        }
        private void ThrowPt1()
        {
            User.TurnTimer.Stop();
            User.Room.NextTurn();

            User.Room.Ground.ThrowPt1(this);
        }
        private void ThrowPt2()
        {
            User.Cards.Remove(this);
            User.Room.Ground.ThrowPt2(this);
        }
        private void RevertThrow()
        {
            RecordedTransform.LoadTo(this);
            Type = CardOwner.Mine;

        }

        //shared
        public void Throw()
        {
            //happens when it's time, not in the error message, the client is blocked before the server by seconds so there shouldn't be error possibilty
            //this design make the user init every action which is not true
            ThrowPt1();
            ThrowPt2();
        }
        private void MyThrow()
        {
            Debug.Log($"UnConfirmedThrow is called by user id: {User.TurnId}, on card: {Front.Id}");

            RecordThrow();
            ThrowPt1();

            AppManager.I.LastRevertAction = RevertThrow;
            //by initiator only
            AppManager.I.HubConnection.Send("Throw", User.Cards.IndexOf(this)).OnSuccess((future) => ThrowPt2());
        }
        public void OppoThrow(int cardId)
        {
            Debug.Log($"OppoThrow is called by user id: {User.TurnId}, with front value: {cardId}");

            AddFront(cardId);

            Throw();
        }

        public void AddFront(int id)
        {
            Front = Instantiate(FrequentAssets.I.FrontPrefab, transform).GetComponent<Front>();
            Front.transform.localPosition = Vector3.back * .01f;
            Front.Set(id);
        }

    }
}

#region general instant feedback with reflection, currenlty deprecated
// private void RecordThrow()
// {
//     User.Room.Ground.RecordThrow(this);
// }
// private void ReversibleThrow()
// {
//     User.TurnTimer.Stop();//no need for reverse
//     User.Room.NextTurn();
//     //record current turn
//     //reset timer(not data)

//     //not recored, because it's not reverted

//     User.Room.Ground.ReversibleThrow(this);
// }
// private void ConfirmThrow()
// {
//     User.Cards.Remove(this);
//     User.Room.Ground.ConfirmThrow(this);
// }//stored in delegate and can be called outside

// public void Throw()
// {
//     //happens when it's time, not in the error message, the client is blocked before the server by seconds so there shouldn't be error possibilty
//     //this design make the user init every action which is not true
//     ReversibleThrow();
//     ConfirmThrow();
// }
// private void UnConfirmedThrow()
// {
//     if (User.Type != UserType.Me) throw new System.Exception("wrong user operation");

//     new InstantRpcRecord(ConfirmThrow, this);

//     RecordThrow();
//     ReversibleThrow();

//     //by initiator only
//     AppManager.I.SendUnconfirmed("Throw", User.Cards.IndexOf(this));
// }
#endregion

