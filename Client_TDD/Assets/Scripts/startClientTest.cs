using UnityEngine;
using Zenject;

public class startClientTest : MonoBehaviour
{
    [Inject] private readonly IController _controller;

    public void startClient(string id)
    {
        _controller.TstStartClient(id);
    }
}