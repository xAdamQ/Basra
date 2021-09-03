using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public abstract class MonoModule<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T I;

    protected static async UniTask Create(string address, Transform parent)
    {
        I = (await Addressables.InstantiateAsync(address, parent)).GetComponent<T>();
    }

    protected virtual void Awake()
    {
        I = gameObject.GetComponent<T>(); //because "this" won't work
    }

    // public static void Destroy()
    // {
    //     Object.Destroy(I.gameObject);
    //     I = null;
    // }
}