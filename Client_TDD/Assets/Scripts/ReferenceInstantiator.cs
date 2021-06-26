using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class ReferenceInstantiator
{
    [Inject] private DiContainer _diContainer;

    public void Instantiate(AssetReference assetReference, System.Action<GameObject> onComplete, Transform parent = null)
    {
        assetReference.InstantiateAsync(parent).Completed += handle =>
        {
            _diContainer.Inject(handle.Result);
            onComplete(handle.Result);
        };
    }
}