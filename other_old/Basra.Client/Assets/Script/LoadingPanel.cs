using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Basra.Client
{
    public class LoadingPanel : MonoBehaviour
    {
        public Text MessageText;
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