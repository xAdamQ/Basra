using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public interface ICoreGameplay
{
    IPlayerBase PlayerInTurn { get; }
    int CurrentTurn { get; }

    void CreatePlayers();
    void InitialTurn();
    void NextTurn(bool endPrevTurn = true);
    void Distribute(List<int> handCardIds);
    void BeginGame(List<int> myHand, List<int> groundCards);
    void LastDistribute(List<int> handCardIds);
    UniTask EatLast(int lastEaterTurnId);
    void ResumeGame(List<int> myHand, List<int> ground, List<int> handCounts, int currentTurn);
}

public class CoreGameplay : ICoreGameplay, IInitializable, System.IDisposable
{
    [Inject] private readonly IGround _ground;
    [Inject] private readonly PlayerBase.Factory _playerFactory;
    [Inject] private readonly IController _controller;
    [Inject] private readonly RoomSettings _roomSettings;

    public void Initialize()
    {
        AssignRpcs();
    }

    public void Dispose()
    {
        _controller.RemoveModuleRpcs(nameof(CoreGameplay));
    }

    private List<IPlayerBase> Players { get; } = new List<IPlayerBase>();
    public IPlayerBase PlayerInTurn => Players[CurrentTurn];

    //this is server turn
    public int CurrentTurn { get; private set; }

    public async UniTask EatLast(int lastEaterTurnId)
    {
        await Players[lastEaterTurnId].EatLast();
    }

    public void CreatePlayers()
    {
        var oppoPlaceCounter = 1;
        //oppo place starts at 1 to 3

        for (int i = 0; i < _roomSettings.Capacity; i++)
        {
            if (_roomSettings.MyTurn == i)
            {
                MyPlayer = _playerFactory.Create(0, i) as IPlayer;
                Players.Add(MyPlayer);
            }
            else
            {
                var oppo = _playerFactory.Create(oppoPlaceCounter++, i) as IOppo;
                Players.Add(oppo);
                Oppos.Add(oppo);
            }
        }
    }

    private List<IOppo> Oppos { get; } = new List<IOppo>();
    private IPlayer MyPlayer { get; set; }

    public void InitialTurn()
    {
        CurrentTurn = -1;
        NextTurn(false);
    }

    public void NextTurn(bool endPrevTurn = true)
    {
        if (endPrevTurn) PlayerInTurn.EndTurn();

        CurrentTurn = ++CurrentTurn % _roomSettings.Capacity;

        if (PlayerInTurn.HandCards.Count == 0 && isLastDistribute) return;

        PlayerInTurn.StartTurn();
    }

    public void ResumeGame(List<int> myHand, List<int> ground, List<int> handCounts, int currentTurn)
    {
        _ground.Distribute(ground);

        MyPlayer.Distribute(myHand);

        for (int i = 0; i < handCounts.Count; i++)
        {
            if (i == _roomSettings.MyTurn) continue;

            ((IOppo)Players[i]).Distribute(handCounts[i]);
        }

        CurrentTurn = currentTurn - 1;
        NextTurn(false);
    }

    #region rpcs

    private void AssignRpcs()
    {
        _controller.AssignRpc<List<int>>(Distribute, nameof(CoreGameplay));
        _controller.AssignRpc<List<int>>(LastDistribute, nameof(CoreGameplay));
        _controller.AssignRpc<ThrowResult>(MyThrowResult, nameof(CoreGameplay));
        _controller.AssignRpc<ThrowResult>(ForcePlay, nameof(CoreGameplay));
        _controller.AssignRpc<ThrowResult>(CurrentOppoThrow, nameof(CoreGameplay));
    }

    private bool isLastDistribute;

    public void Distribute(List<int> handCardIds)
    {
        MyPlayer.Distribute(handCardIds);

        foreach (var oppo in Oppos)
        {
            oppo.Distribute();
        }
    }
    public void LastDistribute(List<int> handCardIds)
    {
        isLastDistribute = true;
        Distribute(handCardIds);
    }
    public void MyThrowResult(ThrowResult throwResult)
    {
        MyPlayer.MyThrowResult(throwResult);
    }
    public void ForcePlay(ThrowResult throwResult)
    {
        MyPlayer.ForceThrow(throwResult);
    }
    public void CurrentOppoThrow(ThrowResult throwResult)
    {
        ((IOppo)PlayerInTurn).Throw(throwResult);
    }

    #endregion


    public void BeginGame(List<int> myHand, List<int> groundCards)
    {
        _ground.Distribute(groundCards);

        Distribute(myHand);

        Debug.Log($"hand cards are {string.Join(", ", myHand)}");

        InitialTurn();
    }
}