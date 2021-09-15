using System.Collections;
using System.Collections.Generic;
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

        protected IEnumerator LoadEss2()
        {
            var handle = Addressables.InstantiateAsync("canvas");
            yield return handle;

            canvas = handle.Result.GetComponent<Transform>();

            yield return Addressables.InstantiateAsync("camera");
            yield return Addressables.InstantiateAsync("eventSystem");
        }
    }
}