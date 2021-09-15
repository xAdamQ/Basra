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

    [SerializeField] private ChoiceButton langChoice;
    [SerializeField] private TMP_Text langText;

    public static void Create()
    {
        UniTask.Create(async () =>
        {
            i = (await Addressables.InstantiateAsync("signInPanel", ProjectReferences.I.Canvas))
                .GetComponent<SignInPanel>();
        });
    }

    private void Awake()
    {
        langChoice.ChoiceChanged += c => langText.text = c == 0 ? "عربي" : "Enlgish";

        var lang = PlayerPrefs.GetInt("lang");
        langChoice.SetChoice(lang);
    }

    private void Start()
    {
        HMSAccountManager.Instance.OnSignInSuccess = OnLoginSuccess;
        HMSAccountManager.Instance.OnSignInFailed = OnLoginFailure;
    }

    private void OnLoginFailure(HMSException exc)
    {
        Debug.Log("failed huawei login with exception: " + exc);
        BlockingPanel.Hide("getting huawei info");
    }

    private void OnLoginSuccess(AuthAccount authAccount)
    {
        Controller.I.ConnectToServer(huaweiAuthCode: authAccount.AuthorizationCode);
    }

    public void HuaweiSignIn()
    {
        PlayerPrefs.SetInt("lang", langChoice.CurrentChoice);
        PlayerPrefs.Save();

        Translatable.CurrentLanguage = (Language)langChoice.CurrentChoice;

        BlockingPanel.Show("getting huawei info")
            .Forget(e => throw e);

        HMSAccountManager.Instance.SilentSignIn();
    }


    public static void Destroy()
    {
        if (i)
            Destroy(i.gameObject);
    }
}