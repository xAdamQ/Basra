using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface ILobby
{
}

public class Lobby : ILobby, IInitializable
{
    private IRepository _repository;

    [Inject]
    public void Construct(IRepository repository)
    {
        _repository = repository;
    }

    public void Initialize()
    {
        Debug.Log("lobby is initialized");
    }

    public class Factory : PlaceholderFactory<Lobby>
    {
    }
}