using System.Collections;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;
using Zenject;

namespace Basra.Client.Room
{
    public enum CardOwner
    {
        Oppo,
        Me,
        Ground
    }

    public class CardInterface : MonoBehaviour, ICardInterface
    {
        public Transform Transform => transform;

        public void Hide() => gameObject.SetActive(false);
    }

    public interface ICardInterface
    {
        Transform Transform { get; }
        void Hide();
    }

    public class CardInterfaceFactory : PlaceholderFactory<ICardInterface>
    {
    }

    public interface ICard
    {
        ICardInterface Interface { get; }
        CardOwner Type { get; set; }
        Front Front { get; set; }
        public IUser User { get; set; }

        void Throw();
        void OppoThrow(int cardId);
    }

    public class Card : ICard
    {
        public ICardInterface Interface { get; }
        private readonly IRoom _room;
        public static Vector2 Bounds = new Vector2(.75f, 1f);

        public CardOwner Type { get; set; }

        //works on my, ground
        public Front Front { get; set; }

        //works on my, oppo
        public IUser User { get; set; }

        public static GameObject Prefab { get; set; }

        public Card(int frontId, IUser user, IRoom room, CardInterfaceFactory cardInterfaceFactory)
        {
            _room = room;
            Interface = cardInterfaceFactory.Create();

            // cardInterface.Transform.SetParent(user != null ? user..transform : ground.transform);
            // var card = Object.Instantiate(Prefab,)
            //     .GetComponent<Card>();
            // card.construct(user, ground, frontId);
            // return card;

            if (user != null) User = user;
            if (frontId != -1) AddFront(frontId);
        }

        public static async UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Card");
        }

        private void OnMouseDown()
        {
            Debug.Log($"OnMouseDown on card {Front?.Id}");

            if (User == null || Type != CardOwner.Me || User.TurnId != _room.CurrentTurn) return;

            MyThrow();
        }

        TransformValue RecordedTransform;

        private void RecordThrow()
        {
            RecordedTransform.LoadTo(Interface.Transform);
        }

        private void ThrowPt1()
        {
            Debug.Log($"ThrowPt1 on user {User.TurnId} and card {Front.Id} with index {User.Cards.IndexOf(this)}");

            User.TurnTimer.Stop(); //no reverse

            _room.Ground.AddPt1(this);

            _room.NextTurn();
        }

        private void ThrowPt2()
        {
            User.Cards.Remove(this);
            _room.Ground.AddPt2(this);
        }

        private void RevertThrow()
        {
            Debug.Log("RevertThrow is called");
            _room.RevertTurn();

            Type = CardOwner.Me;
            RecordedTransform.LoadTo(Interface.Transform);
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

            NetManager.I.LastRevertAction = RevertThrow;
            NetManager.I.Send("Throw", User.Cards.IndexOf(this)).OnSuccess((future) => ThrowPt2());
        }

        public void OppoThrow(int cardId)
        {
            Debug.Log($"OppoThrow is called by user id: {User.TurnId}, with front value: {cardId}");

            AddFront(cardId);

            Throw();
        }

        private void AddFront(int id)
        {
            Front = Object.Instantiate(Front.Prefab).GetComponent<Front>();
            Front.transform.localPosition = Vector3.back * .01f;
            Front.Set(id);
        }

        public class Factory : PlaceholderFactory<IUser, int, ICard>
        {
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