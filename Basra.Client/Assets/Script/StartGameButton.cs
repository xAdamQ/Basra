using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGameButton : MonoBehaviour
{
    [SerializeField]
    private int RoomGenre;
    [SerializeField]
    private int PlayerCount;

    public void StartGame()
    {
        AppManager.I.Lobby.AskForRoom(RoomGenre, PlayerCount);
    }
}
