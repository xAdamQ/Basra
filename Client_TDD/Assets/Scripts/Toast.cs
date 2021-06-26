using System.Collections;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public interface IToast
{
    /// <param name="seconds"> auto hide after seconds, -1 disable auto hide</param>
    void Show(string message, float seconds = -1);
    void Hide();
}

public class ConsoleToast : IToast
{
    public void Show(string message, float seconds)
    {
        Debug.Log($"toast showed with message {message ?? ""}");
    }

    public void Hide()
    {
        Debug.Log("toast hide");
    }
}

public class Toast : MonoBehaviour, IToast
{
    [SerializeField] private Text messageText;

    public void Show(string message, float seconds = -1)
    {
        messageText.text = message ?? "";

        var milliSeconds = (int)(seconds * 1000);
        UniTask.Delay(milliSeconds).ContinueWith(Hide).Forget();

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        messageText.text = "";
        gameObject.SetActive(false);
    }
}