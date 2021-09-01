using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HuaweiConstants;
using HuaweiMobileServices.Base;
using HuaweiMobileServices.IAP;
using System;
using UnityEngine.Events;
using HuaweiMobileServices.Id;
using HmsPlugin;
using HuaweiMobileServices.Utils;
using UnityEngine.UI;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

public class IapDemoManager : MonoBehaviour
{
    [SerializeField]
    private Text statusText;
    private List<InAppPurchaseData> productPurchasedList;

    // Please insert your products via custom editor. You can find it in Huawei > Kit Settings > IAP tab.

    void Start()
    {
        Debug.Log("[HMS]: IapDemoManager Started");
        HMSIAPManager.Instance.OnBuyProductSuccess += OnBuyProductSuccess;
        HMSIAPManager.Instance.OnCheckIapAvailabilitySuccess += OnCheckIapAvailabilitySuccess;
        HMSIAPManager.Instance.OnCheckIapAvailabilityFailure += OnCheckIapAvailabilityFailure;

        // Uncomment below if InitializeOnStart is not enabled in Huawei > Kit Settings > IAP tab.
        //HMSIAPManager.Instance.CheckIapAvailability();
    }

    private void OnCheckIapAvailabilityFailure(HMSException obj)
    {
        statusText.text = "IAP is not ready.";
    }

    private void OnCheckIapAvailabilitySuccess()
    {
        statusText.text = "IAP is ready.";
        HMSIAPManager.Instance.OnIsSandboxActivatedSuccess += res => Debug.Log("sandbox res is: " + JsonConvert.SerializeObject(res));
        HMSIAPManager.Instance.IsSandboxActivated();
    }

    public void SignIn()
    {
        HMSAccountManager.Instance.OnSignInSuccess += acc =>
         HMSIAPManager.Instance.CheckIapAvailability();

        HMSAccountManager.Instance.SilentSignIn();
    }

    private void RestorePurchases()
    {
        HMSIAPManager.Instance.RestorePurchases((restoredProducts) =>
        {
            productPurchasedList = new List<InAppPurchaseData>(restoredProducts.InAppPurchaseDataList);
        });
    }

    public void BuyProduct(string productID)
    {
        HMSIAPManager.Instance.BuyProduct(productID);
    }

    private void OnBuyProductSuccess(PurchaseResultInfo obj)
    {
        UniTask.Create(async () =>
        {
            await Controller.I.SendAsync("IAPBuy", obj.InAppPurchaseData.ProductId);

            switch (obj.InAppPurchaseData.ProductId)
            {
                case "money500":
                    Debug.Log("added 500 money on the server and the client should this");
                    break;

                case "money3000":
                    Debug.Log("added 500 money on the server and the client should this");
                    break;

                case "royalBg":
                    Debug.Log("the user owns the royal bg on the server now");
                    break;
            }
        });
    }
}