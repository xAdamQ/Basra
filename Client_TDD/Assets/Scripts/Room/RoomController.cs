using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;

public interface IRoomController
{
    void FinalizeGame();
    void NextTurn(); //trivial to test

    int CurrentTurn { get; }

    void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds); //trivial to test
    void CurrentOppoThrow(ThrowResult throwResult); //trivial to test
    void ForcePlay(ThrowResult throwResult); //trivial to test
    void DestroyModule();
    void MyThrowResult(ThrowResult throwResult);
    void PlayersDistribute(List<int> handCardIds);
}

public enum PlayerType
{
    Me,
    Oppo
}

public class RoomController : IRoomController, IInitializable
{
    private readonly IRepository _repository;
    private readonly IGround _ground;
    private readonly PlayerBase.Factory _playerFactory;
    private readonly RoomUserView.Factory _roomUserViewFactory;
    private readonly IController _controller;
    private readonly TurnTimer _turnTimer;
    private readonly RoomSettings _roomSettings;
    private readonly IBlockingPanel _blockingPanel;

    [Inject]
    public RoomController(IController controller, IRepository repository, IGround ground,
        PlayerBase.Factory playerFactory, RoomUserView.Factory roomUserViewFactory, TurnTimer turnTimer,
        RoomSettings roomSettings, IBlockingPanel blockingPanel)
    {
        _repository = repository;
        _ground = ground;
        _playerFactory = playerFactory;
        _roomUserViewFactory = roomUserViewFactory;
        _controller = controller;
        _turnTimer = turnTimer;
        _roomSettings = roomSettings;
        _blockingPanel = blockingPanel;
    }

    private List<PlayerBase> Players { get; } = new List<PlayerBase>();
    private PlayerBase PlayerInTurn => Players[CurrentTurn];
    public int CurrentTurn { get; private set; }

    public void MyThrowResult(ThrowResult throwResult)
    {
        //todo am i always 0 despite my turn id?
        (Players[0] as IPlayer).MyThrowResult(throwResult);
    }

    public void Initialize()
    {
        CreatePlayers();
        CreateUserViews();

        _controller.SendAsync("Ready").Forget(e => throw e);

        _blockingPanel.Hide();
    }

    private void CreatePlayers()
    {
        Players.Add(_playerFactory.Create(PlayerType.Me, 0));

        for (var i = 1; i < _roomSettings.Capacity; i++)
            Players.Add(_playerFactory.Create(PlayerType.Oppo, i));
    }

    private void CreateUserViews()
    {
        _roomUserViewFactory.Create(0, _repository.PersonalFullInfo);

        for (var i = 1; i < _roomSettings.Capacity; i++)
        {
            _roomUserViewFactory.Create(i, _roomSettings.OpposInfo[i - 1].FullUserInfo);
        }
    }

    public void NextTurn()
    {
        //todo suppress next turn on finalize
        CurrentTurn = ++CurrentTurn % Players.Count;
        _turnTimer.uniTaskTimer.Play().Forget();
    }

    public void FinalizeGame()
    {
        throw new System.NotImplementedException();
    }

    public void StartRoomRpc(List<int> handCardIds, List<int> groundCardIds)
    {
        _ground.InitialDistribute(groundCardIds);

        PlayersDistribute(handCardIds);

        Debug.Log($"hand cards are {string.Join(", ", handCardIds)}");

        CurrentTurn = -1;
        NextTurn();
    }

    public void PlayersDistribute(List<int> handCardIds)
    {
        (Players[0] as IPlayer).Distribute(handCardIds);

        for (var i = 1; i < _roomSettings.Capacity; i++)
            (Players[i] as IOppo).Distribute();
    }

    public void ForcePlay(ThrowResult throwResult)
    {
        ((IPlayer)PlayerInTurn).ServerThrow(throwResult);
    }

    public void CurrentOppoThrow(ThrowResult throwResult)
    {
        Debug.Log($"CurrentOppoThrow on user: {CurrentTurn} and card index: {throwResult.ThrownCard}");
        ((IOppo)PlayerInTurn).Throw(throwResult);
    }

    public void DestroyModule()
    {
        Object.Destroy(Object.FindObjectOfType<LobbyInstaller>().gameObject);
    }

    public class Factory : PlaceholderFactory<RoomSettings, RoomController>
    {
    }
}