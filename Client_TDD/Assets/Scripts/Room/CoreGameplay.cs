using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;

public interface ICoreGameplay
{
    PlayerBase PlayerInTurn { get; }
    int CurrentTurn { get; }

    void CreatePlayers();
    void InitalTurn();
    void NextTurn();
    void Distribute(List<int> handCardIds);
    void BeginGame(List<int> handCardIds, List<int> groundCardIds);
}

public class CoreGameplay : ICoreGameplay, IInitializable, System.IDisposable
{
    [Inject] private readonly IGround _ground;
    [Inject] private readonly PlayerBase.Factory _playerFactory;
    [Inject] private readonly ITurnTimer _turnTimer;
    [Inject] private readonly IController _controller;
    [Inject] private readonly IBlockingPanel _blockingPanel;
    [Inject] private readonly RoomSettings _roomSettings;

    public void Initialize()
    {
        AssignRpcs();
    }

    public void Dispose()
    {
        _controller.RemoveModuleRpcs(nameof(CoreGameplay));
    }

    //used to identify finalize game
    private const int TotalPossibleTurns = 52;
    private int TurnsCount;

    public static int ConvertTurnToPlayerIndex(int turn, int myTurn, int roomCapacity)
    {
        if (turn == myTurn) return 0;

        var newTurn = myTurn;
        for (var playerIndex = 1; playerIndex < roomCapacity; playerIndex++)
        {
            newTurn = ++newTurn % roomCapacity;
            if (newTurn == turn) return playerIndex;
        }

        throw new System.Exception("couldn't convert");
    }

    private List<PlayerBase> Players { get; } = new List<PlayerBase>();
    public PlayerBase PlayerInTurn => Players[ConvertTurnToPlayerIndex(CurrentTurn, _roomSettings.MyTurn, _roomSettings.Capacity)];
    //this is server turn
    public int CurrentTurn { get; private set; }

    public void CreatePlayers()
    {
        Players.Add(_playerFactory.Create(PlayerType.Me, 0));

        for (var i = 1; i < _roomSettings.Capacity; i++)
            Players.Add(_playerFactory.Create(PlayerType.Oppo, i));
    }

    public void InitalTurn()
    {
        CurrentTurn = -1;
        NextTurn();
    }
    public void NextTurn()
    {
        TurnsCount++;

        if (TurnsCount >= TotalPossibleTurns)
        {
            _blockingPanel.Show();// wait for finalize
            return;
        }

        if (CurrentTurn != -1)
            PlayerInTurn.EndTurn();

        CurrentTurn = ++CurrentTurn % Players.Count;

        _turnTimer.Play();

        PlayerInTurn.StartTurn();
    }

    private void AssignRpcs()
    {
        _controller.AssignRpc<List<int>>(Distribute, nameof(CoreGameplay));
        _controller.AssignRpc<ThrowResult>(MyThrowResult, nameof(CoreGameplay));
        _controller.AssignRpc<ThrowResult>(ForcePlay, nameof(CoreGameplay));
        _controller.AssignRpc<ThrowResult>(CurrentOppoThrow, nameof(CoreGameplay));
    }

    //Rpcs
    public void Distribute(List<int> handCardIds)
    {
        (Players[0] as IPlayer).Distribute(handCardIds);

        for (var i = 1; i < _roomSettings.Capacity; i++)
            (Players[i] as IOppo).Distribute();
    }
    public void MyThrowResult(ThrowResult throwResult)
    {
        (Players[0] as IPlayer).MyThrowResult(throwResult);
    }
    public void ForcePlay(ThrowResult throwResult)
    {
        ((IPlayer)PlayerInTurn).ForceThrow(throwResult);
    }
    public void CurrentOppoThrow(ThrowResult throwResult)
    {
        Debug.Log($"CurrentOppoThrow on user: {CurrentTurn} and card value: {throwResult.ThrownCard}");
        ((IOppo)PlayerInTurn).Throw(throwResult);
    }

    public void BeginGame(List<int> handCardIds, List<int> groundCardIds)
    {
        _ground.Distribute(groundCardIds);
        Distribute(handCardIds);

        Debug.Log($"hand cards are {string.Join(", ", handCardIds)}");

        InitalTurn();
    }
}