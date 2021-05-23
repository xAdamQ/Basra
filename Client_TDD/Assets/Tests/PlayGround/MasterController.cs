using UnityEngine;
using Zenject;

public class MasterController : IInitializable
{
    [Inject] private ShopController.Factory _shopControllerFactory;

    public void Initialize()
    {
        Debug.Log("MasterController is initialized");
        _shopControllerFactory.Create();
    }
}