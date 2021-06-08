using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Basra.Client
{
    public interface ILoadingFeedback
    {
        void Show(string message = "");
        void Hide();
    }

    public class UUILoadingFeedback : MonoBehaviour, ILoadingFeedback
    {
        [SerializeField] Text MessageText;

        public void Show(string message = "")
        {
            MessageText.text = message;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}