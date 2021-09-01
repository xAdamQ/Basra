using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BlockingPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private Button dismissButton; //for both cancellation and dismiss

    private static BlockingPanel i;

    public static async UniTask Show(string message = null, Action dismissButtonAction = null)
    {
        if (i) Destroy(i.gameObject);
        //you can remove this to support multiple panels
        //but the new should draw over the old

        i = (await Addressables.InstantiateAsync("blockingPanel", ProjectReferences.I.Canvas))
            .GetComponent<BlockingPanel>();

        if (dismissButtonAction != null)
        {
            i.dismissButton.onClick.AddListener(() => dismissButtonAction());
            //if you want to reuse the same object make sure to save and remove this
            i.dismissButton.gameObject.SetActive(true);
        }
        else
        {
            i.dismissButton.gameObject.SetActive(false);
        }

        i.messageText.text = message ?? "";
    }

    public static void HideDismiss()
    {
        i.dismissButton.gameObject.SetActive(false);
    }

    //you shouldn't hide manually if you have cancellation action
    //cancel itself hide
    public static void Hide(string message = null)
    {
        if (string.IsNullOrEmpty(message))
        {
            if (i) //this line enables you to call hide aggressively
                Destroy(i.gameObject);
        }
        else
        {
            i.messageText.text = message;
            i.dismissButton.gameObject.SetActive(true);
        }
    }
}