using UnityEngine;

public class MessageButton : MonoBehaviour
{
    public void OnClick()
    {
        Controller.I.SendAsync("ShowMessage", name);

        ChatSystem.I.ShowMessage(RoomSettings.I.MyTurn, name);

        ChatSystem.I.ChatPanel.SetActive(false);
    }
}