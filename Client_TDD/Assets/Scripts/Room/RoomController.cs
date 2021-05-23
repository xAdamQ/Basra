using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public interface IRoomController
{
    void FinalizeGame();
    void NextTurn(); //trivial to test

    int CurrentTurn { get; }

    void StartGameRpc(int[] handCardIds, int[] groundCardIds); //trivial to test
    void OppoThrowRpc(int cardId, ThrowResponse throwResponse); //trivial to test
    void ServerThrowMyCardRpc(int cardHandIndex, ThrowResponse throwResponse); //trivial to test
}

public enum PlayerType
{
    Me,
    Oppo
}

[UsedImplicitly]
public class RoomController : IRoomController, IInitializable
{
    //services
    private readonly IRepository _repository;
    private readonly IGround _ground;
    private readonly PlayerBase.Factory _playerFactory;
    private readonly RoomUserView.Factory _roomUserViewFactory;
    private readonly IController _controller;
    private readonly TurnTimer _turnTimer;
    private readonly IRoomRepo _roomRepo;

    private List<PlayerBase> Players { get; }
    private PlayerBase PlayerInTurn => Players[CurrentTurn];
    public int CurrentTurn { get; private set; }


    [Inject]
    public RoomController(IRepository repository, IGround ground, PlayerBase.Factory playerFactory,
        RoomUserView.Factory roomUserViewFactory, IController controller, TurnTimer turnTimer, IRoomRepo roomRepo)
    {
        _repository = repository;
        _ground = ground;
        _playerFactory = playerFactory;
        _roomUserViewFactory = roomUserViewFactory;
        _controller = controller;
        _turnTimer = turnTimer;
        _roomRepo = roomRepo;
    }

    public void Initialize()
    {
        // BetChoice = _settings.BetChoice;
        // CapacityChoice = _settings.CapacityChoice;

        CreatePlayers();
        CreateUserViews();

        _controller.SendAsync("Ready").Forget(e => throw e);
    }

    private void CreatePlayers()
    {
        Players.Add(_playerFactory.Create(PlayerType.Me, 0));

        for (var i = 1; i < _roomRepo.Capacity; i++)
        {
            Players.Add(_playerFactory.Create(PlayerType.Oppo, i));
        }
    }

    private void CreateUserViews()
    {
        _roomUserViewFactory.Create(0, _repository.PersonalFullInfo);

        for (var i = 1; i < _roomRepo.Capacity; i++)
        {
            _roomUserViewFactory.Create(i, _roomRepo.OpposInfo[i - 1]);
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

    public void StartGameRpc(int[] handCardIds, int[] groundCardIds)
    {
        _ground.InitialDistribute(groundCardIds);

        for (var i = 0; i < _roomRepo.CapacityChoice; i++)
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


// private void InitVisuals()
// {
//     _roomInterface.BetChoice = BetChoice.ToString();
// }

    public class Factory : PlaceholderFactory<RoomController>
    {
    }
}