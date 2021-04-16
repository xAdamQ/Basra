using UnityEngine;
using Zenject;

public interface IRoom
{
    void PlayTurn(int cardIndexInHand);
    void FinalizeGame();
}

public class Room : MonoBehaviour, IRoom
{
    private IRepository _repository;

    [Inject]
    public void Construct(IRepository repository)
    {
        _repository = repository;
    }

    public void PlayTurn(int cardIndexInHand)
    {
        throw new System.NotImplementedException();
    }

    public void FinalizeGame()
    {
        throw new System.NotImplementedException();
    }
}