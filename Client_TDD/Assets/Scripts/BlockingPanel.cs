using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public interface IBlockingPanel
{
    void Show(string message = null);
    void Hide(string message = null);
}

public class BlockingPanel : MonoBehaviour, IBlockingPanel
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private GameObject dismissButton;

    public static IBlockingPanel I;

    public static async UniTask Create()
    {
        I = (await Addressables.InstantiateAsync("blockingPanel", ProjectRefernces.I.Canvas)).GetComponent<BlockingPanel>();
    }

    private void Awake()
    {
        I = this;
    }

    public void Show(string message = null)
    {
        dismissButton.SetActive(false);
        gameObject.SetActive(true);
        messageText.text = message ?? "";
    }
    public void Hide(string message = null)
    {
        if (message != null)
        {
            messageText.text = message;
            dismissButton.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }
}