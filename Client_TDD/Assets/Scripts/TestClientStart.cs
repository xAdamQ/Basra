using TMPro;
using UnityEngine;

public class TestClientStart : MonoBehaviour
{
    [SerializeField] TMP_Text idInput;

    public void StartClient()
    {
        Controller.I.TstStartClient(idInput.text);
        // Destroy(gameObject);
    }

    public void addChar(string chr)
    {
        if (idInput.text.Length >= 5) return;

        idInput.text += chr;
    }

    public void clearInput()
    {
        idInput.text = "";
    }
}