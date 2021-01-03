using System.Collections;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Basra.Client.Room.Components
{
    public class Card : MonoBehaviour
    {
        public Room.Card Logical;

        public static Vector2 Bounds = new Vector2(.75f, 1f);

        #region refernces

        //works on my, ground
        public Front Front;

        //works on my, oppo
        public User User;
        public Ground Ground;

        #endregion

        public static GameObject Prefab { get; set; }

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Card");
        }

        public static Card Construct(Room.Card lCard, User user = null, Ground ground = null)
        {
            var parent = user ? user.transform : ground.transform;
            var card = Object.Instantiate(Prefab, parent).GetComponent<Card>();

            card.construct(lCard, user, ground);
            return card;
        }
        private void construct(Room.Card lCard, User user, Ground ground)
        {
            Logical = lCard;

            if (user != null) User = user;
            if (ground != null) Ground = ground;
        }

        private void OnMouseDown()
        {
            Debug.Log($"OnMouseDown on card {Front?.Logical.Id}");

            if (Logical.ICanThrow())
            {
                MyUnconfirmedThrow();
                RecordThrow();
            }
        }

        TransformValue RecordedTransform;
        private void RecordThrow()
        {
            RecordedTransform = new TransformValue(transform);
        }
        private void RevertThrow()
        {
            RecordedTransform.LoadTo(this);
            //reverse of ground pt1
        }

        public void MyUnconfirmedThrow()
        {
            Logical.MyUnconfirmedThrow();

            Client.Components.AppManager.I.LastRevertAction += RevertThrow;//overwrite logical revert
        }

        private void AddFront(int id)
        {
            Logical.AddFront(id);
            Front = Front.Construct(Logical.Front, this);
            Front.transform.localPosition = Vector3.back * .01f;
        }

        /*
        add calc funs and ui
        get the winner and add the ui
         */

        //focus and make the right or die and start over
        //but if testing you would be more consistant

    }
}

namespace Basra.Client.Room
{
    public enum CardOwner { Oppo, Me, Ground }

    public class Card
    {
        public CardOwner Type;

        #region refernces

        //works on my, ground
        public Front Front;

        //works on my, oppo
        public User User;
        public Ground Ground;

        #endregion

        public Card(User user = null, Ground ground = null, int frontId = -1)
        {
            //var card = Object.Instantiate(Prefab, user ? user.transform : ground.transform).GetComponent<Card>();
            if (user != null) User = user;
            if (ground != null) Ground = ground;
            if (frontId != -1) AddFront(frontId);
        }

        private void ThrowPt1()
        {
            Debug.Log($"ThrowPt1 is called by user id: {User.TurnId}, on card: {Front.Id}");

            User.UniTaskTimer.Stop();//no reverse

            User.Room.Ground.MyThrowPt1(this);

            User.Room.NextTurn();
        }
        private void ThrowPt2()
        {
            User.Cards.Remove(this);
            User.Room.Ground.MyThrowPt2(this);
        }
        private void RevertThrow()
        {
            Debug.Log("RevertThrow is called");

            User.Room.RevertTurn();

            Type = CardOwner.Me;
            //RecordedTransform.LoadTo(this);
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

        public bool ICanThrow()
        {
            return (User != null && Type == CardOwner.Me && User.TurnId == User.Room.CurrentTurn);
        }
        //this is public but you don't extent, it's utility

        public void MyUnconfirmedThrow()
        {
            //AppManager.I.SendUnconfirmed("Throw", onSuccess: ThrowPt2,
            //revert: RevertThrow, args: User.Cards.IndexOf(this));

            User.Room.AppManager.LastRevertAction += RevertThrow;
            //is cleaned after server response
            //override or pt2 is called before any next unconfirmed action
            User.Room.AppManager.HubConnection.Send("Throw", User.Cards.IndexOf(this)).OnSuccess((future) =>
            {
                User.Room.AppManager.LastRevertAction = null;
                ThrowPt2();
            });

            ThrowPt1();
        }

        public void OppoThrow(int cardId)
        {
            Debug.Log($"OppoThrow is called by user id: {User.TurnId}, with front value: {cardId}");

            AddFront(cardId);

            Throw();
        }

        public void AddFront(int id)
        {
            Front = new Front(id);
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

