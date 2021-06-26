using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

//blocking flags
//when you play I wait for the server
//you can't play again
//when you buy an item (affect money)
//you can't buy again
//BuyFlag, PlayFlag

public class CardbackShopItem : MonoBehaviour
{
    private int index;
    public ShopItemState State;
    private int price;

    [SerializeField] private ShopItemStateView stateView;
    [SerializeField] private Image cardbackImage;

    [Inject] private readonly IController _controller;
    [Inject] private readonly BlockingOperationManager _blockingOperationManager;
    [Inject] private readonly IRepository _repository;
    [Inject] private readonly CardbackShop _cardbackShop;
    [Inject] private readonly IToast _toast;
    //injecting is not expensive because they're just references, I hope reflection baking make it fast

    public void Init(ShopItemState shopItemState, int price, Sprite sprite, int index)
    {
        this.index = index;
        cardbackImage.sprite = sprite;
        this.price = price;

        SetState(shopItemState);
    }

    public void SetState(ShopItemState shopItemState)
    {
        State = shopItemState;

        switch (shopItemState)
        {
            case ShopItemState.Locked:
                stateView.Background.color = Color.red;
                stateView.Desc.text = price + "$";
                break;
            case ShopItemState.Unlocked:
                stateView.Background.color = Color.green;
                stateView.Desc.text = "available";
                break;
            case ShopItemState.Set:
                stateView.Background.color = Color.yellow;
                stateView.Desc.text = "used";
                break;
        }
    }

    public void OnClick()
    {
        switch (State)
        {
            case ShopItemState.Locked:
                _blockingOperationManager.Forget(TryUnlock());
                break;
            case ShopItemState.Unlocked:
                _blockingOperationManager.Forget(TrySet());
                break;
        }
    }

    //this require server assertion
    private async UniTask TryUnlock()
    {
        //client assertion
        if (State != ShopItemState.Locked)
        {
            Debug.LogError($"cardback with index {index} is already unlocked or set, what the fuck you're doing");
            return;
        }
        if (_repository.PersonalFullInfo.Money < price)
        {
            _toast.Show("no enough money", 2);
            return;
        }

        await _controller.BuyCardback(index);
        //stop interaction? it depends on whether interacting will create issues or not.
        //and usually it will
        //await feedback
        //(1) block all interactions
        //(2) block this item only
        //unless you have a mechanism for blocking individual items easily(which is not true)
        //for example closing and reopening the panel will discard the blocking!

        //if I am here, the server assured

        _repository.PersonalFullInfo.Money -= price; //propagate visually
        SetState(ShopItemState.Unlocked); //change the visuals also
    }
    private async UniTask TrySet()
    {
        switch (State)
        {
            //instant client validation
            case ShopItemState.Locked:
                Debug.LogError($"cardback with index {index} is issued set while it's locked");
                return;
            case ShopItemState.Set:
                return;
        }

        await _controller.SelectCardback(index);

        _cardbackShop.UnselectedCurrentCard();
        SetState(ShopItemState.Set);
    }
}