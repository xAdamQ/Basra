using UnityEngine;
using UnityEngine.UI;

namespace Basra.Client
{
    public interface ILobbyInterface
    {
        string UserName { get; set; }
    }

    public class LobbyMonoInterface : MonoBehaviour, ILobbyInterface
    {
        [SerializeField] private Text UserNameText;

        public string UserName
        {
            get => UserNameText.text;
            set => UserNameText.text = value;
        }
    }
}