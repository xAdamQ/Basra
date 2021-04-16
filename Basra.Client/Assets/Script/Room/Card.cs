using System.Collections;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Basra.Client.Room
{
    public enum CardOwner { Oppo, Me, Ground }

    public class Card : MonoBehaviour
    {
        public static Vector2 Bounds = new Vector2(.75f, 1f);

        public CardOwner Type;

        //works on my, ground
        public Front Front;

        //works on my, oppo
        public User User;

        public static GameObject Prefab { get; set; }

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Card");
        }

        public static Card Construct(User user = null, Ground ground = null, int frontId = -1)
        {
            var card = Object.Instantiate(Prefab, user ? user.transform : ground.transform).GetComponent<Card>();
            card.construct(user, ground, frontId);
            return card;
        }
        private void construct(User user, Ground ground, int frontId)
        {
            if (user != null) User = user;
            if (frontId != -1) AddFront(frontId);
        }

        private void OnMouseDown()
        {
            Debug.Log($"OnMouseDown on card {Front?.Id}");

            if (User == null || Type != CardOwner.Me || User.TurnId != User.Room.CurrentTurn) return;

            MyThrow();
        }

        TransformValue RecordedTransform;
        private void RecordThrow()
        {
            RecordedTransform = new TransformValue(transform);
        }
        private void ThrowPt1()
        {
            Debug.Log($"ThrowPt1 on user {User.TurnId} and card {Front.Id} with index {User.Cards.IndexOf(this)}");

            User.TurnTimer.Stop();//no reverse

            User.Room.Ground.AddPt1(this);

            User.Room.NextTurn();
        }
        private void ThrowPt2()
        {
            User.Cards.Remove(this);
            User.Room.Ground.AddPt2(this);
        }
        private void RevertThrow()
        {
            Debug.Log("RevertThrow is called");
            User.Room.RevertTurn();

            Type = CardOwner.Me;
            RecordedTransform.LoadTo(this);
            //reverse of ground pt1
        }

        //if a unit test happening affects another one
        //so making a simulation won't be effective

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
            AppManager.I.HubConnection.Send("Throw", User.Cards.IndexOf(this)).OnSuccess((future) => ThrowPt2());
        }
        public void OppoThrow(int cardId)
        {
            Debug.Log($"OppoThrow is called by user id: {User.TurnId}, with front value: {cardId}");

            AddFront(cardId);

            Throw();
        }

        private void AddFront(int id)
        {
            Front = Object.Instantiate(Front.Prefab, transform).GetComponent<Front>();
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

