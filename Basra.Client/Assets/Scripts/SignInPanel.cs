#if UNITY_ANDROID
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

#if HMS
using HmsPlugin;
using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
#endif

#if GMS
using Facebook.Unity;
using UnityEngine.UI;
#endif

public class SignInPanel : MonoModule<SignInPanel>
{
    [SerializeField] private Button huaweiButton, facebookButton;

    public static async UniTaskVoid Create()
    {
        await Create("signInPanel", ProjectReferences.I.Canvas);

#if HMS
        I.facebookButton.gameObject.SetActive(false);
#endif
#if GMS
        I.huaweiButton.gameObject.SetActive(false);
#endif
    }

    private void Start()
    {
#if HMS
        HMSAccountManager.Instance.OnSignInSuccess = OnHuaweiLoginSuccess;
        HMSAccountManager.Instance.OnSignInFailed = OnHuaweiLoginFailure;
#endif
    }

    public void FbSignIn()
    {
#if GMS
        Debug.Log("logging to facebook");
        FB.LogInWithReadPermissions(
            new List<string> {"public_profile", "gaming_user_picture", "email"}, FbLoginCallback);
#endif
    }

#if GMS
    private void FbLoginCallback(ILoginResult result)
    {
        // if (!result.AccessToken.Permissions.Contains("public_profile"))
        // {
        // Toast.I.Show("you need to give profile permission", 3);
        // return;
        // }

        if (result.Error != null)
        {
            Debug.LogError("login ended with an error: " + result.Error);
            return;
        }

        if (result.Cancelled)
        {
            Debug.Log("login was canceled");
            return;
        }

        Debug.Log($"logged in with id {result.AccessToken.UserId} and accToken " +
                  $"{result.AccessToken.TokenString} and connecting to server now");

        Controller.I.ConnectToServer(facebookAccToken: result.AccessToken.TokenString);
        //puts blocking panel internally
    }
#endif

    public void HuaweiSignIn()
    {
#if HMS
        BlockingPanel.Show("getting huawei info")
            .Forget(e => throw e);

        HMSAccountManager.Instance.SignIn();
#endif
    }

#if HMS
    private void OnHuaweiLoginFailure(HMSException exc)
    {
        BlockingPanel.Done("failed huawei login with exception: " + exc);
    }

    private void OnHuaweiLoginSuccess(AuthAccount authAccount)
    {
        Controller.I.ConnectToServer(huaweiAuthCode: authAccount.AuthorizationCode);
    }
#endif
}
#endif