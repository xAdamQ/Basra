using UnityEngine;

public class RoomMessage : MonoBehaviour
{
    [SerializeField] public string Id;

    public void OnClick()
    {
        Controller.I.SendAsync("SendMessage", Id);
    }
}