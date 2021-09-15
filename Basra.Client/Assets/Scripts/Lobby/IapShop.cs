using System;
using Cysharp.Threading.Tasks;
using HmsPlugin;
using HuaweiMobileServices.IAP;
using UnityEngine;

public class IapShop : MonoModule<IapShop>
{
    public static void Create()
    {
        Create("iapShop", LobbyReferences.I.Canvas)
            .Forget(e => throw e);
    }

    private void Start()
    {
        HMSIAPManager.Instance.OnBuyProductSuccess += OnPurchaseSuccess;
    }

    private void OnDestroy()
    {
        HMSIAPManager.Instance.OnBuyProductSuccess -= OnPurchaseSuccess;
    }

    private void OnPurchaseSuccess(PurchaseResultInfo obj)
    {
        Controller.I.Send("MakePurchase", obj.InAppPurchaseDataRawJSON, obj.InAppDataSignature);
    }

    public void MakePurchase(string productId)
    {
        HMSIAPManager.Instance.BuyProduct(productId);
    }
}