using UnityEngine;

public class TestClientStart : MonoBehaviour
{
    public void StartClient(string index)
    {
        Controller.I.TstStartClient(index);
    }
}