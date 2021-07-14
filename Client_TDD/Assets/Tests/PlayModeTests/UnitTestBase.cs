using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PlayModeTests
{
    public abstract class UnitTestBase
    {
        protected Transform canvas;

        protected async UniTask LoadEss()
        {
            canvas = (await Addressables.InstantiateAsync("canvas")).GetComponent<Transform>();
            (await Addressables.InstantiateAsync("camera")).GetComponent<Transform>();
            (await Addressables.InstantiateAsync("eventSystem")).GetComponent<Transform>();
        }

    }
}
