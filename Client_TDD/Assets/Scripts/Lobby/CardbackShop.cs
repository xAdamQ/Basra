using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class CardbackShop : MonoBehaviour
{
    //args
    [Inject] private readonly AssetReference _cardbackSheetRef;
    [Inject] private readonly GameObject cardbackShopItemPrefab;

    [Inject] private readonly IInstantiator _instantiator;
    [Inject] private readonly IRepository _repository;
    [Inject] private readonly BlockingOperationManager _blockingOperationManager;

    [SerializeField] private Transform gridView;
    [SerializeField] private GameObject shopPanel;

    private readonly List<CardbackShopItem> cardbackShopItems = new List<CardbackShopItem>();

    public async void ShowPanel()
    {
        await _blockingOperationManager.Start(LoadCardbacks());
        shopPanel.SetActive(true);
    }
    public void HidePanel()
    {
        shopPanel.SetActive(false);
        foreach (Transform t in gridView) Destroy(t.gameObject);
    }

    private Sprite[] cardbackSprites;

    private async UniTask LoadCardbacks()
    {
        if (cardbackSprites == null)
            Debug.Log("cardback sprites are unloaded");

        cardbackSprites ??= await _cardbackSheetRef.LoadAssetAsync<Sprite[]>();

        for (int i = 0; i < _repository.CardbackPrices.Length; i++)
        {
            var state = ShopItemState.Locked;
            var bought = _repository.PersonalFullInfo.OwnedCardBackIds.Contains(i);
            if (bought) state = ShopItemState.Unlocked;
            var inUse = _repository.PersonalFullInfo.SelectedCardback == i;
            if (inUse) state = ShopItemState.Set;

            var item = _instantiator.InstantiatePrefab(cardbackShopItemPrefab, gridView)
                .GetComponent<CardbackShopItem>();

            item.Init(state, _repository.CardbackPrices[i], cardbackSprites[i], i);

            cardbackShopItems.Add(item);
        }
    }

    public void UnselectedCurrentCard()
    {
        cardbackShopItems.First(c => c.State == ShopItemState.Set).SetState(ShopItemState.Unlocked);
    }
}