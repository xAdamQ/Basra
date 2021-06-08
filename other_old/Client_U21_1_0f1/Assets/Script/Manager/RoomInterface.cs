using UnityEngine;
using UnityEngine.UI;

namespace Script.Manager
{
    public interface IRoomInterface
    {
        string BetChoice { get; set; }
    }

    public class RoomInterface : MonoBehaviour, IRoomInterface
    {
        [SerializeField] private Text BetChoiceText;

        public string BetChoice
        {
            get => BetChoiceText.text;
            set => BetChoiceText.text = value;
        }
    }
}