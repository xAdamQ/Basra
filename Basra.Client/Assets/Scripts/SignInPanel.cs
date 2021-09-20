using System;
using Cysharp.Threading.Tasks;
using HmsPlugin;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SignInPanel : MonoBehaviour
{
    private static SignInPanel i;

    public static void Create()
    {
        UniTask.Create(async () =>
        {
            i = (await Addressables.InstantiateAsync("signInPanel", ProjectReferences.I.Canvas))
                .GetComponent<SignInPanel>();
        });
    }

    private void Start()
    {
        HMSAccountManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountManager.Instance.OnSignInFailed = OnLoginFailure;
    }

    public void HuaweiSignIn()
    {
        BlockingPanel.Show("getting huawei info")
            .Forget(e => throw e);

        HMSAccountManager.Instance.SignIn();
    }

    private void OnLoginFailure(HMSException exc)
    {
        BlockingPanel.Done("failed huawei login with exception: " + exc);
    }

    private void OnLoginSuccess(AuthAccount authAccount)
    {
        Controller.I.ConnectToServer(huaweiAuthCode: authAccount.AuthorizationCode);
    }

    public static void Destroy()
    {
        if (i) Destroy(i.gameObject);
    }
}
