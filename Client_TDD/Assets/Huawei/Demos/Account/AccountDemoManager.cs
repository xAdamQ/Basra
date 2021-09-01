using HuaweiMobileServices.Id;
using HuaweiMobileServices.Utils;
using UnityEngine;
using UnityEngine.UI;
using HmsPlugin;
using System.ComponentModel;
using Newtonsoft.Json;

public class AccountDemoManager : MonoBehaviour
{
    private const string NOT_LOGGED_IN = "No user logged in";
    private const string LOGGED_IN = "{0} is logged in";
    private const string LOGIN_ERROR = "Error or cancelled login";

    [SerializeField]
    private Text loggedInUser;

    void Start()
    {
        loggedInUser.text = NOT_LOGGED_IN;

        HMSAccountManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountManager.Instance.OnSignInFailed = OnLoginFailure;
    }

    public void LogIn()
    {
        HMSAccountManager.Instance.SignIn();
    }

    public void SilentSignIn()
    {
        HMSAccountManager.Instance.SilentSignIn();
    }

    public void LogOut()
    {
        HMSAccountManager.Instance.SignOut();
        loggedInUser.text = NOT_LOGGED_IN;
    }

    public void OnLoginSuccess(AuthAccount authHuaweiId)
    {
        loggedInUser.text = string.Format(LOGGED_IN, authHuaweiId.DisplayName);

        Debug.Log("authCode: " + authHuaweiId.AuthorizationCode);
        Debug.Log("accToken: " + authHuaweiId.AccessToken);
        Debug.Log("idToken: " + authHuaweiId.IdToken);
        Debug.Log("uid: " + authHuaweiId.Uid);
        Debug.Log("openId: " + authHuaweiId.OpenId);
    }

    public void OnLoginFailure(HMSException error)
    {
        loggedInUser.text = LOGIN_ERROR;
    }
}
