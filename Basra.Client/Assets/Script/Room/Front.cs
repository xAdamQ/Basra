using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Cysharp.Threading.Tasks;

namespace Basra.Client.Room
{
    public class Front : MonoBehaviour
    {
        public int Id;

        public static GameObject Prefab { get; private set; }
        public static Sprite[] FrontSprites;

        public async static UniTask StaticInit()
        {
            FrontSprites = await Addressables.LoadAssetAsync<Sprite[]>("FrontSprites");
            Prefab = await Addressables.LoadAssetAsync<GameObject>("Front");
        }

        public void Set(int id)
        {
            Id = id;
            GetComponent<SpriteRenderer>().sprite = FrontSprites[id];
        }
    }
}