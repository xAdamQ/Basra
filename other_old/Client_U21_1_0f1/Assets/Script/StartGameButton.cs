using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Basra.Client
{
    public class StartGameButton : MonoBehaviour
    {
        [SerializeField] private int RoomGenre;
        [SerializeField] private int PlayerCount;

        [Inject] private Lobby _lobby;

        public void StartGame()
        {
            _lobby.RequestRoom(RoomGenre, PlayerCount);
        }
    }
}