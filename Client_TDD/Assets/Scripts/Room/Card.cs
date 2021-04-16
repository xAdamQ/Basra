using UnityEngine;
using Zenject;

public interface ICard : IPlayerBase
{
}

public class Card : MonoBehaviour, ICard
{
    public class Factory : PlaceholderFactory<Card>
    {
    }
}