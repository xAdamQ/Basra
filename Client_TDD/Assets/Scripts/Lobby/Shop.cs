using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public enum ItemType
{
    Cardback,
    Background,
}

public class Shop : MonoBehaviour
{
    [SerializeField] private GameObject shopItemPrefab;

    [SerializeField] private Transform layoutGroup;
    [SerializeField] private GameObject shopPanel;

    private readonly List<ShopItem> shopItems = new List<ShopItem>();

    public ItemType ItemType { get; private set; }

    public static async UniTask Create(Transform parent, ItemType itemType)
    {
        (await Addressables.InstantiateAsync(itemType == ItemType.Cardback ? "cardbackShop" : "backgroundShop", parent))
            .GetComponent<Shop>().ItemType = itemType;
    }

    public static Shop Active;

    /// <summary>
    /// uses BlockingOperationManager
    /// </summary>
    public async void ShowPanel()
    {
        if (Active) Active.HidePanel();
        Active = this;

        await BlockingOperationManager.I.Start(LoadItems());
        shopPanel.SetActive(true);
    }
    public void HidePanel()
    {
        Active = null;
        shopPanel.SetActive(false);
        foreach (Transform t in layoutGroup) Destroy(t.gameObject); //optional
    }

    private Sprite[] itemSprites;

    /// <summary>
    /// uses Repository
    /// </summary>
    private async UniTask LoadItems()
    {
        if (itemSprites == null)
            Debug.Log("item sprites are unloaded");

        itemSprites ??=
            ItemType == ItemType.Cardback
                ? await Addressables.LoadAssetAsync<Sprite[]>("cardbackSprites")
                : (await Addressables.LoadAssetsAsync<Sprite>("background", _ => { })).ToArray();

        var itemType = (int) ItemType;

        for (int i = 0; i < Repository.ItemPrices[itemType].Length; i++)
        {
            var bought = Repository.I.PersonalFullInfo.OwnedItemIds[itemType].Contains(i);
            var inUse = Repository.I.PersonalFullInfo.SelectedItem[itemType] == i;

            var state = ShopItemState.Locked;
            if (bought) state = ShopItemState.Unlocked;
            if (inUse) state = ShopItemState.Set;

            //this could happen by create pattern also
            var item = Instantiate(shopItemPrefab, layoutGroup).GetComponent<ShopItem>();
            item.Init(state, Repository.ItemPrices[itemType][i], itemSprites[i], i);
            shopItems.Add(item);
        }
    }

    public void UnselectedCurrentCard()
    {
        shopItems.First(c => c.State == ShopItemState.Set).SetState(ShopItemState.Unlocked);
    }
}