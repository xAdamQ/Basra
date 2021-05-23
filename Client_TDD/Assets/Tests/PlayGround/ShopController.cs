using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ShopController : IInitializable
{
    public class Factory : PlaceholderFactory<ShopController>
    {
    }

    [Inject] private InputHandler _inputHandler;
    [Inject] private Front.Factory _frontFactory;
    [Inject] private IInstantiator _instantiator;

    public void Initialize()
    {
        Debug.Log("ShopController is initialized");

        // dest().Forget();
    }

    async UniTask dest()
    {
        await UniTask.Delay(3000);
        Object.Destroy(GameObject.Find("ShopContext(Clone)"));
        Debug.Log("destroying");
    }
}