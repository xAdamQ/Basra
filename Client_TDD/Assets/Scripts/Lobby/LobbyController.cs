using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface ILobbyController
{
    void StartRequestedRoom(FullUserInfo[] fullUsersInfos);
}

public class LobbyController : ILobbyController, IInitializable
{
    private IRepository _repository;
    private readonly IController _controller;

    [Inject]
    public LobbyController(IRepository repository, IController controller)
    {
        _repository = repository;
        _controller = controller;
    }

    public void Initialize()
    {
        _controller.AddLobbyRpcs(this);
    }

    public class Factory : PlaceholderFactory<LobbyController>
    {
    }

    public void StartRequestedRoom(FullUserInfo[] fullUsersInfos)
    {
    }
}