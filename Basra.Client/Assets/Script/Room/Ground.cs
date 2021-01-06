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

namespace Basra.Client.Room
{
    public class Ground : MonoBehaviour
    {
        public List<Card> Cards;

        public static Vector2 Bounds = new Vector2(1.5f, 2.5f);
        private IRoomManager Room;

        public static GameObject Prefab { get; private set; }

        public async static UniTask StaticInit()
        {
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Ground");
        }

        public static Ground Construct(IRoomManager room)
        {
            var ground = Object.Instantiate(Prefab, Vector3.zero, new Quaternion()).GetComponent<Ground>();
            ground._construct(room);
            return ground;
        }
        private void _construct(IRoomManager room)
        {
            Room = room;
        }

        public void CreateInitialCards(int[] ground)
        {
            for (var i = 0; i < ground.Length; i++)
            {
                var card = MakeCard(ground[i]);
                Distribute(card);
            }
        }

        public void Distribute(Card card)
        {
            Cards.Add(card);
            PlaceCard(card);
        }

        public Card MakeCard(int id)
        {
            var card = Card.Construct(ground: this, frontId: id);
            return card;
        }

        private void PlaceCard(Card card)
        {
            var xPoz = Random.Range(-Bounds.x, Bounds.x);
            var yPoz = Random.Range(-Bounds.y, Bounds.y);
            card.transform.position = new Vector3(xPoz, yPoz);
        }

        private List<Card> EatenCardsPending;
        public void AddPt1(Card card)
        {
            card.Type = CardOwner.Ground;
            PlaceCard(card);
            EatenCardsPending = Cards.GetRange(0, 1);
        }
        public void AddPt2(Card card)
        {
            card.User.Eaten.AddRange(EatenCardsPending);
            EatenCardsPending.ForEach((eatenCard) =>
            {
                eatenCard.gameObject.SetActive(false);
            });

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

