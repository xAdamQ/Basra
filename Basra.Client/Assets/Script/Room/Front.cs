using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Basra.Client.Room.Components
{
    public class Front : MonoBehaviour
    {
        public Room.Front Logical;

        public Card Card;

        public static GameObject Prefab { get; private set; }
        public static Sprite[] FrontSprites;

        public async static UniTask StaticInit()
        {
            FrontSprites = await Addressables.LoadAssetAsync<Sprite[]>("FrontSprites");
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Front");
        }

        public static Front Construct(Room.Front lFront, Card card)
        {
            var front = Object.Instantiate(Front.Prefab, card.transform).GetComponent<Front>();
            return front;
        }
        public void _construct(Room.Front lFront, Card card)
        {
            Logical = lFront;
            Card = card;
            GetComponent<SpriteRenderer>().sprite = FrontSprites[lFront.Id];
        }

    }
}

namespace Basra.Client.Room
{
    public class Front
    {
        public int Id;

        public Front(int id)
        {
            Id = id;
        }

    }
}