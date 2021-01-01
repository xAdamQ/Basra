using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

//the use of Gos in unity
//in your game context when you need them?
//represent actual visual elements in your scene
//so can these elemets relay on non-components?
//especially on unity events like mouse down?
//what makes me sure that I won't need the go to script relation

//the humle pattern seems over-work

namespace Basra.Client.Room.Components
{

    public class Ground : MonoBehaviour
    {
        public Room.Ground Logical;

        public static Vector2 Bounds = new Vector2(1.5f, 2.5f);

        public List<Card> Cards;
        private RoomManager Room;

        public static GameObject Prefab { get; private set; }

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Ground");
        }

        public static Ground Construct(Room.Ground logicalGround, RoomManager room)
        {
            var ground = Instantiate(Prefab, Vector3.zero, new Quaternion()).GetComponent<Ground>();
            ground._construct(logicalGround, room);
            return ground;
        }
        private void _construct(Room.Ground logicalGround, RoomManager room)
        {
            Logical = logicalGround;
            Room = room;
        }

        public void CreateInitialCards(int[] ground)
        {
            Logical.CreateInitialCards(ground);

            for (var i = 0; i < ground.Length; i++)
            {
                var card = Card.Construct(Logical.Cards[i], ground: this);
                Cards.Add(card);
                PlaceCard(card);
            }
        }

        private void PlaceCard(Card card)
        {
            var xPoz = Random.Range(-Bounds.x, Bounds.x);
            var yPoz = Random.Range(-Bounds.y, Bounds.y);
            card.transform.position = new Vector3(xPoz, yPoz);
        }

        public void ThrowPt1(Card card)
        {
            Logical.MyThrowPt1(card.Logical);

            PlaceCard(card);
        }
        public void ThrowPt2(Card card)
        {
            card.User = null;
            Cards.Add(card);
        }
    }
}

namespace Basra.Client.Room
{
    public class Ground
    {
        public static Vector2 Bounds = new Vector2(1.5f, 2.5f);

        public List<Card> Cards;
        public RoomManager Room;

        public Ground(RoomManager room)
        {
            Room = room;
        }

        public void CreateInitialCards(int[] ground)
        {
            for (var i = 0; i < ground.Length; i++)
            {
                var card = new Card(ground: this, frontId: i);
                Cards.Add(card);
            }
        }

        private Card[] LastEatenCards;
        public Card[] MyThrowPt1(Card card)
        {
            card.Type = CardOwner.Ground;
            return Cards.GetRange(0, 1).ToArray();
        }
        public void MyThrowPt2(Card card)
        {
            card.User = null;
            Cards.Add(card);
        }
    }
}

#region general instant feedback with reflection, currenlty deprecated
///every out rpc is visual, Real, Server
// public void RecordThrow(Card card)
// {
//     InstantRpcRecord.Current.RecordField(nameof(card.Type), card.Type);
//     InstantRpcRecord.Current.RecoredTransform(card);
// }
#endregion

