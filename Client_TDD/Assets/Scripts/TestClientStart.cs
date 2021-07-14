using UnityEngine;
using Zenject;

public class TestClientStart : MonoBehaviour
{
    [Inject] private IController _controller;

    public void StartClient(string index)
    {
        _controller.TstStartClient(index);
    }

}
