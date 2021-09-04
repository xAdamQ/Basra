using System;
using Cysharp.Threading.Tasks;
using HmsPlugin;
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

    public void HuaweiSignIn()
    {
        PlayerPrefs.SetInt("lang", langChoice.CurrentChoice);
        PlayerPrefs.Save();

        Translatable.CurrentLanguage = (Language)langChoice.CurrentChoice;

#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("huawei sign in");
        HMSAccountManager.Instance.SilentSignIn();
#endif
    }


    public static void Destroy()
    {
        if (i)
            Destroy(i.gameObject);
    }
}