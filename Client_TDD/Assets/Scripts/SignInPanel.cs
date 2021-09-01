using Cysharp.Threading.Tasks;
using HmsPlugin;
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

    public void HuaweiSignIn()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("should sign in");
        HMSAccountManager.Instance.SignIn();
#endif
    }

    public static void Destroy()
    {
        if (i)
            Destroy(i.gameObject);
    }
}