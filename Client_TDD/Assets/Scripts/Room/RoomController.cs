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

    void StartGameRpc(List<int> handCardIds, List<int> groundCardIds); //trivial to test
    void OppoThrowRpc(int cardId, ThrowResponse throwResponse); //trivial to test
    void ServerThrowMyCardRpc(int cardHandIndex, ThrowResponse throwResponse); //trivial to test
    void Destroy();
}

public enum PlayerType
{
    Me,
    Oppo
}

//todo update view
//turn timer
//oppos

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

    // [Inject]
    // public RoomController(IController controller, IRepository repository, IGround ground,
    //     PlayerBase.Factory playerFactory, RoomUserView.Factory roomUserViewFactory, TurnTimer turnTimer)
    // {
    //     _repository = repository;
    //     _ground = ground;
    //     _playerFactory = playerFactory;
    //     _roomUserViewFactory = roomUserViewFactory;
    //     _controller = controller;
    //     _turnTimer = turnTimer;
    //     // _roomSettings = roomSettings;
    //     // _blockingPanel = blockingPanel;
    // }

    private List<PlayerBase> Players { get; }
    private PlayerBase PlayerInTurn => Players[CurrentTurn];
    public int CurrentTurn { get; private set; }

    public void Initialize()
    {
        // BetChoice = _settings.BetChoice;
        // CapacityChoice = _settings.CapacityChoice;

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
        CurrentTurn = ++CurrentTurn % Players.Count;

        _turnTimer.uniTaskTimer.Play().Forget();
    }

    public void FinalizeGame()
    {
        throw new System.NotImplementedException();
    }

    public void StartGameRpc(List<int> handCardIds, List<int> groundCardIds)
    {
        _ground.InitialDistribute(groundCardIds);

        for (var i = 0; i < _roomSettings.CapacityChoice; i++)
        {
            if (Players[i] is IOppo)
            {
                ((IOppo) Players[i]).Distribute();
            }
            else
            {
                ((IPlayer) Players[i]).Distribute(handCardIds);
            }
        }

        Debug.Log($"hand cards are {string.Join(", ", handCardIds)}");

        CurrentTurn = 0;
        _turnTimer.uniTaskTimer.Play().Forget();
    }

    public void ServerThrowMyCardRpc(int cardHandIndex, ThrowResponse throwResponse)
    {
        ((IPlayer) PlayerInTurn).ServerThrow(cardHandIndex, throwResponse);
    }

    public void OppoThrowRpc(int cardId, ThrowResponse throwResponse)
    {
        Debug.Log($"CurrentOppoThrow on user: {CurrentTurn} and card index: {cardId}");
        ((IOppo) PlayerInTurn).Throw(cardId, throwResponse);
    }

    public void Destroy()
    {
        Object.Destroy(Object.FindObjectOfType<LobbyInstaller>().gameObject);
    }

    public class Factory : PlaceholderFactory<RoomSettings, RoomController>
    {
    }
}